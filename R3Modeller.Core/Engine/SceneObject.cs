using System.Numerics;
using OpenTK.Graphics.OpenGL;
using Vector3 = System.Numerics.Vector3;

namespace R3Modeller.Core.Engine {
    public class SceneObject {
        public Vector3 pos;
        public Vector3 scale;
        public Vector3 euler;

        private readonly int vao;
        private readonly int vbo;

        public SceneObject() {
            this.pos = Vector3.Zero;
            this.scale = Vector3.One;
            this.euler = Vector3.Zero;

            float[] verts = {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                0.0f,  1.0f, 0.0f,
            };

            this.vao = GL.GenVertexArray();
            GL.BindVertexArray(this.vao);

            this.vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
        }

        public virtual void Render(Camera cam, Shader shader) {
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
            // Draw the triangle !
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3); // Starting from vertex 0; 3 vertices total -> 1 triangle
            GL.DisableVertexAttribArray(0);
        }
    }
}