using System.Collections.Generic;
using ObjLoader.Data.DataStore;

namespace ObjLoader.Data.Elements {
    public class Group : IFaceGroup {
        private readonly List<Face> _faces = new List<Face>();

        public Group(string name) {
            this.Name = name;
        }

        public string Name { get; private set; }
        public Material Material { get; set; }

        public IList<Face> Faces { get { return this._faces; } }

        public void AddFace(Face face) {
            this._faces.Add(face);
        }
    }
}