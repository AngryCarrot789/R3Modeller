using System;
using R3Modeller.Core.Engine.Utils;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine.ViewModels {
    public class CameraViewModel : BaseViewModel {
        public Camera Model { get; }

        public float Fov {
            get => this.Model.fov;
            set {
                this.Model.SetFov(Maths.Clamp(value, 20f, 120f));
                this.RaisePropertyChanged(nameof(this.Fov));
                this.Viewport.InvalidateRender();
            }
        }

        public RenderViewportViewModel Viewport { get; }

        public CameraViewModel(Camera model, RenderViewportViewModel viewport) {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.Viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
        }
    }
}