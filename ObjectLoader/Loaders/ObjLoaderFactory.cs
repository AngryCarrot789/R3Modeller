using System.IO;
using ObjectLoader.Data.DataStore;
using ObjectLoader.TypeParsers;

namespace ObjectLoader.Loaders {
    public interface IMaterialStreamProvider {
        Stream Open(string materialFilePath);
    }

    public class ObjLoaderFactory : IObjLoaderFactory {
        public IObjLoader Create() {
            return this.Create(new MaterialStreamProvider());
        }

        public IObjLoader Create(IMaterialStreamProvider materialStreamProvider) {
            DataStore dataStore = new DataStore();

            FaceParser faceParser = new FaceParser(dataStore);
            GroupParser groupParser = new GroupParser(dataStore);
            NormalParser normalParser = new NormalParser(dataStore);
            TextureParser textureParser = new TextureParser(dataStore);
            VertexParser vertexParser = new VertexParser(dataStore);

            MaterialLibraryLoader materialLibraryLoader = new MaterialLibraryLoader(dataStore);
            MaterialLibraryLoaderFacade materialLibraryLoaderFacade = new MaterialLibraryLoaderFacade(materialLibraryLoader, materialStreamProvider);
            MaterialLibraryParser materialLibraryParser = new MaterialLibraryParser(materialLibraryLoaderFacade);
            UseMaterialParser useMaterialParser = new UseMaterialParser(dataStore);

            return new ObjLoader(dataStore, faceParser, groupParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);
        }
    }
}