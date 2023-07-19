using ObjectLoader.Common;
using ObjectLoader.Data.DataStore;
using ObjectLoader.Data.VertexData;
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

            Texture texture = new Texture(x, y);
            this._textureDataStore.AddTexture(texture);
        }
    }
}