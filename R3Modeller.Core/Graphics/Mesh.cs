using System.Collections.Generic;
using System.Numerics;

namespace R3Modeller.Core.Graphics {
    public class Mesh {
        private readonly List<Vector3> verts;

        public Mesh(List<Vector3> verts) {
            this.verts = verts;
        }

        public Vector3 GetCenterOfMass() {
            Vector3 vec = new Vector3();
            foreach (Vector3 vert in this.verts)
                vec += vert; // may use SIMD during JIT
            return vec * (1f / this.verts.Count);
        }
    }
}