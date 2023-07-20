using R3Modeller.Core.Rendering;

namespace R3Modeller.Core.Engine.ViewModels {
    public class ProjectViewModel : BaseViewModel {
        public IRenderTarget RenderTarget { get; }

        public SceneViewModel Scene { get; }

        public Project Model { get; set; }

        public EditorViewModel Editor { get; set; }

        public ProjectViewModel(IRenderTarget renderTarget, Project project) {
            this.RenderTarget = renderTarget;
            this.Model = project;
            this.Scene = new SceneViewModel(project.Scene);
        }

        public void OnRenderInvalidated() {
            this.RenderTarget.InvalidateRender(true);
        }
    }
}