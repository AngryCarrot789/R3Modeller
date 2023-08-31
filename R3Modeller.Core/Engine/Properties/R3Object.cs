using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using R3Modeller.Core.RBC;

namespace R3Modeller.Core.Engine.Properties {
    /// <summary>
    /// A class for objects that store values for <see cref="R3Property{T}"/>
    /// </summary>
    public class R3Object {
        private static readonly List<TransferValueCommand> UpdateQueue;

        // TODO: combine live and cached data into a single array
        private readonly byte[] structData;
        private readonly object[] objectData;
        private readonly int[] propertyFlags;
        private readonly int sdCount, odCount;

        private const int IsUpdateScheduled = 0b0001;

        public R3TypeRegistration TypeRegistration { get; }

        public R3Object() {
            R3TypeRegistration registration = R3TypeRegistration.GetRegistration(this.GetType());
            this.TypeRegistration = registration;
            this.sdCount = registration.HierarchicalStructSize;
            this.odCount = registration.HierarchicalObjectCount;
            this.structData = new byte[this.sdCount * 2];
            this.objectData = new object[this.odCount * 2];
            this.propertyFlags = new int[registration.NextHierarchicalIndex];
        }

        static R3Object() {
            UpdateQueue = new List<TransferValueCommand>();
        }

        /// <summary>
        /// Enqueues a command that indicates that the BufferA data (from the given owner) for the given property
        /// should be written to BufferB when the main and render threads are synchronized
        /// </summary>
        /// <param name="owner">Owner instance</param>
        /// <param name="property">Property whose value has changed</param>
        /// <typeparam name="T">The type of value</typeparam>
        public static void OnPropertyChanged(R3Object owner, R3Property property) {
            if (!owner.ReadFlag(property, IsUpdateScheduled)) {
                owner.SetFlag(property, IsUpdateScheduled);
                UpdateQueue.Add(new TransferValueCommand(owner, property));
            }
        }

        public static void ProcessUpdates() {
            foreach (TransferValueCommand command in UpdateQueue) {
                command.Owner.TransferValue(command.Property);
                command.Owner.ClearFlag(command.Property, IsUpdateScheduled);
            }

            UpdateQueue.Clear();
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
            return BinaryUtils.ReadStruct<T>(this.structData, property.structOffset);
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
            OnPropertyChanged(this, property);
        }

        /// <summary>
        /// Clears the unmanaged struct value for the given property (setting all bytes to 0). This will publish a new update
        /// </summary>
        /// <param name="property">The property whose value is to be cleared</param>
        /// <exception cref="Exception">Incompatible owner type, or the property is not for storing unmanaged structs</exception>
        public void ClearValueU(R3Property property) {
            VerifyProperty(this, property);
            if (!property.IsStruct)
                throw new Exception("Property is not a struct type. Use " + nameof(this.ClearValueM));
            BinaryUtils.WriteEmpty(this.structData, property.structOffset, property.structSize);
            OnPropertyChanged(this, property);
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

        public void SetValueM<T>(R3Property<T> property, T value) => this.SetObjectInternal(property, value);

        public void ClearValueM(R3Property property) => this.SetObjectInternal(property, null);

        private void SetObjectInternal(R3Property property, object value) {
            VerifyProperty(this, property);
            if (property.IsStruct)
                throw new Exception("Property is a struct type. Use " + nameof(this.SetValueU));
            if (!IsValidType(value, property.TargetType))
                throw new ArgumentException($"Value ({value?.GetType()}) is not assignable to {property.TargetType}");
            this.objectData[property.objectIndex] = value;
            OnPropertyChanged(this, property);
        }

        /// <summary>
        /// Reads an unmanaged struct value (for the given property) from the cached (aka BufferB) data
        /// </summary>
        public T ReadValueU<T>(R3Property<T> property) where T : unmanaged {
            return BinaryUtils.ReadStruct<T>(this.structData, this.sdCount + property.structOffset);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TransferValue(R3Property property) {
            if (property.IsStruct) {
                int idx = property.structOffset;
                Unsafe.CopyBlock(ref this.structData[idx + this.sdCount], ref this.structData[idx], (uint) property.structSize);
            }
            else {
                int idx = property.objectIndex;
                this.objectData[idx + this.odCount] = this.objectData[idx];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReadFlag(R3Property p, int flag) => (this.propertyFlags[p.HierarchicalIndex] & flag) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetFlag(R3Property p, int flag) => this.propertyFlags[p.HierarchicalIndex] |= flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearFlag(R3Property p, int flag) => this.propertyFlags[p.HierarchicalIndex] &= ~flag;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetFlag(R3Property p, int flag, bool set) {
            if (set) {
                this.SetFlag(p, flag);
            }
            else {
                this.ClearFlag(p, flag);
            }
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
                throw new Exception($"Incompatible property owner type. Property is {property.OwnerType}, but the target was {src.GetType()}");
            }
        }

        private readonly struct TransferValueCommand {
            public readonly R3Object Owner;
            public readonly R3Property Property;

            public TransferValueCommand(R3Object owner, R3Property property) {
                this.Owner = owner;
                this.Property = property;
            }
        }
    }
}