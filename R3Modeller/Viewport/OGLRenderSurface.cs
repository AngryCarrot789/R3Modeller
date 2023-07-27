using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Controls;
using R3Modeller.Core.Engine;
using R3Modeller.Core.Engine.Objs;
using R3Modeller.Core.Engine.Utils;
using R3Modeller.Core.Engine.ViewModels;
using R3Modeller.Core.Rendering;
using R3Modeller.Core.Utils;
using R3Modeller.Utils;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace R3Modeller.Viewport {
    public class OGLRenderSurface : Control, IRenderTarget {
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register("Viewport", typeof(RenderViewportViewModel), typeof(OGLRenderSurface), new PropertyMetadata(null));

        public RenderViewportViewModel Viewport {
            get => (RenderViewportViewModel) this.GetValue(ViewportProperty);
            set => this.SetValue(ViewportProperty, value);
        }

        public readonly OGLContextWrapper ogl;
        private readonly Action invalidateVisualAction;

        private readonly LineObject axisLineX;
        private readonly LineObject axisLineY;
        private readonly LineObject axisLineZ;
        private readonly LineObject targetPointLineX;
        private readonly LineObject targetPointLineY;
        private readonly LineObject targetPointLineZ;

        private bool isOrbitActive;
        private bool ignoreMouseMoveEvent;
        private Point? lastMouse;
        private Point? mousePosBeforeOrbitEnabled;
        private int contextUsageCounter;

        private readonly bool isDesignerMode;
        private volatile bool isRenderScheduled;

        // This will be the same size as the BRT, but without the -1 scale on the Y axis
        private Border PART_RenderTarget_LayoutProxy;
        public BitmapRenderTarget PART_RenderTarget;

        public OGLRenderSurface() {
            this.isDesignerMode = DesignerProperties.GetIsInDesignMode(this);
            this.invalidateVisualAction = () => this.PART_RenderTarget?.InvalidateVisual();
            if (this.isDesignerMode) {
                return;
            }

            this.ogl = new OGLContextWrapper();
            this.ogl.MakeCurrent(true);
            GL.ClearColor(0.2f, 0.4f, 0.8f, 1.0f);
            // disabled so that both faces are rendered
            // GL.Enable(EnableCap.CullFace);
            // GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.Multisample);
            GL.DepthMask(true);

            this.axisLineX = new LineObject(new Vector3(), new Vector3(1f, 0f, 0f));
            this.axisLineY = new LineObject(new Vector3(), new Vector3(0f, 1f, 0f));
            this.axisLineZ = new LineObject(new Vector3(), new Vector3(0f, 0f, 1f));
            this.targetPointLineX = new LineObject(new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f));
            this.targetPointLineY = new LineObject(new Vector3(0f, 1f, 0f), new Vector3(0f, -1f, 0f));
            this.targetPointLineZ = new LineObject(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, -1f));
            this.ogl.MakeCurrent(false);
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            this.PART_RenderTarget = (BitmapRenderTarget) this.GetTemplateChild("PART_RenderTarget");
            this.PART_RenderTarget_LayoutProxy = (Border) this.GetTemplateChild("PART_RenderTarget_LayoutProxy");
            if (!this.isDesignerMode && this.PART_RenderTarget != null) {
                this.PART_RenderTarget.Paint += this.OnPaint;
            }
        }

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate) {
            base.OnTemplateChanged(oldTemplate, newTemplate);
            if (!this.isDesignerMode && this.PART_RenderTarget != null) {
                this.PART_RenderTarget.Paint -= this.OnPaint;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
            switch (e.Key) {
                case Key.System: {
                    if (!e.IsRepeat && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)) {
                        e.Handled = true;
                        this.mousePosBeforeOrbitEnabled = Mouse.GetPosition(this);
                        this.isOrbitActive = true;
                        this.UpdateCursor();
                        this.InvalidateRender();
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
                    if (!e.IsRepeat && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)) {
                        e.Handled = true;
                        this.isOrbitActive = false;
                        this.UpdateCursor();
                        this.InvalidateRender();
                    }

                    break;
                }
                default: return;
            }
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
            base.OnPreviewMouseWheel(e);
            if (e.Delta == 0) {
                return;
            }

            if (!(this.Viewport is RenderViewportViewModel vp)) {
                return;
            }

            CameraViewModel camera = vp.Camera;
            float oldRange = camera.OrbitRange;
            float newRange;
            if (oldRange < 0.01f) {
                newRange = Maths.Clamp(e.Delta > 0 ? (oldRange / 20f) : (oldRange * 20f), 0.0001f, 5000f);
            }
            else {
                float multiplier = e.Delta > 0 ? (1f - 0.25f) : (1f + 0.25f);
                newRange = Maths.Clamp(oldRange * multiplier, 0.0001f, 5000f);
            }

            if (Math.Abs(newRange - oldRange) > 0.00001f) {
                camera.OrbitRange = newRange;
                this.InvalidateRender();
            }
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

        private static bool IsOrbitKeyPressed() {
            return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (this.ignoreMouseMoveEvent) {
                return;
            }

            if (IsOrbitKeyPressed()) {
                if (!this.isOrbitActive) {
                    this.isOrbitActive = true;
                    this.UpdateCursor();
                }
            }
            else if (this.isOrbitActive) {
                this.isOrbitActive = false;
                this.UpdateCursor();
            }

            Point mpos = e.GetPosition(this); // use "this" instead of OGLViewPort as it's easier due to the scale being -1
            if (this.lastMouse is Point lastPos && this.isOrbitActive) {
                if (Math.Abs(mpos.X - lastPos.X) < 0.001f && Math.Abs(mpos.Y - lastPos.Y) < 0.001f) {
                    return;
                }

                bool wrap = false;
                double wrapX = mpos.X, w = this.ActualWidth;
                double wrapY = mpos.Y, h = this.ActualHeight;

                if (mpos.X < 0) {
                    wrapX = w;
                    wrap = true;
                }
                else if (mpos.X > w) {
                    wrapX = 0;
                    wrap = true;
                }

                if (mpos.Y < 0) {
                    wrapY = h;
                    wrap = true;
                }
                else if (mpos.Y > h) {
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

                double width = this.ActualWidth;
                double height = this.ActualHeight;

                double changeX = 1d + Maths.Map(mpos.X - lastPos.X, 0d, width, -1d, 1d);
                double changeY = 1d - Maths.Map(mpos.Y - lastPos.Y, 0d, height, 1d, -1d);

                Camera camera = this.Viewport.Camera.Model;
                const double sensitivity = 0.8d;
                // double aspW = width / height;
                // double aspH = height / width;
                // double sX = (float) sensitivity * Math.Max(aspW, aspH);
                // double sY = (float) sensitivity * Math.Max(aspW, aspH);
                const double sX = sensitivity;
                const double sY = sensitivity;
                const float epsilon = 0.00001f;
                if (e.LeftButton == MouseButtonState.Pressed) {
                    float yaw = camera.yaw;
                    float pitch = camera.pitch;
                    yaw -= (float) (changeX * sX);
                    if (yaw > Maths.PI) {
                        yaw = Maths.PI_NEG + epsilon;
                    }
                    else if (yaw < Maths.PI_NEG) {
                        yaw = Maths.PI - epsilon;
                    }

                    pitch -= (float) (changeY * sY);
                    if (pitch > Maths.PI_HALF) {
                        pitch = Maths.PI_HALF - epsilon;
                    }
                    else if (pitch < Maths.PI_NEG_HALF) {
                        pitch = Maths.PI_NEG_HALF + epsilon;
                    }

                    camera.SetYawPitch(yaw, pitch);
                    this.InvalidateRender();
                }
                else if (e.RightButton == MouseButtonState.Pressed) {
                    Vector3 direction = camera.direction;
                    Vector3 rightward = Vector3.Normalize(Vector3.Cross(direction, Vector3.UnitY));
                    Vector3 upward = Vector3.Cross(rightward, direction);
                    double speed = camera.orbitRange / 1.5d;
                    Vector3 translationOffset = rightward * (float) (changeX * speed) + upward * (float) (changeY * speed);
                    camera.SetTarget(camera.target + translationOffset);
                    this.InvalidateRender();
                }
            }

            this.lastMouse = mpos;
        }

        private void OnPaint(object sender, DrawEventArgs e) {
            // Update hidden window, if the size has changed
            if (!(this.Viewport is RenderViewportViewModel vp)) {
                return;
            }

            this.ogl.MakeCurrent(true);
            this.ogl.UpdateSize(e.Width, e.Height);

            Camera camera = vp.Model.Camera;
            camera.UpdateSize(e.Width, e.Height);

            ProjectViewModel project = vp.Editor.Project;
            if (project != null) {
                this.PaintInternal(camera, project.Model, e);
            }

            // Read pixels from OpenGL's back buffer, then write into draw event's back buffer (aka the WPF bitmap)
            // The resulting pixels will be flipped vertically, because OpenGL origin direction is bottom-left, where as
            // windowing apps (like WPF) use the top left.
            // However the LayoutTransform property of the control can be used to scale the layout by -1 in the Y axis to flip
            // it back. It's most likely faster than doing a manual pixel copy, because the layout transformation (i'm guessing) is happening in DirectX
            GL.Flush();
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.ReadPixels(0, 0, e.Width, e.Height, PixelFormat.Bgra, PixelType.UnsignedByte, e.BackBuffer);
            this.ogl.MakeCurrent(false);
            this.isRenderScheduled = false;
        }

        private void PaintInternal(Camera camera, Project project, DrawEventArgs e) {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            // Render scene
            IReadOnlyList<SceneObject> items = project.Scene.Root.Items;
            for (int i = 0, c = items.Count; i < c; i++) {
                SceneObject obj = items[i];
                if (obj.IsVisible) {
                    obj.Render(camera);
                }
            }

            // Cached ortho projection matrix for the VP size
            Matrix4x4 ortho = Matrix4x4.CreateOrthographic(e.Width, e.Height, 0.001f, 500f);

            {
                // Render XYZ axis on top right
                const float size = 35f;
                const float gap = 10f;

                Vector3 camPos = Rotation.GetOrbitPosition(camera.yaw, camera.pitch);
                Matrix4x4 view = Matrix4x4.CreateLookAt(camPos, new Vector3(), Vector3.UnitY);

                // Calculates the screen position of the axis preview origin
                Vector3 axisPos = new Vector3(Maths.Map(size + gap, 0, e.Width, 1f, -1f), Maths.Map(size + gap, 0, e.Height, 1f, -1f), 0f);

                // Calculate the model-view-matrix of the line
                // Transformation is done at the end to apply translation after projection and scale
                Matrix4x4 lineMvp = view * ortho * Matrix4x4.CreateScale(size) * Matrix4x4.CreateTranslation(axisPos);
                Matrix4 mat4 = Unsafe.As<Matrix4x4, Matrix4>(ref lineMvp);
                Matrix4x4 mvp = Unsafe.As<Matrix4, Matrix4x4>(ref mat4);

                this.axisLineX.DrawAt(mvp, new Vector4(1f, 0f, 0f, 1f));
                this.axisLineY.DrawAt(mvp, new Vector4(0f, 1f, 0f, 1f));
                this.axisLineZ.DrawAt(mvp, new Vector4(0f, 0f, 1f, 1f));
            }

            {
                // Draw orbit target point
                if (this.isOrbitActive) {
                    Vector3 position = Rotation.GetOrbitPosition(camera.yaw, camera.pitch);
                    Matrix4x4 lineModelView = Matrix4x4.CreateLookAt(position, new Vector3(), Vector3.UnitY);
                    Matrix4x4 mvp = lineModelView * ortho * Matrix4x4.CreateScale(10f);
                    this.targetPointLineX.DrawAt(mvp, new Vector4(0.9f, 0.2f, 0.2f, 0.6f), 1f);
                    this.targetPointLineY.DrawAt(mvp, new Vector4(0.2f, 0.9f, 0.2f, 0.6f), 1f);
                    this.targetPointLineZ.DrawAt(mvp, new Vector4(0.2f, 0.2f, 0.9f, 0.6f), 1f);
                }
            }
        }

        public void InvalidateRender(bool schedule = false) {
            if (this.isRenderScheduled) {
                return;
            }

            if (schedule) {
                this.Dispatcher.InvokeAsync(this.invalidateVisualAction);
            }
            else if (this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(this.invalidateVisualAction, System.Windows.Threading.DispatcherPriority.Render);
                // this.invalidateVisualAction();
            }
            else {
                this.Dispatcher.Invoke(this.invalidateVisualAction);
            }
        }

        public void BeginFrame() {
            if (++this.contextUsageCounter == 1) {
                this.ogl.MakeCurrent(true);
            }
        }

        public void EndFrame() {
            if (--this.contextUsageCounter == 0) {
                this.ogl.MakeCurrent(false);
            }

            if (this.contextUsageCounter < 0) {
                throw new Exception($"Excessive calls to {nameof(this.EndFrame)}()");
            }
        }
    }
}