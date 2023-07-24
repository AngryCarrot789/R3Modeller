using System.Collections.Generic;
using System.Numerics;
using ObjectLoader.Data;
using ObjectLoader.Data.Elements;

namespace ObjectLoader.Loaders {
    public class LoadResult {
        public IList<Vector3> Vertices { get; set; }
        public IList<Vector3> Textures { get; set; }
        public IList<Vector3> Normals { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Material> Materials { get; set; }
    }
}