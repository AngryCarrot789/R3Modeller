using System.IO;

namespace ObjLoader.Loaders {
    public interface IObjLoader {
        LoadResult Load(Stream lineStream);
    }
}