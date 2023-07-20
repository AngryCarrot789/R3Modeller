using System;
using System.Collections.Generic;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace R3Modeller.Core.Engine {
    public class Shader : IDisposable {
        private readonly Dictionary<string, int> uniformNameToId;
        private readonly Dictionary<int, string> uniformIdToName;
        private readonly Dictionary<string, int> attributeNameToId;
        private readonly Dictionary<int, string> attributeIdToName;

        /// <summary>
        /// The ID of the shader program
        /// </summary>
        public int ProgramID { get; private set; }

        public IReadOnlyDictionary<string, int> UniformNameToId => this.uniformNameToId;
        public IReadOnlyDictionary<int, string> UniformIdToName => this.uniformIdToName;
        public IReadOnlyDictionary<string, int> AttributeNameToId => this.attributeNameToId;
        public IReadOnlyDictionary<int, string> AttributeIdToName => this.attributeIdToName;

        public Shader(string vertexCode, string fragmentCode) {
            this.uniformNameToId = new Dictionary<string, int>();
            this.uniformIdToName = new Dictionary<int, string>();
            this.attributeNameToId = new Dictionary<string, int>();
            this.attributeIdToName = new Dictionary<int, string>();

            this.ProgramID = GL.CreateProgram();

            int vertId = LoadShader(this.ProgramID, vertexCode, ShaderType.VertexShader);
            int fragId = LoadShader(this.ProgramID, fragmentCode, ShaderType.FragmentShader);

            GL.LinkProgram(this.ProgramID);

            // Check if linked
            GL.GetProgram(this.ProgramID, GetProgramParameterName.LinkStatus, out int linked);
            if (linked < 1) {
                GL.GetProgramInfoLog(this.ProgramID, out string info);
                GL.DeleteProgram(this.ProgramID);
                throw new Exception($"Failed to link shader program :\n{info}");
            }

            GL.GetProgram(this.ProgramID, GetProgramParameterName.ActiveUniforms, out int count);
            for (int i = 0; i < count; i++) {
                GL.GetActiveUniform(this.ProgramID, i, 16, out int length, out int size, out ActiveUniformType uniformType, out string name);
                this.uniformNameToId[name] = i;
                this.uniformIdToName[i] = name;
            }

            GL.GetProgram(this.ProgramID, GetProgramParameterName.ActiveAttributes, out count);
            for (int i = 0; i < count; i++) {
                GL.GetActiveAttrib(this.ProgramID, i, 16, out int length, out int size, out ActiveAttribType type, out string name);
                this.attributeNameToId[name] = i;
                this.attributeIdToName[i] = name;
            }

            GL.DetachShader(this.ProgramID, vertId);
            GL.DetachShader(this.ProgramID, fragId);
            GL.DeleteShader(vertId);
            GL.DeleteShader(fragId);
        }

        // Loads the shaders into the main program
        public static int LoadShader(int programId, string code, ShaderType type) {
            int id = GL.CreateShader(type);
            GL.ShaderSource(id, code);
            GL.CompileShader(id);

            // Check if it compiled
            GL.GetShader(id, ShaderParameter.CompileStatus, out int isCompiled);
            if (isCompiled < 1) {
                string shaderType = type == ShaderType.VertexShader ? "Vertex" : (type == ShaderType.FragmentShader ? "Fragment" : "<Unknown>");
                GL.GetShaderInfoLog(id, out string info);
                GL.DeleteProgram(programId);
                throw new Exception($"Failed to compile {shaderType} shader: {info}");
            }

            GL.AttachShader(programId, id);
            return id;
        }

        public bool GetUniformLocation(string name, out int index) => this.uniformNameToId.TryGetValue(name, out index);

        public void SetUniformBool(string name, bool value) {
            if (this.GetUniformLocation(name, out int index))
                GL.Uniform1(index, value ? 1 : 0);
        }

        public void SetUniformInt(string name, int value) {
            if (this.GetUniformLocation(name, out int index))
                GL.Uniform1(index, value);
        }

        public void SetUniformFloat(string name, float value) {
            if (this.GetUniformLocation(name, out int index))
                GL.Uniform1(index, value);
        }

        public void SetUniformVec2(string name, Vector2 value) {
            if (this.GetUniformLocation(name, out int index))
                GL.Uniform2(index, value.X, value.Y);
        }

        public void SetUniformVec2(int location, Vector2 value) {
            GL.Uniform2(location, value.X, value.Y);
        }

        public static void SetUniformVec2(int location, float x, float z) {
            GL.Uniform2(location, x, z);
        }

        public void SetUniformVec3(string name, Vector3 value) {
            if (this.GetUniformLocation(name, out int index))
                GL.Uniform3(index, value.X, value.Y, value.Z);
        }

        public void SetUniformVec4(string name, Vector4 value) {
            if (this.GetUniformLocation(name, out int index))
                GL.Uniform4(index, value.X, value.Y, value.Z, value.W);
        }

        public void SetUniformMatrix4(string name, ref Matrix4x4 value) {
            if (this.GetUniformLocation(name, out int index)) {
                GL.UniformMatrix4(index, 1, false, ref value.M11);
            }
        }

        public unsafe void SetUniformMatrix4(string name, float* ptr) {
            if (this.GetUniformLocation(name, out int index)) {
                GL.UniformMatrix4(index, 1, false, ptr);
            }
        }

        public static void SetUniformMatrix4(int location, ref Matrix4x4 value) {
            GL.UniformMatrix4(location, 1, false, ref value.M11);
        }

        public void Use() {
            GL.UseProgram(this.ProgramID);
        }

        public void Dispose() {
            GL.DeleteProgram(this.ProgramID);
        }
    }
}