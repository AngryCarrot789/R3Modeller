using R3Modeller.Core.Engine.Objs.ViewModels;

namespace R3Modeller.Core.Engine.ViewModels {
    public class SceneViewModel : BaseViewModel {
        /// <summary>
        /// A collection of root scene objects
        /// </summary>
        public SceneObjectViewModel RootObject { get; }

        public SceneGraph Model { get; set; }

        public ProjectViewModel Project { get; }

        public SceneViewModel(SceneGraph model, ProjectViewModel project) {
            this.Model = model;
            this.Project = project;
            this.RootObject = new SceneObjectViewModel(model.Root);
            this.RootObject.SetProject(project);
        }
    }
}