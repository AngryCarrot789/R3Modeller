using ObjectLoader.Data.DataStore;
using ObjectLoader.TypeParsers.Interfaces;

namespace ObjectLoader.TypeParsers {
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