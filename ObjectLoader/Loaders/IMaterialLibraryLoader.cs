using System.IO;

namespace ObjLoader.Loaders {
    public interface IMaterialLibraryLoader {
        void Load(Stream lineStream);
    }
}