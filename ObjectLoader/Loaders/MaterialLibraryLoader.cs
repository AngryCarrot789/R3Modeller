using System;
using System.Collections.Generic;
using System.IO;
using ObjLoader.Common;
using ObjLoader.Data;
using ObjLoader.Data.DataStore;

namespace ObjLoader.Loaders {
    public class MaterialLibraryLoader : LoaderBase, IMaterialLibraryLoader {
        private readonly IMaterialLibrary _materialLibrary;
        private Material _currentMaterial;

        private readonly Dictionary<string, Action<string>> _parseActionDictionary = new Dictionary<string, Action<string>>();
        private readonly List<string> _unrecognizedLines = new List<string>();

        public MaterialLibraryLoader(IMaterialLibrary materialLibrary) {
            this._materialLibrary = materialLibrary;

            this.AddParseAction("newmtl", this.PushMaterial);
            this.AddParseAction("Ka", d => this.CurrentMaterial.AmbientColor = this.ParseVec3(d));
            this.AddParseAction("Kd", d => this.CurrentMaterial.DiffuseColor = this.ParseVec3(d));
            this.AddParseAction("Ks", d => this.CurrentMaterial.SpecularColor = this.ParseVec3(d));
            this.AddParseAction("Ns", d => this.CurrentMaterial.SpecularCoefficient = d.ParseInvariantFloat());

            this.AddParseAction("d", d => this.CurrentMaterial.Transparency = d.ParseInvariantFloat());
            this.AddParseAction("Tr", d => this.CurrentMaterial.Transparency = d.ParseInvariantFloat());

            this.AddParseAction("illum", i => this.CurrentMaterial.IlluminationModel = i.ParseInvariantInt());

            this.AddParseAction("map_Ka", m => this.CurrentMaterial.AmbientTextureMap = m);
            this.AddParseAction("map_Kd", m => this.CurrentMaterial.DiffuseTextureMap = m);

            this.AddParseAction("map_Ks", m => this.CurrentMaterial.SpecularTextureMap = m);
            this.AddParseAction("map_Ns", m => this.CurrentMaterial.SpecularHighlightTextureMap = m);

            this.AddParseAction("map_d", m => this.CurrentMaterial.AlphaTextureMap = m);

            this.AddParseAction("map_bump", m => this.CurrentMaterial.BumpMap = m);
            this.AddParseAction("bump", m => this.CurrentMaterial.BumpMap = m);

            this.AddParseAction("disp", m => this.CurrentMaterial.DisplacementMap = m);

            this.AddParseAction("decal", m => this.CurrentMaterial.StencilDecalMap = m);
        }

        private Material CurrentMaterial { get { return this._currentMaterial; } }

        private void AddParseAction(string key, Action<string> action) {
            this._parseActionDictionary.Add(key.ToLowerInvariant(), action);
        }

        protected override void ParseLine(string keyword, string data) {
            Action<string> parseAction = this.GetKeywordAction(keyword);

            if (parseAction == null) {
                this._unrecognizedLines.Add(keyword + " " + data);
                return;
            }

            parseAction(data);
        }

        private Action<string> GetKeywordAction(string keyword) {
            Action<string> action;
            this._parseActionDictionary.TryGetValue(keyword.ToLowerInvariant(), out action);

            return action;
        }

        private void PushMaterial(string materialName) {
            this._currentMaterial = new Material(materialName);
            this._materialLibrary.Push(this._currentMaterial);
        }

        private Vec3 ParseVec3(string data) {
            string[] parts = data.Split(' ');

            float x = parts[0].ParseInvariantFloat();
            float y = parts[1].ParseInvariantFloat();
            float z = parts[2].ParseInvariantFloat();

            return new Vec3(x, y, z);
        }

        public void Load(Stream lineStream) {
            this.StartLoad(lineStream);
        }
    }
}