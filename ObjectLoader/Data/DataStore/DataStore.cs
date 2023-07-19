using System.Collections.Generic;
using System.Linq;
using ObjLoader.Common;
using ObjLoader.Data.Elements;
using ObjLoader.Data.VertexData;

namespace ObjLoader.Data.DataStore {
    public class DataStore : IDataStore, IGroupDataStore, IVertexDataStore, ITextureDataStore, INormalDataStore,
        IFaceGroup, IMaterialLibrary, IElementGroup {
        private Group _currentGroup;

        private readonly List<Group> _groups = new List<Group>();
        private readonly List<Material> _materials = new List<Material>();

        private readonly List<Vertex> _vertices = new List<Vertex>();
        private readonly List<Texture> _textures = new List<Texture>();
        private readonly List<Normal> _normals = new List<Normal>();

        public IList<Vertex> Vertices {
            get { return this._vertices; }
        }

        public IList<Texture> Textures {
            get { return this._textures; }
        }

        public IList<Normal> Normals {
            get { return this._normals; }
        }

        public IList<Material> Materials {
            get { return this._materials; }
        }

        public IList<Group> Groups {
            get { return this._groups; }
        }

        public void AddFace(Face face) {
            this.PushGroupIfNeeded();

            this._currentGroup.AddFace(face);
        }

        public void PushGroup(string groupName) {
            this._currentGroup = new Group(groupName);
            this._groups.Add(this._currentGroup);
        }

        private void PushGroupIfNeeded() {
            if (this._currentGroup == null) {
                this.PushGroup("default");
            }
        }

        public void AddVertex(Vertex vertex) {
            this._vertices.Add(vertex);
        }

        public void AddTexture(Texture texture) {
            this._textures.Add(texture);
        }

        public void AddNormal(Normal normal) {
            this._normals.Add(normal);
        }

        public void Push(Material material) {
            this._materials.Add(material);
        }

        public void SetMaterial(string materialName) {
            Material material = this._materials.SingleOrDefault(x => x.Name.EqualsOrdinalIgnoreCase(materialName));
            this.PushGroupIfNeeded();
            this._currentGroup.Material = material;
        }
    }
}