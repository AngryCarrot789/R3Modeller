namespace R3Modeller.Controls {
    public readonly struct OGLDrawEventArgs {
        public readonly int Width;
        public readonly int Height;
        public readonly double ScaleX;
        public readonly double ScaleY;

        public OGLDrawEventArgs(int width, int height, double scaleX, double scaleY) {
            this.Width = width;
            this.Height = height;
            this.ScaleX = scaleX;
            this.ScaleY = scaleY;
        }
    }
}