using R3Modeller.Core;
using R3Modeller.Core.Shortcuts.Dialogs;
using R3Modeller.Core.Shortcuts.Inputs;

namespace R3Modeller.CrossPlatform.Shortcuts.Dialogs {
    [ServiceImplementation(typeof(IKeyboardDialogService))]
    public class KeyboardDialogService : IKeyboardDialogService {
        public KeyStroke? ShowGetKeyStrokeDialog() {
            KeyStrokeInputWindow window = new KeyStrokeInputWindow();
            if (window.ShowDialog() != true || window.Stroke.Equals(default)) {
                return null;
            }
            else {
                return window.Stroke;
            }
        }
    }
}