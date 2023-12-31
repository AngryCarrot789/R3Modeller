using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine.Meshes {
    /// <summary>
    /// A class for storing a basic mesh that only stores coordinates
    /// </summary>
    public class BasicMesh : IDisposable {
        private readonly float[] verts;
        private readonly int vertices;
        private readonly int vao;
        private readonly int vbo;

        public BasicMesh(float[] verts) {
            if (verts.Length % 3 != 0) {
                throw new Exception("Vertices count must be divisible by 3");
            }

            this.verts = verts.Copy();
            this.vertices = verts.Length / 3;
            this.vao = GL.GenVertexArray();
            GL.BindVertexArray(this.vao);

            this.vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            // Unbind the VAO after configuration.
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void DrawTriangles() => this.DrawTriangles(0, this.vertices);

        public void DrawTriangles(int offset, int count) {
            GL.BindVertexArray(this.vao);
            GL.DrawArrays(PrimitiveType.Triangles, offset, count);
            GL.BindVertexArray(0);
        }

        public void Dispose() {
            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.vbo);
        }
    }
}