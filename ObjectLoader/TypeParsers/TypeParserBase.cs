using ObjLoader.Common;
using ObjLoader.TypeParsers.Interfaces;

namespace ObjLoader.TypeParsers {
    public abstract class TypeParserBase : ITypeParser {
        protected abstract string Keyword { get; }

        public bool CanParse(string keyword) {
            return keyword.EqualsOrdinalIgnoreCase(this.Keyword);
        }

        public abstract void Parse(string line);
    }
}