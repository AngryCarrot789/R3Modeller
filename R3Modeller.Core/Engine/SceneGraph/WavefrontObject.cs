using System.Collections.Generic;
using System.Numerics;
using ObjectLoader.Data.Elements;
using ObjectLoader.Data.VertexData;
using ObjectLoader.Loaders;
using OpenTK.Graphics.OpenGL;

namespace R3Modeller.Core.Engine.SceneGraph {
    public class WavefrontObject : SceneObject {
        private readonly Shader shader;
        private readonly int vao, vbo, nbo, tbo, ibo;
        private readonly int indicesCount;

        public WavefrontObject(LoadResult result, Group group) {
            this.shader = new Shader(ResourceLocator.ReadFile("Shaders/MeshShader.vert"), ResourceLocator.ReadFile("Shaders/MeshShader.frag"));
            IList<Normal> normList = result.Normals;

            // Define lists to store final vertex data and indices
            List<float> vertexData = new List<float>();
            List<float> texCoordData = new List<float>();
            List<float> normalData = new List<float>();
            List<uint> indices = new List<uint>();

            foreach (var face in group.Faces) {
                foreach (FaceVertex faceVertex in face._vertices) {
                    // Extract vertex data and store in final lists

                    Vector3 v = result.Vertices[faceVertex.VertexIndex - 1];
                    vertexData.Add(v.X);
                    vertexData.Add(v.Y);
                    vertexData.Add(v.Z);

                    Texture uv = result.Textures[faceVertex.TextureIndex - 1];
                    texCoordData.Add(uv.X);
                    texCoordData.Add(uv.Y);

                    Normal norm = normList[faceVertex.NormalIndex - 1];
                    normalData.Add(norm.X);
                    normalData.Add(norm.Y);
                    normalData.Add(norm.Z);

                    // Store the index of the newly added vertex data
                    indices.Add((uint) (vertexData.Count - 1));
                }
            }

            this.indicesCount = indices.Count;

            this.vao = GL.GenVertexArray();
            GL.BindVertexArray(this.vao);

            {
                this.vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Count * sizeof(float), vertexData.ToArray(), BufferUsageHint.StaticDraw);

                // Bind texture coordinate buffer and load texture coordinate data
                this.tbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, texCoordData.Count * sizeof(float), texCoordData.ToArray(), BufferUsageHint.StaticDraw);

                // Bind normal buffer and load normal data
                this.nbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, normalData.Count * sizeof(float), normalData.ToArray(), BufferUsageHint.StaticDraw);

                // Bind index buffer and load index data
                this.ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            {
                // Bind vertex buffer and set the attribute pointer for vertices
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                // Bind texture coordinate buffer and set the attribute pointer for texture coordinates
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.tbo);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                // Bind normal buffer and set the attribute pointer for normals
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
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            // GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            this.RenderChildren(camera);
        }
    }
}