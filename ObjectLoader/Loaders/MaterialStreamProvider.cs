using System.IO;

namespace ObjectLoader.Loaders {
    public class MaterialStreamProvider : IMaterialStreamProvider {
        public string LoadFolder { get; }

        public MaterialStreamProvider(string loadFolder = null) {
            this.LoadFolder = loadFolder;
        }

        public Stream Open(string materialFilePath) {
            string file = this.LoadFolder != null ? Path.Combine(this.LoadFolder, materialFilePath) : materialFilePath;
            return File.Open(file, FileMode.Open, FileAccess.Read);
        }
    }

    public class MaterialNullStreamProvider : IMaterialStreamProvider {
        public Stream Open(string materialFilePath) {
            return null;
        }
    }
}