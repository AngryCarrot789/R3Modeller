using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace R3Modeller.Core.Engine {
    public class FloorPlaneObject : SceneObject {
        private readonly int vao;
        private readonly int vbo;

        public FloorPlaneObject() {
            this.scale = new Vector3(5f);
            float[] verts = {
                -1.0f,  0.0f,  1.0f,
                 1.0f,  0.0f,  1.0f,
                 1.0f,  0.0f, -1.0f,
                -1.0f,  0.0f,  1.0f,
                 1.0f,  0.0f, -1.0f,
                -1.0f,  0.0f, -1.0f,
            };

            this.vao = GL.GenVertexArray();
            GL.BindVertexArray(this.vao);

            this.vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);

            // Unbind the VAO after configuration.
            GL.BindVertexArray(0);
        }

        public override void Render(Camera camera, Shader shader) {
            Matrix4x4 view = camera.view;
            Matrix4x4 projection = camera.proj;
            Matrix4x4 modelMatrix = MatrixUtils.LocalToWorld(this.pos, this.euler, this.scale);
            Matrix4x4 matrix = modelMatrix * view * projection;
            shader.Use();
            shader.SetUniformVec3("in_color", new Vector3(0.4f, 0.4f, 0.7f));
            shader.SetUniformMatrix4("mvp", ref matrix);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
            GL.VertexAttribPointer(
                0,     // attribute 0. No particular reason for 0, but must match the layout in the shader.
                3,     // size
                VertexAttribPointerType.Float,
                false, // normalized?
                0,     // stride
                0      // array buffer offset
            );

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.DisableVertexAttribArray(0);
        }
    }
}