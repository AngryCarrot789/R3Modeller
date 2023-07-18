namespace R3Modeller.Core.Shortcuts.Dialogs {
    public interface IShortcutManagerDialogService {
        bool IsOpen { get; }

        void ShowEditorDialog();
    }
}