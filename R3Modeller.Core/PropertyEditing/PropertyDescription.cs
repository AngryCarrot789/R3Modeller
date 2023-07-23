using System;

namespace R3Modeller.Core.PropertyEditing {
    /// <summary>
    /// A class which describes a single property
    /// </summary>
    public class PropertyDescription {
        /// <summary>
        /// The property name. This is what will be passed to <see cref="IPropertyEditReceiver.OnExternalPropertyModified"/>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of the property, e.g. <see cref="double"/>
        /// </summary>
        public Type Type { get; }

        public PropertyDescription(string name, Type type) {
            this.Name = name;
            this.Type = type;
        }
    }
}