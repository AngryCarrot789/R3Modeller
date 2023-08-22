using System.Windows.Input;
using R3Modeller.Core.Shortcuts.Inputs;
using R3Modeller.Core.Views.Dialogs;
using R3Modeller.Shortcuts.Converters;
using R3Modeller.Views;

namespace R3Modeller.Shortcuts.Dialogs {
    /// <summary>
    /// Interaction logic for MouseStrokeInputWindow.xaml
    /// </summary>
    public partial class MouseStrokeInputWindow : BaseDialog {
        public MouseStroke Stroke { get; set; }

        public MouseStrokeInputWindow() {
            this.InitializeComponent();
            this.DataContext = new BaseConfirmableDialogViewModel(this);
            this.InputBox.Text = "";
        }

        private void InputBox_MouseDown(object sender, MouseButtonEventArgs e) {
            MouseStroke stroke = ShortcutUtils.GetMouseStrokeForEvent(e);
            this.Stroke = stroke;
            this.InputBox.Text = MouseStrokeStringConverter.ToStringFunction(stroke.MouseButton, stroke.Modifiers, stroke.ClickCount);
        }

        private void InputBox_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (ShortcutUtils.GetMouseStrokeForEvent(e, out MouseStroke stroke)) {
                this.Stroke = stroke;
                this.InputBox.Text = MouseStrokeStringConverter.ToStringFunction(stroke.MouseButton, stroke.Modifiers, stroke.ClickCount);
            }
        }
    }
}