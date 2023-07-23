using System.Numerics;
using ObjectLoader.Data.VertexData;

namespace ObjectLoader.Data.DataStore {
    public interface ITextureDataStore {
        void AddTexture(Vector3 texture);
    }
}