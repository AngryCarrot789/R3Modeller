using System.Threading.Tasks;

namespace R3Modeller.Core.Views.Windows {
    public interface IWindow : IViewBase {
        void CloseWindow();

        Task CloseWindowAsync();

        bool IsOpen { get; }
    }
}