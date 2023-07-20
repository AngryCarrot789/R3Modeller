using System.Collections.Generic;
using ObjectLoader.Data.DataStore;

namespace ObjectLoader.Data.Elements {
    public class Group : IFaceGroup {
        public readonly List<Face> Faces = new List<Face>();
        public string Name;
        public Material Material;

        public Group(string name) {
            this.Name = name;
        }

        public void AddFace(Face face) => this.Faces.Add(face);
    }
}