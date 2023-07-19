namespace ObjLoader.Data.VertexData {
    public struct Vertex {
        public Vertex(float x, float y, float z) : this() {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
    }
}