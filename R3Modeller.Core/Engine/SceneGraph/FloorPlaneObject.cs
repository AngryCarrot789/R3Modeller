using System.Numerics;
using R3Modeller.Core.Engine.Meshes;

namespace R3Modeller.Core.Engine.SceneGraph {
    public class FloorPlaneObject : SceneObject {
        private readonly BasicMesh mesh;
        private readonly Shader shader;

        public FloorPlaneObject() {
            this.SetScale(new Vector3(5f));
            this.shader = new Shader(ResourceLocator.ReadFile("Shaders/BasicShader.vert"), ResourceLocator.ReadFile("Shaders/BasicShader.frag"));
            this.mesh = new BasicMesh(new[] {
                -1.0f, 0.0f,  1.0f,
                 1.0f, 0.0f,  1.0f,
                 1.0f, 0.0f, -1.0f,
                -1.0f, 0.0f,  1.0f,
                 1.0f, 0.0f, -1.0f,
                -1.0f, 0.0f, -1.0f,
            });
        }

        public override void Render(Camera camera) {
            Matrix4x4 matrix = this.modelMatrix * camera.view * camera.proj;
            this.shader.Use();
            this.shader.SetUniformVec3("in_color", new Vector3(0.4f, 0.4f, 0.7f));
            this.shader.SetUniformMatrix4("mvp", ref matrix);

            this.mesh.DrawTriangles();
        }
    }
}