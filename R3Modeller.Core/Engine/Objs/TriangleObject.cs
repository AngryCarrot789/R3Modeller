using System.Numerics;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Core.Engine.Meshes;
using R3Modeller.Core.Engine.Utils;
using R3Modeller.Core.Utils;

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
            // Calculates model-view-projection matrix
            Matrix4x4 matrix = this.modelMatrix * camera.view * camera.proj;

            // Upload the final matrix to shader
            this.shader.Use();
            this.shader.SetUniformVec3("in_color", new Vector3(0.4f, 0.7f, 0.8f));
            this.shader.SetUniformMatrix4("mvp", ref matrix);

            // Draw mesh
            this.mesh.DrawTriangles();

            if (this.IsObjectSelected) {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                this.shader.SetUniformVec3("in_color", new Vector3(1f, 1f, 1f));
                this.mesh.DrawTriangles();
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            this.RenderChildren(camera);
        }

        protected override void DisposeCore(ExceptionStack stack) {
            base.DisposeCore(stack);
            this.shader.Dispose();
            this.mesh.Dispose();
        }
    }
}