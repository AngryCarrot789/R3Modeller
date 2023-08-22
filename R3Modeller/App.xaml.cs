using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using OpenTK.Graphics;
using R3Modeller.Core;
using R3Modeller.Core.Actions;
using R3Modeller.Core.Engine.Objs.Actions;
using R3Modeller.Core.History.Actions;
using R3Modeller.Core.Shortcuts.Managing;
using R3Modeller.Core.Shortcuts.ViewModels;
using R3Modeller.Core.Utils;
using R3Modeller.Shortcuts;
using R3Modeller.Shortcuts.Converters;
using R3Modeller.Themes;
using R3Modeller.Utils;
using R3Modeller.Views;

namespace R3Modeller {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private AppSplashScreen splash;

        public static ThemeType CurrentTheme { get; set; }

        public static ResourceDictionary ThemeDictionary {
            get => Current.Resources.MergedDictionaries[0];
            set => Current.Resources.MergedDictionaries[0] = value;
        }

        public static ResourceDictionary ControlColours {
            get => Current.Resources.MergedDictionaries[1];
            set => Current.Resources.MergedDictionaries[1] = value;
        }

        public static ResourceDictionary I18NText {
            get => Current.Resources.MergedDictionaries[3];
            set => Current.Resources.MergedDictionaries[3] = value;
        }

        public static void RefreshControlsDictionary() {
            ResourceDictionary resource = Current.Resources.MergedDictionaries[2];
            Current.Resources.MergedDictionaries.RemoveAt(2);
            Current.Resources.MergedDictionaries.Insert(2, resource);
        }

        public App() {
        }

        public void RegisterActions() {
            // ActionManager.SearchAndRegisterActions(ActionManager.Instance);
            // TODO: Maybe use an XML file to store this, similar to how intellij registers actions?
            ActionManager.Instance.Register("actions.project.history.Undo", new UndoAction());
            ActionManager.Instance.Register("actions.project.history.Redo", new RedoAction());
            ActionManager.Instance.Register("actions.objlist.RenameItem", new RenameObjectAction());
        }

        private async Task SetActivity(string activity) {
            this.splash.CurrentActivity = activity;
            await this.Dispatcher.InvokeAsync(() => {
            }, DispatcherPriority.ApplicationIdle);
        }

        public async Task InitApp() {
            await this.SetActivity("Loading services...");
                        string[] envArgs = Environment.GetCommandLineArgs();
            if (envArgs.Length > 0 && Path.GetDirectoryName(envArgs[0]) is string dir && dir.Length > 0) {
                Directory.SetCurrentDirectory(dir);
            }

            IoC.Dispatcher = new DispatcherDelegate(this.Dispatcher);
            IoC.OnShortcutModified = (x) => {
                if (!string.IsNullOrWhiteSpace(x)) {
                    ShortcutManager.Instance.InvalidateShortcutCache();
                    GlobalUpdateShortcutGestureConverter.BroadcastChange();
                }
            };
            IoC.BroadcastShortcutActivity = (x) => {
            };

            List<(TypeInfo, ServiceImplementationAttribute)> serviceAttributes = new List<(TypeInfo, ServiceImplementationAttribute)>();
            List<(TypeInfo, ActionRegistrationAttribute)> attributes = new List<(TypeInfo, ActionRegistrationAttribute)>();

            // Process all attributes in a single scan, instead of multiple scans for services, actions, etc
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (TypeInfo typeInfo in assembly.DefinedTypes) {
                    ServiceImplementationAttribute serviceAttribute = typeInfo.GetCustomAttribute<ServiceImplementationAttribute>();
                    if (serviceAttribute?.Type != null) {
                        serviceAttributes.Add((typeInfo, serviceAttribute));
                    }

                    ActionRegistrationAttribute actionAttribute = typeInfo.GetCustomAttribute<ActionRegistrationAttribute>();
                    if (actionAttribute != null) {
                        attributes.Add((typeInfo, actionAttribute));
                    }
                }
            }

            foreach ((TypeInfo, ServiceImplementationAttribute) tuple in serviceAttributes) {
                object instance;
                try {
                    instance = Activator.CreateInstance(tuple.Item1);
                }
                catch (Exception e) {
                    throw new Exception($"Failed to create implementation of {tuple.Item2.Type} as {tuple.Item1}", e);
                }

                IoC.Instance.Register(tuple.Item2.Type, instance);
            }

            await this.SetActivity("Loading shortcut and action managers...");
            ShortcutManager.Instance = new WPFShortcutManager();
            ActionManager.Instance = new ActionManager();
            InputStrokeViewModel.KeyToReadableString = KeyStrokeStringConverter.ToStringFunction;
            InputStrokeViewModel.MouseToReadableString = MouseStrokeStringConverter.ToStringFunction;

            this.RegisterActions();

            foreach ((TypeInfo type, ActionRegistrationAttribute attribute) in attributes.OrderBy(x => x.Item2.RegistrationOrder)) {
                AnAction action;
                try {
                    action = (AnAction) Activator.CreateInstance(type, true);
                }
                catch (Exception e) {
                    throw new Exception($"Failed to create an instance of the registered action '{type.FullName}'", e);
                }

                if (attribute.OverrideExisting && ActionManager.Instance.GetAction(attribute.ActionId) != null) {
                    ActionManager.Instance.Unregister(attribute.ActionId);
                }

                ActionManager.Instance.Register(attribute.ActionId, action);
            }

            await this.SetActivity("Loading keymap...");
            string keymapFilePath = Path.GetFullPath(@"Keymap.xml");
            if (File.Exists(keymapFilePath)) {
                using (FileStream stream = File.OpenRead(keymapFilePath)) {
                    ShortcutGroup group = WPFKeyMapSerialiser.Instance.Deserialise(stream);
                    WPFShortcutManager.WPFInstance.SetRoot(group);
                }
            }
            else {
                await IoC.MessageDialogs.ShowMessageAsync("No keymap available", "Keymap file does not exist: " + keymapFilePath + $".\nCurrent directory: {Directory.GetCurrentDirectory()}\nCommand line args:{string.Join("\n", Environment.GetCommandLineArgs())}");
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e) {
            // Dialogs may be shown, becoming the main window, possibly causing the
            // app to shutdown when the mode is OnMainWindowClose or OnLastWindowClose

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.MainWindow = this.splash = new AppSplashScreen();
            this.splash.Show();

#if !DEBUG // allow debug mode to catch the exception
            try {
#endif
                await this.InitApp();
#if !DEBUG
            }
            catch (Exception ex) {
                if (IoC.MessageDialogs != null) {
                    await IoC.MessageDialogs.ShowMessageExAsync("App init failed", "Failed to start R3Modeller", ex.GetToString());
                }
                else {
                    MessageBox.Show("Failed to start R3Modeller:\n\n" + ex, "Fatal App init failure");
                }

                this.Dispatcher.Invoke(() => {
                    this.Shutdown(0);
                }, DispatcherPriority.Background);
                return;
            }
#endif

            await this.SetActivity("Loading R3Modeller main window...");
            MainWindow window = new MainWindow();
            this.splash.Close();
            this.MainWindow = window;
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
        }
    }
}