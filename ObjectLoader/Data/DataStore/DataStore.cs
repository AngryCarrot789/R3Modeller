﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ObjectLoader.Common;
using ObjectLoader.Data.Elements;

namespace ObjectLoader.Data.DataStore {
    public class DataStore : IDataStore, IGroupDataStore, IVertexDataStore, ITextureDataStore, INormalDataStore,
        IFaceGroup, IMaterialLibrary, IElementGroup {
        private Group _currentGroup;

        private readonly List<Group> _groups = new List<Group>();
        private readonly List<Material> _materials = new List<Material>();

        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector3> _textures = new List<Vector3>();
        private readonly List<Vector3> _normals = new List<Vector3>();

        public IList<Vector3> Vertices {
            get { return this._vertices; }
        }

        public IList<Vector3> Textures {
            get { return this._textures; }
        }

        public IList<Vector3> Normals {
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

        public void AddVertex(Vector3 vertex) {
            this._vertices.Add(vertex);
        }

        public void AddTexture(Vector3 texture) {
            this._textures.Add(texture);
        }

        public void AddNormal(Vector3 normal) {
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