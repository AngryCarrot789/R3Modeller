using System.IO;

namespace ObjLoader.Loaders {
    public class MaterialStreamProvider : IMaterialStreamProvider {
        public Stream Open(string materialFilePath) {
            return File.Open(materialFilePath, FileMode.Open, FileAccess.Read);
        }
    }

    public class MaterialNullStreamProvider : IMaterialStreamProvider {
        public Stream Open(string materialFilePath) {
            return null;
        }
    }
}