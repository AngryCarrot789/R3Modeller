using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace R3Modeller.Controls {
    /// <summary>
    /// A control that allows drawing into a <see cref="WriteableBitmap"/>, while automatically handling when the control's size changes
    /// <para>
    /// Rendering can be done by handling the <see cref="Paint"/> event
    /// </para>
    /// </summary>
    public class BitmapRenderTarget : FrameworkElement {
        private readonly bool designMode;
        private WriteableBitmap bitmap; // stuff draws into this, then it is rendered

        [Category("Appearance")]
        public event EventHandler<OGLDrawEventArgs> Paint;

        public BitmapRenderTarget() {
            this.designMode = DesignerProperties.GetIsInDesignMode(this);
        }

        /// <param name="dc">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.</summary>
        /// <remarks />
        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);
            PresentationSource source;
            if (this.designMode || this.Visibility != Visibility.Visible || (source = PresentationSource.FromVisual(this)) == null)
                return;
            SKSizeI size;
            double scaleX, scaleY;
            double actualWidth = this.ActualWidth;
            double actualHeight = this.ActualHeight;
            if (IsPositive(actualWidth) && IsPositive(actualHeight)) {
                Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
                scaleX = (float) transformToDevice.M11;
                scaleY = (float) transformToDevice.M22;
                size = new SKSizeI((int) Math.Ceiling(actualWidth * scaleX), (int) Math.Ceiling(actualHeight * scaleY));
            }
            else {
                scaleX = scaleY = 1d;
                size = new SKSizeI();
            }

            if (size.Width <= 0 || size.Height <= 0)
                return;
            if (this.bitmap == null || size.Width != this.bitmap.PixelWidth || size.Height != this.bitmap.PixelHeight)
                this.bitmap = new WriteableBitmap(size.Width, size.Height, 96.0 * scaleX, 96.0 * scaleY, PixelFormats.Pbgra32, null);
            this.bitmap.Lock();

            this.Paint?.Invoke(this, new OGLDrawEventArgs(size.Width, size.Height, scaleX, scaleY));

            this.bitmap.AddDirtyRect(new Int32Rect(0, 0, size.Width, size.Height));
            this.bitmap.Unlock();
            dc.DrawImage(this.bitmap, new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight));
        }

        private static bool IsPositive(double value) => !double.IsNaN(value) && !double.IsInfinity(value) && value > 0.0;

        private readonly struct SKSizeI {
            public readonly int Width, Height;

            public SKSizeI(int width, int height) {
                this.Width = width;
                this.Height = height;
            }
        }
    }
}