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
        public event EventHandler<DrawEventArgs> Paint;

        public BitmapRenderTarget() {
            this.designMode = DesignerProperties.GetIsInDesignMode(this);
        }

        /// <param name="dc">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.</summary>
        /// <remarks />
        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);
            PresentationSource source;
            EventHandler<DrawEventArgs> paint = this.Paint;
            if (this.designMode || paint == null || (source = PresentationSource.FromVisual(this)) == null) {
                return;
            }

            double actualWidth = this.ActualWidth;
            double actualHeight = this.ActualHeight;
            if (!IsPositive(actualWidth) || !IsPositive(actualHeight)) {
                return;
            }

            Matrix matrix = source.CompositionTarget.TransformToDevice;
            double scaleX = (float) matrix.M11;
            double scaleY = (float) matrix.M22;
            int width = (int) (actualWidth * scaleX);
            int height = (int) (actualHeight * scaleY);
            if (width <= 0 || height <= 0) {
                return;
            }

            if (this.bitmap == null || width != this.bitmap.PixelWidth || height != this.bitmap.PixelHeight) {
                this.bitmap = new WriteableBitmap(width, height, 96.0 * scaleX, 96.0 * scaleY, PixelFormats.Pbgra32, null);
            }

            this.bitmap.Lock();
            paint(this, new DrawEventArgs(this.bitmap.BackBuffer, width, height, scaleX, scaleY));
            this.bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
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