using System;
using System.Collections;
using System.IO;
using System.Numerics;
using System.Reflection;
using R3Modeller.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ObjectLoader.Data.Elements;
using ObjectLoader.Loaders;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Core;
using R3Modeller.Core.Engine;
using R3Modeller.Core.Engine.Objs;
using R3Modeller.Core.Engine.Utils;
using R3Modeller.Core.Engine.ViewModels;
using R3Modeller.Core.Notifications;
using R3Modeller.Core.Utils;
using R3Modeller.Notifications;
using R3Modeller.Themes;
using R3Modeller.Utils;
using R3Modeller.Views;
using Vector3 = System.Numerics.Vector3;
using WindowState = System.Windows.WindowState;

namespace R3Modeller {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx, INotificationHandler {
        private readonly Project project;

        public static readonly DependencyProperty PropertyPageItemsSourceProperty = DependencyProperty.Register("PropertyPageItemsSource", typeof(IEnumerable), typeof(MainWindow), new PropertyMetadata(null));

        public IEnumerable PropertyPageItemsSource {
            get => (IEnumerable) this.GetValue(PropertyPageItemsSourceProperty);
            set => this.SetValue(PropertyPageItemsSourceProperty, value);
        }

        public EditorViewModel Editor { get; }

        private readonly NotificationPanelViewModel NotificationPanel;

        public MainWindow() {
            this.InitializeComponent();
            this.DataContext = this.Editor = new EditorViewModel(new RenderViewport(this.OGLViewPort), this);
            this.NotificationPanel = this.Editor.NotificationPanel;
            this.NotificationPanelPopup.Placement = PlacementMode.Absolute;
            this.NotificationPanelPopup.PlacementTarget = this;
            this.NotificationPanelPopup.PlacementRectangle = System.Windows.Rect.Empty;
            this.NotificationPanelPopup.DataContext = this.NotificationPanel;
            this.RefreshPopupLocation();

            this.OGLViewPort.BeginFrame();
            this.project = new Project();
            this.Editor.MainViewport.Model.Camera.SetYawPitch(0.45f, -0.35f);
            this.project.Scene.Root.AddItem(new TriangleObject());
            TriangleObject tri = new TriangleObject();
            tri.RelativePosition = new Vector3(3f, 2f, 3f);
            this.project.Scene.Root.Items[0].AddItem(tri);

            this.project.Scene.Root.AddItem(new FloorPlaneObject());

            ObjLoaderFactory factory = new ObjLoaderFactory();
            IObjLoader loader = factory.Create(new MaterialStreamProvider(ResourceLocator.ResourceDirectory));
            using (FileStream stream = File.OpenRead(ResourceLocator.GetResourceFile("untitled.obj"))) {
                LoadResult result = loader.Load(stream);
                SceneObject objFile = new SceneObject() {
                    DisplayName = "untitled.obj"
                };

                int i = 0;
                foreach (Group group in result.Groups) {
                    i++;
                    objFile.AddItem(new WavefrontObject(result, group) {DisplayName = group.Name ?? $"Group {i}"});
                }

                this.project.Scene.Root.AddItem(objFile);
            }

            this.OGLViewPort.EndFrame();

            this.Editor.SetProject(new ProjectViewModel(this.project));
            // this.Loaded += this.OnLoaded;
        }

        // private void OnLoaded(object sender, RoutedEventArgs e) {
        //     WindowInteropHelper interop = new WindowInteropHelper(this);
        //     HwndSource source = HwndSource.FromHwnd(interop.Handle);
        //     source.AddHook(this.WndProc);
        // }
        // 
        // private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled) {
        //     byte[] lpb = new byte[256];
        //     int dwSize = 256;
        // 
        //     if (msg == 255) {
        // 
        //     }
        //     return IntPtr.Zero;
        // }

        #region Notification panel

        public void OnNotificationPushed(NotificationViewModel notification) {
            if (!this.NotificationPanelPopup.IsOpen)
                this.NotificationPanelPopup.IsOpen = true;
            this.Dispatcher.InvokeAsync(this.RefreshPopupLocation, DispatcherPriority.Render);
        }

        public void OnNotificationRemoved(NotificationViewModel notification) {
            if (this.NotificationPanel.Notifications.Count < 1) {
                this.NotificationPanelPopup.IsOpen = false;
            }

            this.Dispatcher.InvokeAsync(this.RefreshPopupLocation, DispatcherPriority.Loaded);
        }

