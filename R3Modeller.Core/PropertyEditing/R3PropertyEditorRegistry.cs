using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.PropertyEditing.Editors.Scenes;

namespace R3Modeller.Core.PropertyEditing {
    public class R3PropertyEditorRegistry : PropertyEditorRegistry {
        public static R3PropertyEditorRegistry Instance { get; } = new R3PropertyEditorRegistry();

        /// <summary>
        /// The root group container for this registry. This group by itself is invalid
        /// and should never be used apart from storing child objects
        /// </summary>
        public PropertyGroupViewModel Root { get; }

        private R3PropertyEditorRegistry() {
            this.Root = new PropertyGroupViewModel(null, "<root>");

            // scene object
            {
                PropertyGroupViewModel typeGroup = this.RegisterRoot(typeof(SceneObjectViewModel), "Scene Object");
                typeGroup.AddPropertyEditor("Transformation", new TransformationEditorViewModel(typeof(SceneObjectViewModel)));

                // only for testing the UI and also the applicability calculators.
                // passing typeof(SceneObjectViewModel) to the transformation editor is not nessesary but it
                // makes debugging the code easier as I can fake a higher applicable type
                // PropertyGroupViewModel group1 = typeGroup.CreateSubGroup(typeof(SceneObjectViewModel), "Group 1");
                // group1.AddPropertyEditor("Position 1", new TransformationEditorViewModel(typeof(SceneObjectViewModel)));
                // group1.AddPropertyEditor("Floor Position 1", new TransformationEditorViewModel(typeof(FloorPlaneObjectViewModel)));
                // PropertyGroupViewModel group2 = group1.CreateSubGroup(typeof(SceneObjectViewModel), "Group 2");
                // group2.AddPropertyEditor("Position 2", new TransformationEditorViewModel(typeof(SceneObjectViewModel)));
                // group2.AddPropertyEditor("Floor Position 2", new TransformationEditorViewModel(typeof(FloorPlaneObjectViewModel)));
                // PropertyGroupViewModel group3 = group2.CreateSubGroup(typeof(SceneObjectViewModel), "Group 3");
                // group3.AddPropertyEditor("Sexy Position", new TransformationEditorViewModel(typeof(SceneObjectViewModel)));
            }
        }

        private PropertyGroupViewModel RegisterRoot(Type type, string name, bool isExpandedByDefault = true) {
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