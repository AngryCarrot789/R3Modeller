using System.Threading.Tasks;
using R3Modeller.Core.Views.Windows;

namespace R3Modeller.CrossPlatform.Views {
    public class BaseWindow : WindowViewBase, IWindow {
        public bool IsOpen => base.IsLoaded;

        public BaseWindow() {
            this.SetToCenterOfScreen();
        }

        public void CloseWindow() {
            this.Close();
        }

        public Task CloseWindowAsync() {
            return base.CloseAsync();
        }
    }
}