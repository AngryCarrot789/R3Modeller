using System.Collections.Generic;
using ObjectLoader.Data.Elements;
using ObjectLoader.Data.VertexData;

namespace ObjectLoader.Data.DataStore {
    public interface IDataStore {
        IList<Vertex> Vertices { get; }
        IList<Texture> Textures { get; }
        IList<Normal> Normals { get; }
        IList<Material> Materials { get; }
        IList<Group> Groups { get; }
    }
}