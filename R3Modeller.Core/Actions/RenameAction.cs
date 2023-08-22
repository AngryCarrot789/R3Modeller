using System.Threading.Tasks;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Actions {
    [ActionRegistration("actions.general.RenameItem")]
    public class RenameAction : AnAction {
        public override async Task<bool> ExecuteAsync(AnActionEventArgs e) {
            if (!e.DataContext.TryGetContext(out IRenameTarget renameable))
                return false;
            await renameable.RenameAsync();
            return true;
        }
    }
}