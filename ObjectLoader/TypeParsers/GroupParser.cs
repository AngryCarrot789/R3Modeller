using ObjectLoader.Data.DataStore;
using ObjectLoader.TypeParsers.Interfaces;

namespace ObjectLoader.TypeParsers {
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