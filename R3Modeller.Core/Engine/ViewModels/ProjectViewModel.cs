namespace R3Modeller.Core.Engine.ViewModels {
    public class ProjectViewModel : BaseViewModel {
        public SceneViewModel Scene { get; }

        public Project Model { get; set; }

        public EditorViewModel Editor { get; set; }

        public ProjectViewModel(Project project) {
            this.Model = project;
            this.Scene = new SceneViewModel(project.Scene, this);
        }

        public void OnRenderInvalidated() {
            this.Editor.InvalidateAllViewports(true);
        }
    }
}