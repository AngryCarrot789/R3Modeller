using System;
using OpenTK.Graphics.OpenGL;

namespace R3Modeller.Controls {
    public class OGLViewPortControl : BitmapRenderTarget {
        public readonly OGLContextWrapper ogl;
        private readonly Action invalidateVisualAction;

        public OGLViewPortControl() {
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
            this.ogl.MakeCurrent(false);

            this.invalidateVisualAction = this.InvalidateVisual;
        }

        protected override void OnPaint(DrawEventArgs e) {
            this.ogl.MakeCurrent(true);
            this.ogl.UpdateSize(e.Width, e.Height);

            base.OnPaint(e);

            // Read pixels from OpenGL's back buffer, then write into draw event's back buffer (aka the WPF bitmap)
            // The resulting pixels will be flipped vertically, because OpenGL origin direction is bottom-left, where as
            // windowing apps (like WPF) use the top left.
            // However the LayoutTransform property of the control can be used to scale the layout by -1 in the Y axis to flip
            // it back. It's most likely faster than doing a manual pixel copy, because the layout transformation (i'm guessing) is happening in DirectX
            GL.Flush();
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.ReadPixels(0, 0, e.Width, e.Height, PixelFormat.Bgra, PixelType.UnsignedByte, e.BackBuffer);
            this.ogl.MakeCurrent(false);
        }

        public void InvalidateRender(bool schedule = false) {
            if (schedule) {
                this.Dispatcher.InvokeAsync(this.invalidateVisualAction);
            }
            else if (this.Dispatcher.CheckAccess()) {
                this.InvalidateVisual();
            }
            else {
                this.Dispatcher.Invoke(this.invalidateVisualAction);
            }
        }
    }
}