        public void BeginNotificationFadeOutAnimation(NotificationViewModel notification, Action<NotificationViewModel, bool> onCompleteCallback = null) {
            NotificationList list = VisualTreeUtils.FindVisualChild<NotificationList>(this.NotificationPanelPopup.Child, true);
            if (list == null) {
                return;
            }

            int index = (notification.Panel ?? this.NotificationPanel).Notifications.IndexOf(notification);
            if (index == -1) {
                throw new Exception("Item not present in panel");
            }

            if (!(list.ItemContainerGenerator.ContainerFromIndex(index) is NotificationControl control)) {
                return;
            }

            DoubleAnimation animation = new DoubleAnimation(1d, 0d, TimeSpan.FromSeconds(2), FillBehavior.Stop);
            animation.Completed += (sender, args) => {
                onCompleteCallback?.Invoke(notification, BaseViewModel.GetInternalData<bool>(notification, "IsCancelled"));
            };

            control.BeginAnimation(OpacityProperty, animation);
        }

        public void CancelNotificationFadeOutAnimation(NotificationViewModel notification) {
            if (BaseViewModel.GetInternalData<bool>(notification, "IsCancelled")) {
                return;
            }

            NotificationList list = VisualTreeUtils.FindVisualChild<NotificationList>(this.NotificationPanelPopup.Child, true);
            if (list == null) {
                return;
            }

            int index = (notification.Panel ?? this.NotificationPanel).Notifications.IndexOf(notification);
            if (index == -1) {
                throw new Exception("Item not present in panel");
            }

            if (!(list.ItemContainerGenerator.ContainerFromIndex(index) is NotificationControl control)) {
                return;
            }

            BaseViewModel.SetInternalData(notification, "IsCancelled", BoolBox.True);
            control.BeginAnimation(OpacityProperty, null);
        }

        protected override void OnLocationChanged(EventArgs e) {
            base.OnLocationChanged(e);
            this.RefreshPopupLocation();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
            base.OnRenderSizeChanged(sizeInfo);
            if (this.WindowState == WindowState.Maximized) {
                this.Dispatcher.InvokeAsync(this.RefreshPopupLocation, DispatcherPriority.ApplicationIdle);
            }
            else {
                this.RefreshPopupLocation();
            }

            this.OGLViewPort.InvalidateRender();
        }

        private static readonly FieldInfo actualTopField = typeof(Window).GetField("_actualTop", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        private static readonly FieldInfo actualLeftField = typeof(Window).GetField("_actualLeft", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

        public void RefreshPopupLocation() {
            Popup popup = this.NotificationPanelPopup;
            if (popup == null || !popup.IsOpen) {
                return;
            }

            if (!(popup.Child is FrameworkElement element)) {
                return;
            }

            // winpos = X 1663
            // popup pos = X 1620
            // popup wid = 300

            switch (this.WindowState) {
                case WindowState.Normal: {
                    popup.Visibility = Visibility.Visible;
                    popup.VerticalOffset = this.Top + this.ActualHeight - element.ActualHeight;
                    popup.HorizontalOffset = this.Left + this.ActualWidth - element.ActualWidth;
                    break;
                }
                case WindowState.Minimized: {
                    popup.Visibility = Visibility.Collapsed;
                    break;
                }
                case WindowState.Maximized: {
                    popup.Visibility = Visibility.Visible;
                    Thickness thicc = this.BorderThickness;
                    double top = (double) actualTopField.GetValue(this) + thicc.Top;
                    double left = (double) actualLeftField.GetValue(this) + thicc.Left;
                    popup.VerticalOffset = top - (thicc.Top + thicc.Bottom) + this.ActualHeight - element.ActualHeight;
                    popup.HorizontalOffset = left - (thicc.Left + thicc.Right) + this.ActualWidth - element.ActualWidth;
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        private void SetThemeClick(object sender, RoutedEventArgs e) {
            ThemeType type;
            switch (((MenuItem) sender).Uid) {
                case "0": type = ThemeType.DeepDark; break;
                case "1": type = ThemeType.SoftDark; break;
                case "2": type = ThemeType.DarkGreyTheme; break;
                case "3": type = ThemeType.GreyTheme; break;
                case "4": type = ThemeType.RedBlackTheme; break;
                case "5": type = ThemeType.LightTheme; break;
                default: return;
            }

            ThemesController.SetTheme(type);
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            this.OGLViewPort.InvalidateRender();
        }

        protected override void OnDeactivated(EventArgs e) {
            base.OnDeactivated(e);
            this.OGLViewPort.InvalidateRender();
        }
    }
}