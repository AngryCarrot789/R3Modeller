using System.Collections.Generic;
using System.Numerics;
using ObjectLoader.Data.Elements;
using ObjectLoader.Data.VertexData;

namespace ObjectLoader.Data.DataStore {
    public interface IDataStore {
        IList<Vector3> Vertices { get; }
        IList<Vector3> Textures { get; }
        IList<Normal> Normals { get; }
        IList<Material> Materials { get; }
        IList<Group> Groups { get; }
    }
}