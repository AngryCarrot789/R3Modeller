using System;
using System.Collections;
using System.Collections.Generic;
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
using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.Engine.Utils;
using R3Modeller.Core.Engine.ViewModels;
using R3Modeller.Core.Notifications;
using R3Modeller.Core.Utils;
using R3Modeller.Notifications;
using R3Modeller.Themes;
using R3Modeller.Utils;
using R3Modeller.Views;
using Vector3 = System.Numerics.Vector3;

namespace R3Modeller {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx, INotificationHandler {
        private readonly Camera camera;

        private readonly LineObject axisLineX;
        private readonly LineObject axisLineY;
        private readonly LineObject axisLineZ;

        private readonly LineObject targetPointLineX;
        private readonly LineObject targetPointLineY;
        private readonly LineObject targetPointLineZ;

        private readonly Project project;

        private bool isOrbitActive;

        public NotificationPanelViewModel NotificationPanel { get; }

        public static readonly DependencyProperty PropertyPageItemsSourceProperty = DependencyProperty.Register("PropertyPageItemsSource", typeof(IEnumerable), typeof(MainWindow), new PropertyMetadata(null));

        public IEnumerable PropertyPageItemsSource {
            get => (IEnumerable) this.GetValue(PropertyPageItemsSourceProperty);
            set => this.SetValue(PropertyPageItemsSourceProperty, value);
        }

        public MainWindow() {
            this.InitializeComponent();
            this.project = new Project();
            this.OGLViewPort.BeginFrame();
            this.camera = new Camera();
            this.camera.SetYawPitch(0.45f, -0.35f);
            this.project.Scene.AddItem(new TriangleObject());
            TriangleObject tri = new TriangleObject();
            tri.SetPosition(new Vector3(3f, 2f, 3f));
            this.project.Scene.rootList[0].AddChild(tri);

            this.project.Scene.AddItem(new FloorPlaneObject());
            this.axisLineX = new LineObject(new Vector3(), new Vector3(1f, 0f, 0f));
            this.axisLineY = new LineObject(new Vector3(), new Vector3(0f, 1f, 0f));
            this.axisLineZ = new LineObject(new Vector3(), new Vector3(0f, 0f, 1f));

            this.targetPointLineX = new LineObject(new Vector3(-1f, 0f, 0f), new Vector3(1f,  0f,  0f));
            this.targetPointLineY = new LineObject(new Vector3( 0f, 1f, 0f), new Vector3(0f, -1f,  0f));
            this.targetPointLineZ = new LineObject(new Vector3( 0f, 0f, 1f), new Vector3(0f,  0f, -1f));

            #region obj loader

            // string vertexShader =
            //     "#version 330\n" +
            //     "uniform mat4 mvp;\n" +
            //     "in vec3 in_pos;\n" +
            //     "void main() { gl_Position = mvp * vec4(in_pos, 1.0); }";
            // string fragmentShader =
            //     "#version 330\n" +
            //     "void main() { gl_FragColor = vec4(0.8, 0.2, 1.0, 1.0); }\n";
            // this.shader = new Shader(vertexShader, fragmentShader);

            ObjLoaderFactory factory = new ObjLoaderFactory();
            IObjLoader loader = factory.Create(new MaterialStreamProvider(ResourceLocator.ResourceDirectory));
            using (FileStream stream = File.OpenRead(ResourceLocator.GetResourceFile("untitled.obj"))) {
                LoadResult result = loader.Load(stream);
                SceneObject objFile = new SceneObject() {
                    DisplayName = "untitled.obj"
                };

                foreach (Group group in result.Groups) {
                    objFile.AddChild(new WavefrontObject(result, group));
                }

                this.project.Scene.AddItem(objFile);
            }

            this.OGLViewPort.EndFrame();

            this.DataContext = new EditorViewModel() {
                Project = new ProjectViewModel(this.OGLViewPort, this.project) {
                    Editor = (EditorViewModel) this.DataContext
                }
            };

            this.NotificationPanel = new NotificationPanelViewModel(this);
            this.NotificationPanelPopup.StaysOpen = true;
            this.NotificationPanelPopup.Placement = PlacementMode.Absolute;
            this.NotificationPanelPopup.PlacementTarget = this;
            this.NotificationPanelPopup.PlacementRectangle = System.Windows.Rect.Empty;
            this.NotificationPanelPopup.DataContext = this.NotificationPanel;
            this.RefreshPopupLocation();

            #endregion
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
            Vector3 pos = this.camera.target;
            switch (e.Key) {
                case Key.W: pos.Z -= 0.1f; break;
                case Key.A: pos.X -= 0.1f; break;
                case Key.S: pos.Z += 0.1f; break;
                case Key.D: pos.X += 0.1f; break;
                case Key.Space:     pos.Y += 0.1f; break;
                case Key.LeftShift: pos.Y -= 0.1f; break;
                case Key.System: {
                    if (!e.IsRepeat && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt) {
                        this.isOrbitActive = true;
                    }

                    break;
                }
                default: return;
            }

            this.camera.SetTarget(pos);
            this.UpdateTextInfo();
            this.OGLViewPort.InvalidateRender();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
            base.OnPreviewMouseWheel(e);
            if (e.Delta == 0) {
                return;
            }

            float oldRange = this.camera.orbitRange;
            float multiplier = e.Delta > 0 ? (1f - 0.25f) : (1f + 0.25f);
            float newRange = Maths.Clamp(oldRange * multiplier, 2f, 750f);
            if (Math.Abs(newRange - oldRange) > 0.001f) {
                this.camera.SetOrbitRange(newRange);
                this.OGLViewPort.InvalidateRender();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            switch (e.Key) {
                case Key.System: {
                    if (!e.IsRepeat && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt) {
                        this.isOrbitActive = false;
                    }

                    break;
                }
                default: return;
            }

            this.OGLViewPort.InvalidateRender();
        }

        private Point? lastMouse;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            this.Focus();
            this.CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
            this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            Point currPos = e.GetPosition(this);
            if (this.lastMouse is Point lastPos && this.isOrbitActive) {
                float changeX = 1f + (float) Maths.Map(currPos.X - lastPos.X, 0d, this.ActualWidth, -1d, 1d);
                float changeY = 1f - (float) Maths.Map(currPos.Y - lastPos.Y, 0d, this.ActualHeight, 1d, -1d);

                const float sensitivity = 1.75f;
                const float epsilon = 0.00001f;
                if (e.LeftButton == MouseButtonState.Pressed) {
                    float yaw = this.camera.yaw;
                    float pitch = this.camera.pitch;
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

                    this.camera.SetYawPitch(yaw, pitch);
                    this.UpdateTextInfo();
                    this.OGLViewPort.InvalidateRender();
                }
                else if (e.RightButton == MouseButtonState.Pressed) {
                    Vector3 direction = new Vector3(
                        (float) (Math.Cos(-this.camera.pitch) * Math.Sin(this.camera.yaw)),
                        (float) Math.Sin(-this.camera.pitch),
                        (float) (Math.Cos(-this.camera.pitch) * Math.Cos(this.camera.yaw))
                    );

                    Vector3 rightward = Vector3.Normalize(Vector3.Cross(direction, Vector3.UnitY));
                    Vector3 upward = Vector3.Cross(rightward, direction);

                    float speed = this.camera.orbitRange / 1.5f;

                    Vector3 translationOffset = rightward * (changeX * speed) + upward * (changeY * speed);
                    this.camera.SetTarget(this.camera.target + translationOffset);

                    // Vector3 change = new Vector3(changeX * sensitivity * (this.camera.orbitRange / 2f), 0f, changeY * sensitivity * (this.camera.orbitRange / 2f));
                    // this.camera.SetTarget(this.camera.target - change);
                    this.UpdateTextInfo();
                    this.OGLViewPort.InvalidateRender();
                }
            }

            this.lastMouse = currPos;
        }

