using System.Threading.Tasks;
using R3Modeller.Core;
using R3Modeller.Core.Views.Dialogs.Progression;

namespace R3Modeller.CrossPlatform.Views.Progression {
    [ServiceImplementation(typeof(IProgressionDialogService))]
    public class ProgressionDialogService : IProgressionDialogService {
        public Task ShowIndeterminateAsync(IndeterminateProgressViewModel viewModel) {
            IndeterminateProgressionWindow window = new IndeterminateProgressionWindow();
            viewModel.Window = window;
            window.DataContext = viewModel;
            window.Show();
            return Task.CompletedTask;
        }
    }
}