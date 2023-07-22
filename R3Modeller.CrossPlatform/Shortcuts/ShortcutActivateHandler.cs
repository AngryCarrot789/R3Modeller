using System.Threading.Tasks;
using R3Modeller.Core.Shortcuts.Managing;

namespace R3Modeller.CrossPlatform.Shortcuts {
    public delegate Task<bool> ShortcutActivateHandler(ShortcutProcessor processor, GroupedShortcut shortcut);
}