using ObjLoader.Data.DataStore;
using ObjLoader.TypeParsers.Interfaces;

namespace ObjLoader.TypeParsers {
    public class UseMaterialParser : TypeParserBase, IUseMaterialParser {
        private readonly IElementGroup _elementGroup;

        public UseMaterialParser(IElementGroup elementGroup) {
            this._elementGroup = elementGroup;
        }

        protected override string Keyword {
            get { return "usemtl"; }
        }

        public override void Parse(string line) {
            this._elementGroup.SetMaterial(line);
        }
    }
}