using System.Collections.Generic;

namespace ObjectLoader.Data.Elements {
    public class Face {
        public readonly List<FaceVertex> Vertices = new List<FaceVertex>();
    }

    public struct FaceVertex {
        public int VertexIndex { get; set; }
        public int TextureIndex { get; set; }
        public int NormalIndex { get; set; }

        public FaceVertex(int vertexIndex, int textureIndex, int normalIndex) : this() {
            this.VertexIndex = vertexIndex;
            this.TextureIndex = textureIndex;
            this.NormalIndex = normalIndex;
        }
    }
}