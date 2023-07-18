using System.Threading.Tasks;

namespace R3Modeller.Core.Views.Dialogs.Progression {
    public interface IProgressionDialogService {
        Task ShowIndeterminateAsync(IndeterminateProgressViewModel viewModel);
    }
}