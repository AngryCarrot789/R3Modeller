using System;

namespace R3Modeller.Core.Engine.ViewModels {
    public class RenderViewportViewModel : BaseViewModel {
        public RenderViewport Model { get; }

        public CameraViewModel Camera { get; }

        public EditorViewModel Editor { get; }

        public RenderViewportViewModel(RenderViewport model, EditorViewModel editor) {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.Editor = editor ?? throw new ArgumentNullException(nameof(editor));
            this.Camera = new CameraViewModel(model.Camera, this);
        }

        public void InvalidateRender(bool schedule = true) {
            this.Model.RenderTarget.InvalidateRender(schedule);
        }

        /// <summary>
        /// Calculate a list of objects whose bounding boxes are visible on screen based on the current state of the camera
        /// </summary>
        public void RecalculateVisibleObjects() {

        }
    }
}