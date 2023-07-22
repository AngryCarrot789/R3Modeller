using System.Threading.Tasks;
using System.Windows.Input;
using R3Modeller.Core.Views.Dialogs;

namespace R3Modeller.CrossPlatform.Views {
    public class BaseDialog : WindowViewBase, IDialog {
        public BaseDialog() {
            this.SetToCenterOfScreen();
            this.DataContextChanged += (sender, args) => {
                if (args.NewValue is BaseDialogViewModel vm) {
                    vm.Dialog = this;
                }
            };
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (!e.Handled) {
                switch (e.Key) {
                    case Key.Escape:
                        this.DialogResult = false;
                        break;
                    default:
                        return;
                }

                e.Handled = true;
                this.Close();
            }
        }

        public void CloseDialog(bool result) {
            this.DialogResult = result;
            this.Close();
        }

        public Task CloseDialogAsync(bool result) {
            this.DialogResult = result;
            return this.CloseAsync();
        }
    }
}