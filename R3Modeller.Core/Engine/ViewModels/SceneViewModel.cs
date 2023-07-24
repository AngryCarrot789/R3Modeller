using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.Utils;
using System.Collections.ObjectModel;

namespace R3Modeller.Core.Engine.ViewModels {
    public class SceneViewModel : BaseViewModel {
        /// <summary>
        /// A collection of root scene objects
        /// </summary>
        public SceneObjectViewModel RootObject { get; }

        public SceneGraph Model { get; set; }

        public ProjectViewModel Project { get; }

        public ObservableCollection<SceneObjectViewModel> SelectedItems { get; } = new ObservableCollection<SceneObjectViewModel>();

        // TODO: figure out another way to store the text "n objects selected" as
        // converters don't really work well with observable collections
        public string PropertyEditorText { get; set; }

        public SceneViewModel(SceneGraph model, ProjectViewModel project) {
            this.Model = model;
            this.Project = project;
            this.RootObject = new SceneObjectViewModel(model.Root);
            this.RootObject.SetProject(project);

            this.SelectedItems.CollectionChanged += (sender, args) => {
                this.PropertyEditorText = $"{this.SelectedItems.Count} item{Lang.S(this.SelectedItems.Count)} selected";
                this.RaisePropertyChanged(nameof(this.PropertyEditorText));
            };
        }
    }
}