using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace R3Modeller.Core.Engine.Properties {
    /// <summary>
    /// The base class for <see cref="R3Property{T}"/>
    /// </summary>
    public abstract class R3Property {
        private static readonly object RegistrationLock = typeof(R3Property);
        private static volatile int NextGlobalIndex;

        private readonly R3TypeRegistration registration;
        internal int structOffset;
        internal int structSize;
        internal int objectIndex;

        /// <summary>
        /// The index of this property for the entire application
        /// </summary>
        public int GlobalIndex { get; }

        /// <summary>
        /// The index of this property for the type hierarchy
        /// </summary>
        public int HierarchicalIndex { get; }

        /// <summary>
        /// The index of this property for its owner type
        /// </summary>
        public int LocalIndex { get; }

        /// <summary>
        /// The type that owns this property. The class that represents this type stores the underlying values, keyed by this property
        /// </summary>
        public Type OwnerType { get; }

        /// <summary>
        /// The type of the value that this property represents
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// The name of the property, used for debugging
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether or not this property's <see cref="TargetType"/> is a struct type
        /// </summary>
        public readonly bool IsStruct;

        /// <summary>
        /// The index of this property's struct value within the packed struct data. Throws an exception if <see cref="IsStruct"/> is false
        /// </summary>
        public int StructOffset {
            get => this.IsStruct ? throw new InvalidOperationException("Not a struct property") : this.structOffset;
        }

        /// <summary>
        /// The size of <see cref="TargetType"/> when it's a struct type. Throws an exception if <see cref="IsStruct"/> is false
        /// </summary>
        public int StructSize {
            get => this.IsStruct ? throw new InvalidOperationException("Not a struct property") : this.structSize;
        }

        protected R3Property(R3TypeRegistration registration, int globalIndex, int hierarchicalIndex, int localIndex, Type ownerType, Type targetType, string name, bool isStruct) {
            this.registration = registration;
            this.GlobalIndex = globalIndex;
            this.HierarchicalIndex = hierarchicalIndex;
            this.LocalIndex = localIndex;
            this.OwnerType = ownerType;
            this.TargetType = targetType;
            this.Name = name;
            this.IsStruct = isStruct;
        }

        private static void ValidateArgs(Type owner, string name, StackFrame srcFrame) {
            MethodBase callerMethod = srcFrame.GetMethod();
            if (callerMethod != null && callerMethod.DeclaringType != owner) {
                throw new Exception("Unsafe property registration usage: Property was registered in " + callerMethod.DeclaringType + " but targets type " + owner);
            }

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty or consist of only whitespaces", nameof(name));
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            Type lowest = typeof(R3Object);
            if (!lowest.IsAssignableFrom(owner))
                throw new ArgumentException($"Owner type is not applicable to {typeof(R3Object)}", nameof(owner));
            if (owner == lowest)
                throw new ArgumentException($"Owner type cannot be {typeof(R3Object)}", nameof(owner));
        }

        private static R3TypeRegistration GetRegistrationHelper(Type owner, string propertyName) {
            R3TypeRegistration registration = R3TypeRegistration.GetRegistration(owner, false);
            if (registration.isPacked) {
                throw new InvalidOperationException("Properties can only be registered before the type registry is fully baked");
            }

            if (registration.properties.Any(x => x.Name == propertyName)) {
                throw new Exception($"Property already registered with the name: '{propertyName}'");
            }

            return registration;
        }

        /// <summary>
        /// Registers a property for an unmanaged struct type
        /// </summary>
        /// <param name="owner">The owner type</param>
        /// <param name="name">The name of the property, for debugging</param>
        /// <typeparam name="TValue">The struct type</typeparam>
        /// <returns>A new property</returns>
        public static R3Property<TValue> RegisterU<TValue>(Type owner, string name) where TValue : unmanaged {
            ValidateArgs(owner, name, new StackFrame(1, false));
            lock (RegistrationLock) {
                R3TypeRegistration registration = GetRegistrationHelper(owner, name);
                int hIndex = registration.NextHierarchicalIndex++;
                int lIndex = registration.nextLocalIndex++;
                int gIndex = Interlocked.Increment(ref NextGlobalIndex);
                R3Property<TValue> property = new R3Property<TValue>(registration, gIndex, hIndex, lIndex, owner, typeof(TValue), name, true) {
                    structSize = Unsafe.SizeOf<TValue>()
                };

                R3TypeRegistration.AddInternal(registration, property);
                return property;
            }
        }

        /// <summary>
        /// Registers a property for a managed type (including structs, either managed or unmanaged)
        /// </summary>
        /// <param name="owner">The owner type</param>
        /// <param name="name">The name of the property, for debugging</param>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <returns></returns>
        public static R3Property<TValue> Register<TValue>(Type owner, string name) {
            ValidateArgs(owner, name, new StackFrame(1, false));
            lock (RegistrationLock) {
                R3TypeRegistration registration = GetRegistrationHelper(owner, name);
                int hIndex = registration.NextHierarchicalIndex++;
                int lIndex = registration.nextLocalIndex++;
                int gIndex = Interlocked.Increment(ref NextGlobalIndex);
                R3Property<TValue> property = new R3Property<TValue>(registration, gIndex, hIndex, lIndex, owner, typeof(TValue), name, false);
                R3TypeRegistration.AddInternal(registration, property);
                return property;
            }
        }

        public override string ToString() {
            return $"R3BProperty[{this.OwnerType}.{this.Name} | {this.TargetType} (G{this.GlobalIndex}, H{this.HierarchicalIndex}, L{this.LocalIndex})]";
        }
    }

    /// <summary>
    /// The main implementation of <see cref="R3Property"/>
    /// </summary>
    /// <typeparam name="T">The type of value being stored</typeparam>
    public class R3Property<T> : R3Property {
        internal R3Property(R3TypeRegistration registration, int globalIndex, int hierarchicalIndex, int localIndex, Type ownerType, Type targetType, string name, bool isStruct) : base(registration, globalIndex, hierarchicalIndex, localIndex, ownerType, targetType, name, isStruct) {
        }
    }
}