        public void UpdateTextInfo() {
            Vector3 tgt = this.camera.target;
            this.POS_LABEL.Content = $"Target Pos: \t{Math.Round(tgt.X, 2):F2} \t{Math.Round(tgt.Y, 2):F2} \t{Math.Round(tgt.Z, 2):F2}";
            this.ROT_LABEL.Content = $"Yaw:        \t{Math.Round(this.camera.yaw, 2):F2} \tPitch: \t{Math.Round(this.camera.pitch, 2):F2}";
        }

        // TODO: Could have ViewPortViewModel, which stores a reference to an interface (implemented by an
        // OpenGL control which references the SceneViewModel so that it can draw the objects, selection, etc)
        // Those are just ideas so far

        private void OnPaintViewPort(object sender, DrawEventArgs e) {
            // Update hidden window, if the size has changed
            this.camera.UpdateSize(e.Width, e.Height);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            // Render scene
            foreach (SceneObject obj in this.project.Scene.rootList) {
                obj.Render(this.camera);
            }

            // Render XYZ axis
            {
                // TODO: cache these matrices maybe
                Vector3 position = Rotation.GetOrbitPosition(this.camera.yaw, this.camera.pitch, 10f);
                Matrix4x4 lineModelView = Matrix4x4.CreateLookAt(position, new Vector3(), Vector3.UnitY);

                // Calculates the screen position of the axis preview origin
                Vector3 pos = new Vector3(Maths.Map(60f, 0, e.Width, 1f, -1f), Maths.Map(60f, 0, e.Height, 1f, -1f), 0f);

                // Calculate the model-view-matrix of the line
                // Translation is done at the end to apply translation after projection
                Matrix4x4 lineMvp = lineModelView * this.camera.proj * Matrix4x4.CreateTranslation(pos);
                this.axisLineX.DrawAt(lineMvp, new Vector3(1f, 0f, 0f));
                this.axisLineY.DrawAt(lineMvp, new Vector3(0f, 1f, 0f));
                this.axisLineZ.DrawAt(lineMvp, new Vector3(0f, 0f, 1f));
            }

            // Draw target point
            {
                if (this.isOrbitActive) {
                    Vector3 position = Rotation.GetOrbitPosition(this.camera.yaw, this.camera.pitch, 10f);
                    Matrix4x4 lineModelView = Matrix4x4.CreateLookAt(position, new Vector3(), Vector3.UnitY);
                    Matrix4x4 mvp = lineModelView * this.camera.proj * Matrix4x4.CreateScale(0.25f);
                    this.targetPointLineX.DrawAt(mvp, new Vector3(0.3f, 0.4f, 0.3f), 2f);
                    this.targetPointLineY.DrawAt(mvp, new Vector3(0.3f, 0.4f, 0.3f), 2f);
                    this.targetPointLineZ.DrawAt(mvp, new Vector3(0.3f, 0.4f, 0.3f), 2f);
                }
            }
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            double diff = e.NewValue - e.OldValue;
            this.project.Scene.rootList[0].SetPosition(this.project.Scene.rootList[0].pos + new Vector3((float) diff));
            this.OGLViewPort.InvalidateRender();
        }
    }
}