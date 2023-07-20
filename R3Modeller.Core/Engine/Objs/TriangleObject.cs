using System.Numerics;
using R3Modeller.Core.Engine.Meshes;
using R3Modeller.Core.Engine.Utils;

namespace R3Modeller.Core.Engine.Objs {
    public class TriangleObject : SceneObject {
        private readonly BasicMesh mesh;
        private readonly Shader shader;

        public TriangleObject() {
            this.shader = new Shader(ResourceLocator.ReadFile("Shaders/BasicShader.vert"), ResourceLocator.ReadFile("Shaders/BasicShader.frag"));
            this.mesh = new BasicMesh(new[] {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                0.0f,  1.0f, 0.0f,
            });
        }

        public override void Render(Camera camera) {
            Matrix4x4 view = camera.view;
            Matrix4x4 projection = camera.proj;

            // Calculates model-view-projection matrix
            Matrix4x4 matrix = this.modelMatrix * view * projection;

            // Upload the final matrix to shader
            this.shader.Use();
            this.shader.SetUniformVec3("in_color", new Vector3(0.4f, 0.7f, 0.8f));
            this.shader.SetUniformMatrix4("mvp", ref matrix);

            // Draw mesh
            this.mesh.DrawTriangles();
            this.RenderChildren(camera);
        }
    }
}