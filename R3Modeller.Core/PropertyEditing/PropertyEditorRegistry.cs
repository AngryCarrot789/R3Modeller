using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace R3Modeller.Core.PropertyEditing {
    /// <summary>
    /// A register for mapping a data source to a collection of properties that can be modified
    /// </summary>
    public class PropertyEditorRegistry {
        /// <summary>
        /// The root group container for this registry. This group by itself is invalid
        /// and should never be used apart from storing child objects
        /// </summary>
        public PropertyGroupViewModel Root { get; }

        public PropertyEditorRegistry() {
            this.Root = new PropertyGroupViewModel(null, "<root>");
        }

        /// <summary>
        /// Convenience function for creating a sub-group in our root group container
        /// </summary>
        protected PropertyGroupViewModel CreateRootGroup(Type type, string name, bool isExpandedByDefault = true) {
            return this.Root.CreateSubGroup(type, name, isExpandedByDefault);
        }

        public void SetupObjects(List<object> dataSources) {
            this.Root.ClearHandlersRecursive();
            if (dataSources.Count > 0) {
                this.Root.SetupHierarchyState(dataSources);
            }
        }
    }
}