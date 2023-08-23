using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using R3Modeller.Core.RBC;

namespace R3Modeller.Core.Engine.Properties {
    /// <summary>
    /// A class for objects that store values for <see cref="R3Property{T}"/>
    /// </summary>
    public class R3Object {
        // TODO: combine live and cached data into a single array
        private readonly byte[] structData;
        private readonly object[] objectData;
        private readonly int sdCount, odCount;

        public R3TypeRegistration TypeRegistration { get; }

        public R3Object() {
            R3TypeRegistration registration = R3TypeRegistration.GetRegistration(this.GetType());
            this.TypeRegistration = registration;
            this.sdCount = registration.HierarchialStructSize;
            this.odCount = registration.HierarchialObjectCount;
            this.structData = new byte[this.sdCount * 2];
            this.objectData = new object[this.odCount * 2];
        }

        private static bool IsValidType(object value, Type propertyType) {
            if (value == null) {
                if (propertyType.IsValueType && (!propertyType.IsGenericType || !(propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))) {
                    return false;
                }
            }
            else if (!propertyType.IsInstanceOfType(value)) {
                return false;
            }

            return true;
        }

        private static void VerifyProperty(R3Object src, R3Property property) {
            if (!property.OwnerType.IsInstanceOfType(src)) {
                throw new Exception($"Incompatible property owner type. Property expects {property.OwnerType}, but the target was {src.GetType()}");
            }
        }

        /// <summary>
        /// Gets an unmanaged struct value for the given property
        /// </summary>
        /// <param name="property">The property</param>
        /// <typeparam name="T">The type of unmanaged struct</typeparam>
        /// <returns>The struct value</returns>
        /// <exception cref="Exception">Incompatible owner type, or the property is not for storing unmanaged structs</exception>
        public T GetValueU<T>(R3Property<T> property) where T : unmanaged {
            VerifyProperty(this, property);
            if (!property.IsStruct)
                throw new Exception("Property is not a struct type. Use " + nameof(this.GetValueM));
            return BinaryUtils.ReadStruct<T>(this.structData, property.structOffset, property.structSize);
        }

        /// <summary>
        /// Sets an unmanaged struct value for the given property. This will publish a new update
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="value">The new value</param>
        /// <typeparam name="T">The type of unmanaged struct</typeparam>
        /// <exception cref="Exception">Incompatible owner type, or the property is not for storing unmanaged structs</exception>
        public void SetValueU<T>(R3Property<T> property, T value) where T : unmanaged {
            VerifyProperty(this, property);
            if (!property.IsStruct)
                throw new Exception("Property is not a struct type. Use " + nameof(this.SetValueM));
            BinaryUtils.WriteStruct(value, this.structData, property.structOffset);
            R3Property.PushUpdateU(this, property);
        }

        /// <summary>
        /// Clears the unmanaged struct value for the given property (setting all bytes to 0). This will publish a new update
        /// </summary>
        /// <param name="property">The property whose value is to be cleared</param>
        /// <exception cref="Exception">Incompatible owner type, or the property is not for storing unmanaged structs</exception>
        public void ClearValueU(R3Property property) {
            VerifyProperty(this, property);
            if (!property.IsStruct)
                throw new Exception("Property is not a struct type. Use " + nameof(this.SetValueM) + " and pass null");
            BinaryUtils.WriteEmpty(this.structData, property.structOffset, property.structSize);
            R3Property.PushUpdateU(this, property);
        }

        /// <summary>
        /// Gets a managed value for the given property
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="value">The value</param>
        /// <exception cref="Exception">Incompatible owner type, or the property is for storing unmanaged structs</exception>
        public object GetValueM(R3Property property) {
            VerifyProperty(this, property);
            if (property.IsStruct)
                throw new Exception("Property is a struct type. Use " + nameof(this.GetValueU));
            return this.objectData[property.objectIndex];
        }

        /// <inheritdoc cref="GetValueM"/>
        public T GetValueM<T>(R3Property<T> property) => (T) this.GetValueM((R3Property) property);

        public void SetValueM(R3Property property, object value) {
            VerifyProperty(this, property);
            if (property.IsStruct)
                throw new Exception("Property is a struct type. Use " + nameof(this.SetValueU));
            this.objectData[property.objectIndex] = value;
            R3Property.PushUpdateU(this, property);
        }

        public void SetValueM<T>(R3Property<T> property, T value) => this.SetValueM((R3Property) property, value);

        /// <summary>
        /// Reads an unmanaged struct value (for the given property) from the cached (aka BufferB) data
        /// </summary>
        public T ReadValueU<T>(R3Property<T> property) where T : unmanaged {
            VerifyProperty(this, property);
            if (!property.IsStruct)
                throw new Exception("Property is not a struct type");
            return BinaryUtils.ReadStruct<T>(this.structData, this.sdCount + property.structOffset, property.structSize);
        }

        /// <summary>
        /// Reads a managed object (for the given property) from the cached (aka BufferB) data
        /// </summary>
        public T ReadValueM<T>(R3Property<T> property) {
            VerifyProperty(this, property);
            if (property.IsStruct)
                throw new Exception("Property is a struct type");
            return (T) this.objectData[this.odCount + property.objectIndex];
        }

        public void TransferValue(R3Property property) {
            int idx;
            if (property.IsStruct) {
                // copy live data (BufferA) to cached data (BufferB)
                idx = property.structOffset;
                Unsafe.CopyBlock(ref this.structData[idx + this.sdCount], ref this.structData[idx], (uint) property.structSize);
            }
            else {
                idx = property.objectIndex;
                this.objectData[idx + this.odCount] = this.objectData[idx];
            }
        }
    }
}