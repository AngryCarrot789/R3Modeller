using R3Modeller.Core;
using R3Modeller.Core.Shortcuts.Dialogs;
using R3Modeller.Core.Shortcuts.Inputs;

namespace R3Modeller.CrossPlatform.Shortcuts.Dialogs {
    [ServiceImplementation(typeof(IMouseDialogService))]
    public class MouseDialogService : IMouseDialogService {
        public MouseStroke? ShowGetMouseStrokeDialog() {
            MouseStrokeInputWindow window = new MouseStrokeInputWindow();
            if (window.ShowDialog() != true || window.Stroke.Equals(default)) {
                return null;
            }
            else {
                return window.Stroke;
            }
        }
    }
}