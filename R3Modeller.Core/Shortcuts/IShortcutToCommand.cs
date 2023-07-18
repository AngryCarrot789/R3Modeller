using System.Windows.Input;

namespace R3Modeller.Core.Shortcuts {
    public interface IShortcutToCommand {
        ICommand GetCommandForShortcut(string shortcutId);
    }
}