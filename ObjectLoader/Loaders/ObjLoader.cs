using System.Collections.Generic;
using System.IO;
using ObjectLoader.Data.DataStore;
using ObjectLoader.TypeParsers.Interfaces;

namespace ObjectLoader.Loaders {
    public class ObjLoader : LoaderBase, IObjLoader {
        private readonly IDataStore _dataStore;
        private readonly List<ITypeParser> _typeParsers = new List<ITypeParser>();

        private readonly List<string> _unrecognizedLines = new List<string>();

        public ObjLoader(
            IDataStore dataStore,
            IFaceParser faceParser,
            IGroupParser groupParser,
            INormalParser normalParser,
            ITextureParser textureParser,
            IVertexParser vertexParser,
            IMaterialLibraryParser materialLibraryParser,
            IUseMaterialParser useMaterialParser) {
            this._dataStore = dataStore;
            this.SetupTypeParsers(
                vertexParser,
                faceParser,
                normalParser,
                textureParser,
                groupParser,
                materialLibraryParser,
                useMaterialParser);
        }

        private void SetupTypeParsers(params ITypeParser[] parsers) {
            foreach (ITypeParser parser in parsers) {
                this._typeParsers.Add(parser);
            }
        }

        protected override void ParseLine(string keyword, string data) {
            foreach (ITypeParser typeParser in this._typeParsers) {
                if (typeParser.CanParse(keyword)) {
                    typeParser.Parse(data);
                    return;
                }
            }

            this._unrecognizedLines.Add(keyword + " " + data);
        }

        public LoadResult Load(Stream lineStream) {
            this.StartLoad(lineStream);
            return this.CreateResult();
        }

        private LoadResult CreateResult() {
            LoadResult result = new LoadResult {
                Vertices = this._dataStore.Vertices,
                Textures = this._dataStore.Textures,
                Normals = this._dataStore.Normals,
                Groups = this._dataStore.Groups,
                Materials = this._dataStore.Materials
            };
            return result;
        }
    }
}