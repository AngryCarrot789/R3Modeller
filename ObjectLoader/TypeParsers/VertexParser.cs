using System;
using ObjLoader.Common;
using ObjLoader.Data.DataStore;
using ObjLoader.Data.VertexData;
using ObjLoader.TypeParsers.Interfaces;

namespace ObjLoader.TypeParsers {
    public class VertexParser : TypeParserBase, IVertexParser {
        private readonly IVertexDataStore _vertexDataStore;

        public VertexParser(IVertexDataStore vertexDataStore) {
            this._vertexDataStore = vertexDataStore;
        }

        protected override string Keyword {
            get { return "v"; }
        }

        public override void Parse(string line) {
            string[] parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            Vertex vertex = new Vertex(x, y, z);
            this._vertexDataStore.AddVertex(vertex);
        }
    }
}