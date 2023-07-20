using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace R3Modeller.Core.Engine {
    public class LineObject {
        private readonly Shader shader;
        private readonly int vao;
        private readonly int vbo;

        public LineObject(Vector3 a, Vector3 b) {
            this.shader = new Shader(ResourceLocator.ReadFile("Shaders/LineShader.vert"), ResourceLocator.ReadFile("Shaders/LineShader.frag"));
            this.vao = GL.GenVertexArray();
            this.vbo = GL.GenBuffer();
            GL.BindVertexArray(this.vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6, new[] {
                a.X, a.Y, a.Z,
                b.X, b.Y, b.Z
            }, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void DrawAt(Matrix4x4 mvp, Vector3 colour) {
            this.shader.Use();
            this.shader.SetUniformVec3("in_color", colour);
            this.shader.SetUniformMatrix4("mvp", ref mvp);

            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(this.vao);
            GL.LineWidth(4f);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}