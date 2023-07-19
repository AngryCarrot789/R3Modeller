using ObjLoader.Common;
using ObjLoader.Data.DataStore;
using ObjLoader.Data.VertexData;
using ObjLoader.TypeParsers.Interfaces;

namespace ObjLoader.TypeParsers {
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