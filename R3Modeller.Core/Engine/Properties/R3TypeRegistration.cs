using System;
using System.Collections.Generic;

namespace R3Modeller.Core.Engine.Properties {
    /// <summary>
    /// Stores information about a specific type that contains registered properties
    /// </summary>
    public class R3TypeRegistration {
        private static readonly Dictionary<Type, R3TypeRegistration> RegisteredTypes = new Dictionary<Type, R3TypeRegistration>();

        internal readonly List<R3Property> properties;
        internal int nextLocalIndex;
        internal bool isPacked;

        /// <summary>
        /// The type that this registration is associated with
        /// </summary>
        public Type OwnerType { get; }

        /// <summary>
        /// The total number of bytes that a packed array of bytes contains for storing struct entries that <see cref="OwnerType"/> instances store
        /// </summary>
        public int LocalPackedStructSize { get; private set; }

        /// <summary>
        /// The total number of object and managed struct entries that <see cref="OwnerType"/> instances store
        /// </summary>
        public int LocalPackedObjectSize { get; private set; }

        /// <summary>
        /// The total number of bytes that a packed array of bytes contains for storing struct entries this hierarchy stores
        /// </summary>
        public int HierarchicalStructSize { get; private set; }

        /// <summary>
        /// The total number of object and managed struct entries this hierarchy stores
        /// </summary>
        public int HierarchicalObjectCount { get; private set; }

        public int NextHierarchicalIndex { get; internal set; }

        internal R3TypeRegistration(Type ownerType) {
            this.OwnerType = ownerType;
            this.properties = new List<R3Property>();
        }

        public static R3TypeRegistration GetRegistration(Type ownerType, bool autoPack = true) {
            R3TypeRegistration registration;

            // lazy solution, probably affects overall performance but hopefully it's fine
            lock (RegisteredTypes) {
                if (!RegisteredTypes.TryGetValue(ownerType, out registration)) {
                    RegisteredTypes[ownerType] = registration = new R3TypeRegistration(ownerType);
                    R3TypeRegistration parent = GetParentRegistration(registration);
                    if (parent != null) {
                        registration.NextHierarchicalIndex = parent.NextHierarchicalIndex;
                    }
                }

                if (autoPack && !registration.isPacked) {
                    PackStructure(registration);
                }
            }

            return registration;
        }

        internal static void AddInternal(R3TypeRegistration r, R3Property property) {
            if (property.LocalIndex != r.properties.Count)
                throw new Exception("Invalid property registration order");
            r.properties.Add(property);
        }

        private static void PackStructure(R3TypeRegistration r) {
            Console.WriteLine("Baking registration for type " + r.OwnerType);
            R3TypeRegistration parent = GetParentRegistration(r);
            if (parent != null) {
                if (!parent.isPacked) {
                    PackStructure(parent);
                }

                r.HierarchicalObjectCount = parent.HierarchicalObjectCount;
                r.HierarchicalStructSize = parent.HierarchicalStructSize;
            }

            foreach (R3Property property in r.properties) {
                if (property.IsStruct) {
                    property.structOffset = r.HierarchicalStructSize;
                    r.HierarchicalStructSize += property.structSize;
                    r.LocalPackedStructSize += property.structSize;
                }
                else {
                    property.objectIndex = r.HierarchicalObjectCount;
                    r.HierarchicalObjectCount++;
                    r.LocalPackedObjectSize++;
                }
            }

            r.isPacked = true;
        }

        private static R3TypeRegistration GetParentRegistration(R3TypeRegistration r) {
            // compare type to R3BObject instead of `typeof(R3BObject).IsAssignableFrom(type)` as an optimisation

            // this function is used in a locked context, so RegisteredTypes access is fine
            for (Type type = r.OwnerType.BaseType; type != null && type != typeof(R3Object); type = type.BaseType) {
                if (RegisteredTypes.TryGetValue(type, out R3TypeRegistration registration))
                    return registration;
            }

            return null;
        }
    }
}