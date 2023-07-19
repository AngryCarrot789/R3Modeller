using ObjectLoader.Common;
using ObjectLoader.Data.DataStore;
using ObjectLoader.Data.VertexData;
using ObjectLoader.TypeParsers.Interfaces;

namespace ObjectLoader.TypeParsers {
    public class NormalParser : TypeParserBase, INormalParser {
        private readonly INormalDataStore _normalDataStore;

        public NormalParser(INormalDataStore normalDataStore) {
            this._normalDataStore = normalDataStore;
        }

        protected override string Keyword {
            get { return "vn"; }
        }

        public override void Parse(string line) {
            string[] parts = line.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            Normal normal = new Normal(x, y, z);
            this._normalDataStore.AddNormal(normal);
        }
    }
}