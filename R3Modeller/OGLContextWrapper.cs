using System;
using System.Runtime.CompilerServices;
using OpenTK;
using OpenTK.Graphics;

namespace R3Modeller {
    public class OGLContextWrapper : IDisposable {
        private readonly GameWindow window;

        public OGLContextWrapper() {
            this.window = new GameWindow(1, 1,
                new GraphicsMode(new ColorFormat(4, 4, 4, 4), 24, 0, 0, ColorFormat.Empty),
                "OpenTK Hidden Render Window",
                GameWindowFlags.FixedWindow, DisplayDevice.Default, 1, 0,
                GraphicsContextFlags.Offscreen | GraphicsContextFlags.Debug, null, true) {
                VSync = VSyncMode.Off
            };

            this.window.WindowBorder = WindowBorder.Hidden;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MakeCurrent(bool current) {
            if (current) {
                this.window.MakeCurrent();
            }
            else {
                this.window.Context.MakeCurrent(null);
            }
        }

        public void UpdateSize(int width, int height) {
            if (this.window.Width != width) {
                this.window.Width = width;
            }

            if (this.window.Height != height) {
                this.window.Height = height;
            }
        }

        public void Dispose() {
            this.window.Dispose();
        }
    }
}