using System.Collections.Generic;

namespace ObjectLoader.Data.Elements {
    public class Face {
        public readonly List<FaceVertex> _vertices = new List<FaceVertex>();

        public void AddVertex(FaceVertex vertex) {
            this._vertices.Add(vertex);
        }

        public FaceVertex this[int i] {
            get { return this._vertices[i]; }
        }

        public int Count {
            get { return this._vertices.Count; }
        }
    }

    public struct FaceVertex {
        public FaceVertex(int vertexIndex, int textureIndex, int normalIndex) : this() {
            this.VertexIndex = vertexIndex;
            this.TextureIndex = textureIndex;
            this.NormalIndex = normalIndex;
        }

        public int VertexIndex { get; set; }
        public int TextureIndex { get; set; }
        public int NormalIndex { get; set; }
    }
}