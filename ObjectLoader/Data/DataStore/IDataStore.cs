using System.Collections.Generic;
using System.Numerics;
using ObjectLoader.Data.Elements;

namespace ObjectLoader.Data.DataStore {
    public interface IDataStore {
        IList<Vector3> Vertices { get; }
        IList<Vector3> Textures { get; }
        IList<Vector3> Normals { get; }
        IList<Material> Materials { get; }
        IList<Group> Groups { get; }
    }
}