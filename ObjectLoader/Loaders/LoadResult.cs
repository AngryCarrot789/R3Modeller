using System.Collections.Generic;
using ObjectLoader.Data;
using ObjectLoader.Data.Elements;
using ObjectLoader.Data.VertexData;

namespace ObjectLoader.Loaders {
    public class LoadResult {
        public IList<Vertex> Vertices { get; set; }
        public IList<Texture> Textures { get; set; }
        public IList<Normal> Normals { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Material> Materials { get; set; }
    }
}