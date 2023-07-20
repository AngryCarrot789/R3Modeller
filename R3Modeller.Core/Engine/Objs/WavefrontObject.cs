using System.Collections.Generic;
using System.Numerics;
using ObjectLoader.Data.Elements;
using ObjectLoader.Data.VertexData;
using ObjectLoader.Loaders;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Core.Engine.Utils;

namespace R3Modeller.Core.Engine.Objs {
    public class WavefrontObject : SceneObject {
        private readonly Shader shader;
        private readonly int vao, vbo, nbo, tbo, ibo;
        private readonly int indicesCount;
        private readonly LoadResult load;
        private readonly Group group;

        public WavefrontObject(LoadResult result, Group group) {
            this.load = result;
            this.group = group;

            this.shader = new Shader(ResourceLocator.ReadFile("Shaders/MeshShader.vert"), ResourceLocator.ReadFile("Shaders/MeshShader.frag"));

            List<float> vertexData = new List<float>();
            List<float> texCoordData = new List<float>();
            List<float> normalData = new List<float>();
            List<uint> indices = new List<uint>();

            foreach (Face face in group.Faces) {
                foreach (FaceVertex faceVertex in face.Vertices) {
                    Vector3 v = result.Vertices[faceVertex.VertexIndex - 1];
                    vertexData.Add(v.X);
                    vertexData.Add(v.Y);
                    vertexData.Add(v.Z);

                    Texture uv = result.Textures[faceVertex.TextureIndex - 1];
                    texCoordData.Add(uv.X);
                    texCoordData.Add(uv.Y);

                    Normal norm = result.Normals[faceVertex.NormalIndex - 1];
                    normalData.Add(norm.X);
                    normalData.Add(norm.Y);
                    normalData.Add(norm.Z);

                    indices.Add((uint) (faceVertex.VertexIndex - 1));
                }
            }

            this.indicesCount = indices.Count;

            this.vao = GL.GenVertexArray();
            GL.BindVertexArray(this.vao);

            {
                this.vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Count * sizeof(float), vertexData.ToArray(), BufferUsageHint.StaticDraw);

                this.tbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, texCoordData.Count * sizeof(float), texCoordData.ToArray(), BufferUsageHint.StaticDraw);

                this.nbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, normalData.Count * sizeof(float), normalData.ToArray(), BufferUsageHint.StaticDraw);

                this.ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.tbo);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.nbo);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);
            }

            // Unbind the VAO after configuration.
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public override void Render(Camera camera) {
            Matrix4x4 view = camera.view;
            Matrix4x4 projection = camera.proj;

            Matrix4x4 mv = this.modelMatrix;
            // Calculates model-view-projection matrix
            Matrix4x4 mvp = mv * view * projection;

            // Upload the final matrix to shader
            this.shader.Use();
            this.shader.SetUniformMatrix4("mv", ref mv);
            this.shader.SetUniformMatrix4("mvp", ref mvp);

            // Draw mesh
            GL.BindVertexArray(this.vao);
            GL.DrawArrays(PrimitiveType.Quads, 0, this.indicesCount);
            // GL.DrawElements(PrimitiveType.Quads, this.indicesCount, DrawElementsType.UnsignedInt, 0);

            this.RenderChildren(camera);
        }
    }
}