using System.IO;

namespace ObjectLoader.Loaders {
    public interface IObjLoader {
        LoadResult Load(Stream lineStream);
    }
}