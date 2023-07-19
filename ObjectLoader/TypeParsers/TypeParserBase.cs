using ObjectLoader.Common;
using ObjectLoader.TypeParsers.Interfaces;

namespace ObjectLoader.TypeParsers {
    public abstract class TypeParserBase : ITypeParser {
        protected abstract string Keyword { get; }

        public bool CanParse(string keyword) {
            return keyword.EqualsOrdinalIgnoreCase(this.Keyword);
        }

        public abstract void Parse(string line);
    }
}