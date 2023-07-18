using System.Threading.Tasks;
using R3Modeller.Core.Actions;
using R3Modeller.Core.History.ViewModels;

namespace R3Modeller.Core.History.Actions {
    public class RedoAction : AnAction {
        public override async Task<bool> ExecuteAsync(AnActionEventArgs e) {
            if (IoC.HistoryManager is HistoryManagerViewModel manager) {
                if (manager.CanRedo) {
                    await manager.RedoAction();
                }

                return true;
            }
            else {
                return false;
            }
        }
    }
}