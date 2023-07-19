using System.IO;

namespace ObjectLoader.Loaders {
    public class MaterialLibraryLoaderFacade : IMaterialLibraryLoaderFacade {
        private readonly IMaterialLibraryLoader _loader;
        private readonly IMaterialStreamProvider _materialStreamProvider;

        public MaterialLibraryLoaderFacade(IMaterialLibraryLoader loader, IMaterialStreamProvider materialStreamProvider) {
            this._loader = loader;
            this._materialStreamProvider = materialStreamProvider;
        }

        public void Load(string materialFileName) {
            using (Stream stream = this._materialStreamProvider.Open(materialFileName)) {
                if (stream != null) {
                    this._loader.Load(stream);
                }
            }
        }
    }
}