using System.Collections.Generic;
using ObjLoader.Data;
using ObjLoader.Data.Elements;
using ObjLoader.Data.VertexData;

namespace ObjLoader.Loaders {
    public class LoadResult {
        public IList<Vertex> Vertices { get; set; }
        public IList<Texture> Textures { get; set; }
        public IList<Normal> Normals { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Material> Materials { get; set; }
    }
}