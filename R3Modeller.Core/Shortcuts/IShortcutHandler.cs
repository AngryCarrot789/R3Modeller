using System.Threading.Tasks;
using R3Modeller.Core.Shortcuts.Managing;

namespace R3Modeller.Core.Shortcuts {
    public interface IShortcutHandler {
        Task<bool> OnShortcutActivated(ShortcutProcessor processor, GroupedShortcut shortcut);
    }
}