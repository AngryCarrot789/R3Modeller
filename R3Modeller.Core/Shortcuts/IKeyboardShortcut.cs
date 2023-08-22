using System.Collections.Generic;
using R3Modeller.Core.Shortcuts.Inputs;
using R3Modeller.Core.Shortcuts.Usage;

namespace R3Modeller.Core.Shortcuts {
    /// <summary>
    /// An interface for shortcuts that accept keyboard inputs
    /// </summary>
    public interface IKeyboardShortcut : IShortcut {
        /// <summary>
        /// All of the Key Strokes that this shortcut contains
        /// </summary>
        IEnumerable<KeyStroke> KeyStrokes { get; }

        /// <summary>
        /// This can be used in order to track the usage of <see cref="IShortcut.InputStrokes"/>. If
        /// the list is empty, then the return value of this function is effectively pointless
        /// </summary>
        /// <returns></returns>
        IKeyboardShortcutUsage CreateKeyUsage();
    }
}