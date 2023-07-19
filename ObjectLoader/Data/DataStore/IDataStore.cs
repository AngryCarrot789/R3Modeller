using System.Collections.Generic;
using ObjLoader.Data.Elements;
using ObjLoader.Data.VertexData;

namespace ObjLoader.Data.DataStore {
    public interface IDataStore {
        IList<Vertex> Vertices { get; }
        IList<Texture> Textures { get; }
        IList<Normal> Normals { get; }
        IList<Material> Materials { get; }
        IList<Group> Groups { get; }
    }
}