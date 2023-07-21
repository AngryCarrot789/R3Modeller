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
        private readonly LineObject axisLineX;
        private readonly LineObject axisLineY;
        private readonly LineObject axisLineZ;

        private readonly LineObject targetPointLineX;
        private readonly LineObject targetPointLineY;
        private readonly LineObject targetPointLineZ;

        private readonly Project project;

        private bool isOrbitActive;
        private bool ignoreMouseMoveEvent;
        private Point? lastMouse;
        private Point? mousePosBeforeOrbitEnabled;

        public static readonly DependencyProperty PropertyPageItemsSourceProperty = DependencyProperty.Register("PropertyPageItemsSource", typeof(IEnumerable), typeof(MainWindow), new PropertyMetadata(null));

        public IEnumerable PropertyPageItemsSource {
            get => (IEnumerable) this.GetValue(PropertyPageItemsSourceProperty);
            set => this.SetValue(PropertyPageItemsSourceProperty, value);
        }

        public EditorViewModel Editor { get; }

        private readonly NotificationPanelViewModel NotificationPanel;

        public MainWindow() {
            this.InitializeComponent();
            this.DataContext = this.Editor = new EditorViewModel(new Core.Engine.RenderViewport(this.OGLViewPort), this);
            this.NotificationPanel = this.Editor.NotificationPanel;
            this.NotificationPanelPopup.Placement = PlacementMode.Absolute;
            this.NotificationPanelPopup.PlacementTarget = this;
            this.NotificationPanelPopup.PlacementRectangle = System.Windows.Rect.Empty;
            this.NotificationPanelPopup.DataContext = this.NotificationPanel;
            this.RefreshPopupLocation();

            this.project = new Project();
            this.OGLViewPort.BeginFrame();
            this.Editor.RenderViewport.Model.Camera.SetYawPitch(0.45f, -0.35f);
            this.project.Scene.Root.AddItem(new TriangleObject());
            TriangleObject tri = new TriangleObject();
            tri.SetPosition(new Vector3(3f, 2f, 3f));
            this.project.Scene.Root.Items[0].AddItem(tri);

            this.project.Scene.Root.AddItem(new FloorPlaneObject());
            this.axisLineX = new LineObject(new Vector3(), new Vector3(1f, 0f, 0f));
            this.axisLineY = new LineObject(new Vector3(), new Vector3(0f, 1f, 0f));
            this.axisLineZ = new LineObject(new Vector3(), new Vector3(0f, 0f, 1f));

            this.targetPointLineX = new LineObject(new Vector3(-1f, 0f, 0f), new Vector3(1f,  0f,  0f));
            this.targetPointLineY = new LineObject(new Vector3( 0f, 1f, 0f), new Vector3(0f, -1f,  0f));
            this.targetPointLineZ = new LineObject(new Vector3( 0f, 0f, 1f), new Vector3(0f,  0f, -1f));

            #region obj loader

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

            #endregion

            this.Editor.SetProject(new ProjectViewModel(this.project));
        }

        public void OnOrbitModeEnabled() {
            if (this.isOrbitActive)
                return;
            this.isOrbitActive = true;
            this.UpdateCursor();
        }

        public void OnOrbitModeDisabled() {
            if (!this.isOrbitActive)
                return;
            this.isOrbitActive = false;
            this.UpdateCursor();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            this.axisLineX.Dispose();
            this.axisLineY.Dispose();
            this.axisLineZ.Dispose();
            this.targetPointLineX.Dispose();
            this.targetPointLineY.Dispose();
            this.targetPointLineZ.Dispose();
        }

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

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
            switch (e.Key) {
                case Key.System: {
                    if (!e.IsRepeat && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt) {
                        this.mousePosBeforeOrbitEnabled = Mouse.GetPosition(this);
                        this.isOrbitActive = true;
                        this.UpdateCursor();
                        this.OGLViewPort.InvalidateRender();
                    }

                    break;
                }
                default: return;
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e) {
            base.OnPreviewKeyUp(e);
            switch (e.Key) {
                case Key.System: {
                    if (!e.IsRepeat && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt) {
                        this.isOrbitActive = false;
                        this.UpdateCursor();
                        this.OGLViewPort.InvalidateRender();
                    }

                    break;
                }
                default: return;
            }

            e.Handled = true;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
            base.OnPreviewMouseWheel(e);
            if (e.Delta == 0) {
                return;
            }

            CameraViewModel camera = this.Editor.RenderViewport.Camera;
            float oldRange = camera.OrbitRange;
            float newRange;
            if (oldRange < 0.01f) {
                newRange = Maths.Clamp(e.Delta > 0 ? (oldRange / 20f) : (oldRange * 20f), 0.0001f, 750f);
            }
            else {
                float multiplier = e.Delta > 0 ? (1f - 0.25f) : (1f + 0.25f);
                newRange = Maths.Clamp(oldRange * multiplier, 0.0001f, 750f);
            }

            if (Math.Abs(newRange - oldRange) > 0.00001f) {
                camera.OrbitRange = newRange;
                this.OGLViewPort.InvalidateRender();
            }
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            this.OGLViewPort.InvalidateRender();
        }

        protected override void OnDeactivated(EventArgs e) {
            base.OnDeactivated(e);
            this.OGLViewPort.InvalidateRender();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right) {
                this.Focus();
                this.CaptureMouse();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right) {
                if (this.mousePosBeforeOrbitEnabled is Point point) {
                    this.ignoreMouseMoveEvent = true;
                    Point p = this.PointToScreen(point);
                    CursorUtils.SetCursorPos((int) p.X, (int) p.Y);
                    this.ignoreMouseMoveEvent = false;
                    this.mousePosBeforeOrbitEnabled = null;
                }

                this.ReleaseMouseCapture();
            }
        }

        public void UpdateCursor() {
            if (this.isOrbitActive) {
                this.Cursor = Cursors.None;
            }
            else {
                this.ClearValue(CursorProperty);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (this.ignoreMouseMoveEvent) {
                return;
            }

            if (this.isOrbitActive && !Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt)) {
                this.isOrbitActive = false;
                this.UpdateCursor();
            }

            Point mpos = e.GetPosition(this); // use "this" instead of OGLViewPort as it's easier due to the scale being -1
            if (this.lastMouse is Point lastPos && this.isOrbitActive) {
                bool wrap = false;
                double wrapX = mpos.X;
                double wrapY = mpos.Y;
                if (mpos.X < 0) {
                    wrapX = this.ActualWidth;
                    wrap = true;
                }
                else if (mpos.X > this.ActualWidth) {
                    wrapX = 0;
                    wrap = true;
                }

                if (mpos.Y < 0) {
                    wrapY = this.ActualHeight;
                    wrap = true;
                }
                else if (mpos.Y > this.ActualHeight) {
                    wrapY = 0;
                    wrap = true;
                }

                if (wrap) {
                    this.ignoreMouseMoveEvent = true;
                    Point wp = new Point(wrapX, wrapY);
                    Point sp = this.PointToScreen(wp);
                    this.lastMouse = wp;
                    try {
                        CursorUtils.SetCursorPos((int) sp.X, (int) sp.Y);
                    }
                    finally {
                        this.ignoreMouseMoveEvent = false;
                    }

                    return;
                }

                float changeX = 1f + (float) Maths.Map(mpos.X - lastPos.X, 0d, this.ActualWidth, -1d, 1d);
                float changeY = 1f - (float) Maths.Map(mpos.Y - lastPos.Y, 0d, this.ActualHeight, 1d, -1d);

                Camera camera = this.Editor.RenderViewport.Model.Camera;
                const float sensitivity = 1.75f;
                const float epsilon = 0.00001f;
                if (e.LeftButton == MouseButtonState.Pressed) {
                    float yaw = camera.yaw;
                    float pitch = camera.pitch;
                    yaw -= (changeX * sensitivity);
                    if (yaw > Maths.PI) {
                        yaw = Maths.PI_NEG + epsilon;
                    }
                    else if (yaw < Maths.PI_NEG) {
                        yaw = Maths.PI - epsilon;
                    }

                    pitch -= (changeY * sensitivity);
                    if (pitch > Maths.PI_HALF) {
                        pitch = Maths.PI_HALF - epsilon;
                    }
                    else if (pitch < Maths.PI_NEG_HALF) {
                        pitch = Maths.PI_NEG_HALF + epsilon;
                    }

                    camera.SetYawPitch(yaw, pitch);
                    this.UpdateTextInfo();
                    this.OGLViewPort.InvalidateRender();
                }
                else if (e.RightButton == MouseButtonState.Pressed) {
                    Vector3 direction = camera.direction;
                    Vector3 rightward = Vector3.Normalize(Vector3.Cross(direction, Vector3.UnitY));
                    Vector3 upward = Vector3.Cross(rightward, direction);

                    float speed = camera.orbitRange / 1.5f;

                    Vector3 translationOffset = rightward * (changeX * speed) + upward * (changeY * speed);
                    camera.SetTarget(camera.target + translationOffset);

                    // Vector3 change = new Vector3(changeX * sensitivity * (this.camera.orbitRange / 2f), 0f, changeY * sensitivity * (this.camera.orbitRange / 2f));
                    // this.camera.SetTarget(this.camera.target - change);
                    this.UpdateTextInfo();
                    this.OGLViewPort.InvalidateRender();
                }
            }

            this.lastMouse = mpos;
        }

        public void UpdateTextInfo() {
            Camera camera = this.Editor.RenderViewport.Model.Camera;
            Vector3 tgt = camera.target;
            this.POS_LABEL.Content = $"Target Pos: \t{Math.Round(tgt.X, 2):F2} \t{Math.Round(tgt.Y, 2):F2} \t{Math.Round(tgt.Z, 2):F2}";
            this.ROT_LABEL.Content = $"Yaw:        \t{Math.Round(camera.yaw, 2):F2} \tPitch: \t{Math.Round(camera.pitch, 2):F2}";
        }

        // TODO: Could have ViewPortViewModel, which stores a reference to an interface (implemented by an
        // OpenGL control which references the SceneViewModel so that it can draw the objects, selection, etc)
        // Those are just ideas so far

        private void OnPaintViewPort(object sender, DrawEventArgs e) {
            // Update hidden window, if the size has changed
            Camera camera = this.Editor.RenderViewport.Model.Camera;
            camera.UpdateSize(e.Width, e.Height);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            // Render scene
            foreach (SceneObject obj in this.project.Scene.Root.Items) {
                obj.Render(camera);
            }

            // Cached ortho projection matrix for the VP size
            Matrix4x4 ortho = Matrix4x4.CreateOrthographic(e.Width, e.Height, 0.001f, 500f);

            // Render XYZ axis
            {
                const float size = 35f;
                const float gap = 10f;

                // This uses OpenTK's libraries but it doesn't render for some reason
                // Vector3 position = Rotation.GetOrbitPosition(camera.yaw, camera.pitch, 10f);
                // Matrix4x4 lineModelView = Matrix4x4.CreateLookAt(position, new Vector3(), Vector3.UnitY);
                // Vector3 pos = new Vector3(Maths.Map(size + gap, 0, e.Width, 1f, -1f), Maths.Map(size + gap, 0, e.Height, 1f, -1f), 0f);
                // Matrix4 o = Matrix4.CreateOrthographic(e.Width, e.Height, 0.001f, 500f);
                // Matrix4 mv = Matrix4.LookAt(position.X, position.Y, position.Z, 0f, 0f, 0f, 0f, 1f, 0f);
                // Matrix4 mvp = mv * o * Matrix4.CreateScale(size) * Matrix4.CreateTranslation(pos.X, pos.Y, pos.Z);
                // Matrix4x4 lineMvp = Unsafe.As<Matrix4, Matrix4x4>(ref mvp);
                // this.axisLineX.DrawAt(lineMvp, new Vector3(1f, 0f, 0f));
                // this.axisLineY.DrawAt(lineMvp, new Vector3(0f, 1f, 0f));
                // this.axisLineZ.DrawAt(lineMvp, new Vector3(0f, 0f, 1f));

                Vector3 position = Rotation.GetOrbitPosition(camera.yaw, camera.pitch);
                Matrix4x4 lineModelView = Matrix4x4.CreateLookAt(position, new Vector3(), Vector3.UnitY);

                // Calculates the screen position of the axis preview origin
                Vector3 pos = new Vector3(Maths.Map(size + gap, 0, e.Width, 1f, -1f), Maths.Map(size + gap, 0, e.Height, 1f, -1f), 0f);

                // Calculate the model-view-matrix of the line
                // Transformation is done at the end to apply translation after projection
                Matrix4x4 lineMvp = lineModelView * ortho * Matrix4x4.CreateScale(size) * Matrix4x4.CreateTranslation(pos);

                this.axisLineX.DrawAt(lineMvp, new Vector3(1f, 0f, 0f));
                this.axisLineY.DrawAt(lineMvp, new Vector3(0f, 1f, 0f));
                this.axisLineZ.DrawAt(lineMvp, new Vector3(0f, 0f, 1f));
            }

            // Draw target point
            {
                if (this.isOrbitActive) {
                    Vector3 position = Rotation.GetOrbitPosition(camera.yaw, camera.pitch);
                    Matrix4x4 lineModelView = Matrix4x4.CreateLookAt(position, new Vector3(), Vector3.UnitY);
                    Matrix4x4 mvp = lineModelView * ortho * Matrix4x4.CreateScale(10f);
                    this.targetPointLineX.DrawAt(mvp, new Vector3(0.9f, 0.2f, 0.2f), 1f);
                    this.targetPointLineY.DrawAt(mvp, new Vector3(0.2f, 0.9f, 0.2f), 1f);
                    this.targetPointLineZ.DrawAt(mvp, new Vector3(0.2f, 0.2f, 0.9f), 1f);
                }
            }
        }
    }
}