using System.Numerics;
using OpenTK.Graphics.OpenGL;
using Vector3 = System.Numerics.Vector3;

namespace R3Modeller.Core.Engine {
    public class SceneObject {
        public Vector3 pos;
        public Vector3 scale;
        public Vector3 euler;

        public SceneObject() {
            this.pos = Vector3.Zero;
            this.scale = Vector3.One;
            this.euler = Vector3.Zero;
        }

        public virtual void Render(Camera camera, Shader shader) {

        }
    }
}