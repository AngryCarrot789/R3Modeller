namespace ObjectLoader.Data.VertexData {
    public struct Texture {
        public Texture(float x, float y) : this() {
            this.X = x;
            this.Y = y;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
    }
}