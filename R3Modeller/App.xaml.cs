using System;
using System.IO;
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
using R3Modeller.Utils;
using R3Modeller.Views;

namespace R3Modeller {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private AppSplashScreen splash;

        public App() {
        }

        public void RegisterActions() {
            // ActionManager.SearchAndRegisterActions(ActionManager.Instance);
            // TODO: Maybe use an XML file to store this, similar to how intellij registers actions?
            // ActionManager.Instance.Register("actions.project.Open", new OpenProjectAction());
            // ActionManager.Instance.Register("actions.project.Save", new SaveProjectAction());
            // ActionManager.Instance.Register("actions.project.SaveAs", new SaveProjectAsAction());
            ActionManager.Instance.Register("actions.project.history.Undo", new UndoAction());
            ActionManager.Instance.Register("actions.project.history.Redo", new RedoAction());
            ActionManager.Instance.Register("actions.objlist.RenameItem", new RenameObjectAction());
            // ActionManager.Instance.Register("actions.automation.AddKeyFrame", new AddKeyFrameAction());
            // ActionManager.Instance.Register("actions.editor.timeline.TogglePlayPause", new TogglePlayPauseAction());
            // ActionManager.Instance.Register("actions.resources.DeleteItems", new DeleteResourcesAction());
            // ActionManager.Instance.Register("actions.resources.GroupSelection", new GroupSelectedResourcesAction());
            // ActionManager.Instance.Register("actions.resources.RenameItem", new RenameResourceAction());
            // ActionManager.Instance.Register("actions.resources.ToggleOnlineState", new ToggleResourceOnlineStateAction());
            // ActionManager.Instance.Register("actions.editor.timeline.DeleteSelectedClips", new DeleteSelectedClips());
            // ActionManager.Instance.Register("actions.editor.NewVideoTrack", new NewVideoTrackAction());
            // ActionManager.Instance.Register("actions.editor.NewAudioTrack", new NewAudioTrackAction());
            // ActionManager.Instance.Register("actions.editor.timeline.SliceClips", new SliceClipsAction());
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

            ResourceLocator.Setup();

            IoC.Dispatcher = new DispatcherDelegate(this.Dispatcher);
            IoC.OnShortcutModified = (x) => {
                if (!string.IsNullOrWhiteSpace(x)) {
                    ShortcutManager.Instance.InvalidateShortcutCache();
                    GlobalUpdateShortcutGestureConverter.BroadcastChange();
                }
            };

            IoC.LoadServicesFromAttributes();

            await this.SetActivity("Loading shortcut and action managers...");
            ShortcutManager.Instance = new WPFShortcutManager();
            ActionManager.Instance = new ActionManager();
            InputStrokeViewModel.KeyToReadableString = KeyStrokeStringConverter.ToStringFunction;
            InputStrokeViewModel.MouseToReadableString = MouseStrokeStringConverter.ToStringFunction;

            this.RegisterActions();

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

#if false
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.MainWindow = new PropertyPageDemoWindow();
            this.MainWindow.Show();
#else
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
                    await IoC.MessageDialogs.ShowMessageExAsync("App init failed", "Failed to start FramePFX", ex.GetToString());
                }
                else {
                    MessageBox.Show("Failed to start FramePFX:\n\n" + ex, "Fatal App init failure");
                }

                this.Dispatcher.Invoke(() => {
                    this.Shutdown(0);
                }, DispatcherPriority.Background);
                return;
            }
#endif

            await this.SetActivity("Loading FramePFX main window...");
            MainWindow window = new MainWindow();
            this.splash.Close();
            this.MainWindow = window;
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            window.Show();
            await this.Dispatcher.Invoke(async () => {
                await this.OnVideoEditorLoaded(window);
            }, DispatcherPriority.Loaded);
#endif
        }

        public async Task OnVideoEditorLoaded(MainWindow editor) {
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
        }
    }
}