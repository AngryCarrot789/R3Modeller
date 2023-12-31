using System;
using System.Collections.Generic;
using System.Numerics;
using ObjectLoader.Data.Elements;
using ObjectLoader.Loaders;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Core.Engine.Utils;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine.Objs {
    public class WavefrontObject : SceneObject {
        private readonly Shader shader;
        private readonly int vao, vbo, nbo, tbo, ibo;
        private readonly int indicesCount;
        private readonly LoadResult load;
        private readonly Group group;
        private readonly bool is3dTexture;

        public WavefrontObject(LoadResult result, Group group) {
            this.load = result;
            this.group = group;

            this.shader = new Shader(ResourceLocator.ReadFile("Shaders/MeshShader.vert"), ResourceLocator.ReadFile("Shaders/MeshShader.frag"));

            List<float> vertices = new List<float>();
            List<float> texCoordData = new List<float>();
            List<float> normalData = new List<float>();
            List<uint> indices = new List<uint>();
            foreach (Vector3 v in result.Vertices) {
                vertices.Add(v.X);
                vertices.Add(v.Y);
                vertices.Add(v.Z);
            }

            foreach (Vector3 v in result.Normals) {
                normalData.Add(v.X);
                normalData.Add(v.Y);
                normalData.Add(v.Z);
            }

            foreach (Vector3 v in result.Textures) {
                texCoordData.Add(v.X);
                texCoordData.Add(v.Y);
                if (!float.IsNaN(v.Z)) {
                    texCoordData.Add(v.Y);
                    this.is3dTexture = true;
                }
            }

            bool isQuad = false;
            foreach (Face face in group.Faces) {
                if (face.Vertices.Count > 3) {
                    isQuad = true;
                }

                foreach (FaceVertex vertex in face.Vertices) {
                    indices.Add((uint) (vertex.VertexIndex - 1));
                }
            }

            if (isQuad) {
                uint[] quadIndices = indices.ToArray();
                indices.Clear();
                for (int i = 0; i < quadIndices.Length; i += 4) {
                    // Triangle 1 of the face
                    indices.Add(quadIndices[i]);
                    indices.Add(quadIndices[i + 1]);
                    indices.Add(quadIndices[i + 2]);

                    // Triangle 2 of the face
                    indices.Add(quadIndices[i]);
                    indices.Add(quadIndices[i + 2]);
                    indices.Add(quadIndices[i + 3]);
                }
            }

            this.indicesCount = indices.Count;

            this.vao = GL.GenVertexArray();
            GL.BindVertexArray(this.vao);

            this.vbo = GL.GenBuffer();
            this.tbo = GL.GenBuffer();
            this.nbo = GL.GenBuffer();
            this.ibo = GL.GenBuffer();

            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, this.vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, texCoordData.Count * sizeof(float), texCoordData.ToArray(), BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(1, this.is3dTexture ? 3: 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, this.nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, normalData.Count * sizeof(float), normalData.ToArray(), BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);
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
            this.shader.SetUniformBool("use_colour", true);
            this.shader.SetUniformVec3("in_color", new Vector3(0.6f, 0.2f, 0.3f));

            // // Draw mesh
            // GL.BindVertexArray(this.vao);
            // GL.DrawArrays(PrimitiveType.Quads, 0, this.indicesCount);
            // GL.BindVertexArray(0);

            GL.BindVertexArray(this.vao);
            GL.DrawElements(PrimitiveType.Triangles, this.indicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            if (this.IsObjectSelected) {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                this.shader.SetUniformVec3("in_color", new Vector3(1f, 1f, 1f));
                GL.DrawElements(PrimitiveType.Triangles, this.indicesCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            GL.BindVertexArray(0);

            this.RenderChildren(camera);
        }

        protected override void DisposeCore(ExceptionStack stack) {
            base.DisposeCore(stack);
            this.shader.Dispose();
            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.vbo);
            GL.DeleteBuffer(this.nbo);
            GL.DeleteBuffer(this.tbo);
            GL.DeleteBuffer(this.ibo);
        }
    }
}