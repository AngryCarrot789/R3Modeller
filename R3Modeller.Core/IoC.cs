using System;
using System.Collections.Generic;
using System.Reflection;
using R3Modeller.Core.Actions;
using R3Modeller.Core.History.ViewModels;
using R3Modeller.Core.Services;
using R3Modeller.Core.Shortcuts.Dialogs;
using R3Modeller.Core.Shortcuts.Managing;
using R3Modeller.Core.Views.Dialogs.FilePicking;
using R3Modeller.Core.Views.Dialogs.Message;
using R3Modeller.Core.Views.Dialogs.Progression;
using R3Modeller.Core.Views.Dialogs.UserInputs;

namespace R3Modeller.Core {
    public static class IoC {
        private static volatile bool isAppRunning = true;

        public static SimpleIoC Instance { get; } = new SimpleIoC();

        public static bool IsAppRunning {
            get => isAppRunning;
            set => isAppRunning = value;
        }

        public static ApplicationViewModel App { get; } = new ApplicationViewModel();

        public static ActionManager ActionManager { get; } = new ActionManager();
        public static ShortcutManager ShortcutManager { get; set; }
        public static IShortcutManagerDialogService ShortcutManagerDialog => Provide<IShortcutManagerDialogService>();
        
        public static Action<string> OnShortcutModified { get; set; }
        public static Action<string> BroadcastShortcutActivity { get; set; }

        /// <summary>
        /// The application dispatcher, used to execute actions on the main thread
        /// </summary>
        public static IApplicationDispatcher Dispatcher { get; set; }
        public static IClipboardService Clipboard => Provide<IClipboardService>();
        public static IMessageDialogService MessageDialogs => Provide<IMessageDialogService>();
        public static IProgressionDialogService ProgressionDialogs => Provide<IProgressionDialogService>();
        public static IFilePickDialogService FilePicker => Provide<IFilePickDialogService>();
        public static IUserInputDialogService UserInput => Provide<IUserInputDialogService>();
        public static IExplorerService ExplorerService => Provide<IExplorerService>();
        public static IKeyboardDialogService KeyboardDialogs => Provide<IKeyboardDialogService>();
        public static IMouseDialogService MouseDialogs => Provide<IMouseDialogService>();

        public static ITranslator Translator => Provide<ITranslator>();

        public static HistoryManagerViewModel HistoryManager => Provide<HistoryManagerViewModel>();

        public static Action<string> BroadcastShortcutChanged { get; set; }

        public static void LoadServicesFromAttributes() {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (TypeInfo type in assembly.DefinedTypes) {
                    ServiceImplementationAttribute attrib = type.GetCustomAttribute<ServiceImplementationAttribute>();
                    if (attrib != null && attrib.Type != null) {
                        object instance;
                        try {
                            instance = Activator.CreateInstance(type);
                        }
                        catch (Exception e) {
                            throw new Exception($"Failed to create implementation of {attrib.Type} as {type}", e);
                        }

                        Instance.Register(attrib.Type, instance);
                    }
                }
            }
        }

        public static T Provide<T>() => Instance.Provide<T>();
    }
}