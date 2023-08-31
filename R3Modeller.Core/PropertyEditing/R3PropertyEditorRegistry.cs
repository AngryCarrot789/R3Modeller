using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.PropertyEditing.Editors.Primitives;
using R3Modeller.Core.PropertyEditing.Editors.Scenes;

namespace R3Modeller.Core.PropertyEditing {
    public class R3PropertyEditorRegistry : PropertyEditorRegistry {
        public static R3PropertyEditorRegistry Instance { get; } = new R3PropertyEditorRegistry();

        private R3PropertyEditorRegistry() {
            // scene object

            PropertyGroupViewModel typeGroup = this.CreateRootGroup(typeof(SceneObjectViewModel), "Object Data");

            PropertyGroupViewModel basicData = typeGroup.CreateSubGroup(typeof(SceneObjectViewModel), "Basic Data");
            basicData.AddPropertyEditor("Visibility", CheckBoxEditorViewModel.ForGeneric<SceneObjectViewModel>("Is Visible", x => x.IsVisible, (x, v) => x.IsVisible = v));

            // basicData.AddPropertyEditor("Visibility", new CheckBoxEditorViewModel("Is Visible", typeof(SceneObjectViewModel), x => ((SceneObjectViewModel) x).IsVisible, (x, v) => ((SceneObjectViewModel) x).IsVisible = v));

            PropertyGroupViewModel transformation = typeGroup.CreateSubGroup(typeof(SceneObjectViewModel), "Transformation");
            // probably shouldn't really use stuff like this, as a huge amount of UI control is stripped.
            // A specific view model and data template for controlling stuff
            // transformation.AddPropertyEditor("Absolute Coords Grid", new CheckBoxGridEditorViewModel(typeof(SceneObjectViewModel)) {
            //     CheckBoxEditorViewModel.ForGeneric<SceneObjectViewModel>("Abs Pos", (x) => x.IsPositionAbsolute, (x, v) => x.IsPositionAbsolute = v),
            //     CheckBoxEditorViewModel.ForGeneric<SceneObjectViewModel>("Abs Scale", (x) => x.IsScaleAbsolute, (x, v) => x.IsScaleAbsolute = v),
            //     CheckBoxEditorViewModel.ForGeneric<SceneObjectViewModel>("Abs Rotate", (x) => x.IsRotationAbsolute, (x, v) => x.IsRotationAbsolute = v),
            // });
            transformation.AddPropertyEditor("Transformation", new TransformationEditorViewModel(typeof(SceneObjectViewModel)));

            // only for testing the UI and the applicability calculators.

            // passing typeof(SceneObjectViewModel) to the transformation editor is not necessary but it
            // makes debugging the code easier as I can fake higher applicable types

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
}