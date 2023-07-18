using System.Threading.Tasks;
using R3Modeller.Core.Actions;
using R3Modeller.Core.History.ViewModels;

namespace R3Modeller.Core.History.Actions {
    public class UndoAction : AnAction {
        public override async Task<bool> ExecuteAsync(AnActionEventArgs e) {
            if (IoC.HistoryManager is HistoryManagerViewModel manager) {
                if (manager.CanUndo) {
                    await manager.UndoAction();
                }

                return true;
            }
            else {
                return false;
            }
        }
    }
}