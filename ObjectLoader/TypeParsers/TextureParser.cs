using System.Numerics;
using ObjectLoader.Common;
using ObjectLoader.Data.DataStore;
using ObjectLoader.TypeParsers.Interfaces;

namespace ObjectLoader.TypeParsers {
    public class TextureParser : TypeParserBase, ITextureParser {
        private readonly ITextureDataStore _textureDataStore;

        public TextureParser(ITextureDataStore textureDataStore) {
            this._textureDataStore = textureDataStore;
        }

        protected override string Keyword {
            get { return "vt"; }
        }

        public override void Parse(string line) {
            string[] parts = line.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts.Length > 2 ? parts[2].ParseInvariantFloat() : float.NaN;

            this._textureDataStore.AddTexture(new Vector3(x, y, z));
        }
    }
}