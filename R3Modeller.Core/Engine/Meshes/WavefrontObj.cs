using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace R3Modeller.Core.Engine.Meshes {
    public class WavefrontObj {
        public struct Face {
            public int Vert; // vertex index
            public int UV;   // texture index
            public int Norm; // normal index

            public Face(int vert, int uv, int norm) {
                this.Vert = vert;
                this.UV = uv;
                this.Norm = norm;
            }
        }

        public Vector3[] vertices;
        public Vector3[] colours;
        public Vector2[] uvs;
        public Face[] faces;

        public WavefrontObj() {
        }

        public int[] GetIndices(int offset) {
            List<int> temp = new List<int>();
            foreach (Face face in this.faces) {
                temp.Add(face.Vert + offset);
                temp.Add(face.UV + offset);
                temp.Add(face.Norm + offset);
            }

            return temp.ToArray();
        }

        public static WavefrontObj FromFile(string objFile, string mtlFile = null) {
            return Parse(File.ReadAllLines(objFile), mtlFile != null ? File.ReadAllLines(mtlFile) : null);
        }

        public static WavefrontObj Parse(string[] obj, string[] mtl = null) {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> colors = new List<Vector3>();
            List<Vector2> texs = new List<Vector2>();
            List<Face> faces = new List<Face>();

            foreach (string line in obj) {
                if (line.StartsWith("v ")) {
                    string temp = line.Substring(2);
                    Vector3 vec = new Vector3();
                    if (temp.Count(c => c == ' ') == 2) {
                        string[] vertparts = temp.Split(' ');
                        if (!float.TryParse(vertparts[0], out vec.X) || !float.TryParse(vertparts[1], out vec.Y) || !float.TryParse(vertparts[2], out vec.Z)) {
                            throw new Exception($"Invalid vertex line: {line}");
                        }

                        colors.Add(new Vector3((float) Math.Sin(vec.Z), (float) Math.Sin(vec.Z), (float) Math.Sin(vec.Z)));
                        texs.Add(new Vector2((float) Math.Sin(vec.Z), (float) Math.Sin(vec.Z)));
                    }

                    verts.Add(vec);
                }
                else if (line.StartsWith("f ")) {
                    string temp = line.Substring(2);
                    if (temp.Count(c => c == ' ') == 2) {
                        string[] faceparts = temp.Split(' ');
                        if (!int.TryParse(faceparts[0], out int a) || !int.TryParse(faceparts[1], out int b) || !int.TryParse(faceparts[2], out int c)) {
                            throw new Exception($"Invalid face line: {line}");
                        }

                        faces.Add(new Face(a - 1, b - 1, c - 1));
                    }
                }
            }

            WavefrontObj vol = new WavefrontObj {
                vertices = verts.ToArray(),
                faces = faces.ToArray(),
                colours = colors.ToArray(),
                uvs = texs.ToArray()
            };
            return vol;
        }
    }
}