using ObjLoader.Data.DataStore;
using ObjLoader.TypeParsers.Interfaces;

namespace ObjLoader.TypeParsers {
    public class GroupParser : TypeParserBase, IGroupParser {
        private readonly IGroupDataStore _groupDataStore;

        public GroupParser(IGroupDataStore groupDataStore) {
            this._groupDataStore = groupDataStore;
        }

        protected override string Keyword {
            get { return "g"; }
        }

        public override void Parse(string line) {
            this._groupDataStore.PushGroup(line);
        }
    }
}