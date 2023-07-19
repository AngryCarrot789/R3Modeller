using ObjectLoader.Loaders;
using ObjectLoader.TypeParsers.Interfaces;

namespace ObjectLoader.TypeParsers {
    public class MaterialLibraryParser : TypeParserBase, IMaterialLibraryParser {
        private readonly IMaterialLibraryLoaderFacade _libraryLoaderFacade;

        public MaterialLibraryParser(IMaterialLibraryLoaderFacade libraryLoaderFacade) {
            this._libraryLoaderFacade = libraryLoaderFacade;
        }

        protected override string Keyword {
            get { return "mtllib"; }
        }

        public override void Parse(string line) {
            this._libraryLoaderFacade.Load(line);
        }
    }
}