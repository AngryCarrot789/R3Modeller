using R3Modeller.Core.Rendering;

namespace R3Modeller.Core.Engine {
    public class RenderViewport {
        public IRenderTarget RenderTarget { get; }

        public Camera Camera { get; }

        public RenderViewport(IRenderTarget renderTarget) {
            this.RenderTarget = renderTarget;
            this.Camera = new Camera();
        }
    }
}