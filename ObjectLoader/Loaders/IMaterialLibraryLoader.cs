using System.IO;

namespace ObjectLoader.Loaders {
    public interface IMaterialLibraryLoader {
        void Load(Stream lineStream);
    }
}