using System;
using System.Numerics;
using R3Modeller.Controls;
using System.Windows;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Core;
using R3Modeller.Core.Engine;
using R3Modeller.Core.Utils;
using R3Modeller.Views;

namespace R3Modeller {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx {
        private readonly OGLContextWrapper ogl;
        // private readonly uint vao;
        // private readonly float[] verts;

        private Vector3 camPos;
        private Vector2 camLook;
        private readonly Camera camera;
        private readonly Shader shader;
        private readonly SceneObject triangle;

        private float mX, mY;

        private bool isOrbitActive;

        public MainWindow(OGLContextWrapper ogl) {
            this.ogl = ogl;
            this.InitializeComponent();
            this.ogl.MakeCurrent(true);

            GL.ClearColor(0.2f, 0.4f, 0.8f, 1.0f);
            // GL.Enable(EnableCap.CullFace);
            // GL.CullFace(CullFaceMode.Back);
            // GL.Enable(EnableCap.DepthTest);
            // GL.DepthFunc(DepthFunction.Less);
            // GL.DepthMask(true);

            this.camera = new Camera();
            this.triangle = new SceneObject();
            this.shader = new Shader(ResourceLocator.ReadFile("Shaders/BasicShader.vert"), ResourceLocator.ReadFile("Shaders/BasicShader.frag"));

            #region obj loader

            // string vertexShader =
            //     "#version 330\n" +
            //     "uniform mat4 mvp;\n" +
            //     "in vec3 in_pos;\n" +
            //     "void main() { gl_Position = mvp * vec4(in_pos, 1.0); }";
            // string fragmentShader =
            //     "#version 330\n" +
            //     "void main() { gl_FragColor = vec4(0.8, 0.2, 1.0, 1.0); }\n";
            // this.shader = new Shader(vertexShader, fragmentShader);

            // ObjLoaderFactory factory = new ObjLoaderFactory();
            // IObjLoader loader = factory.Create(new MaterialStreamProvider("F:\\VSProjsV2\\R3Modeller\\Resources"));
            // using (FileStream stream = File.OpenRead("F:\\VSProjsV2\\R3Modeller\\Resources\\untitled.obj")) {
            //     // LoadResult result = loader.Load(stream);
            //     // List<float> v = new List<float>();
            //     // foreach (Vector3 vertex in result.Vertices) {
            //     //     v.Add(vertex.X);
            //     //     v.Add(vertex.Y);
            //     //     v.Add(vertex.Z);
            //     // }
            //     uint VBO, VAO;
            //     GL.GenVertexArrays(1, &VAO);
            //     GL.BindVertexArray(VAO);
            //     GL.GenBuffers(1, &VBO);
            //     GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            //     GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * this.verts.Length, this.verts, BufferUsageHint.StaticDraw);
            //     GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), IntPtr.Zero);
            //     GL.EnableVertexAttribArray(0);
            //     GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //     GL.BindVertexArray(0);
            //     this.vao = VAO;
            // }

            #endregion

            this.ogl.MakeCurrent(false);

            this.camPos = new Vector3(4, 3, 3);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
            switch (e.Key) {
                case Key.W: this.camPos.Z -= 0.1f; break;
                case Key.A: this.camPos.X -= 0.1f; break;
                case Key.S: this.camPos.Z += 0.1f; break;
                case Key.D: this.camPos.X += 0.1f; break;
                case Key.Space:     this.camPos.Y += 0.1f; break;
                case Key.LeftShift: this.camPos.Y -= 0.1f; break;
                case Key.Left:  this.camLook.X -= 0.1f; break;
                case Key.Right: this.camLook.X += 0.1f; break;
                case Key.Up:    this.camLook.Y += 0.1f; break;
                case Key.Down:  this.camLook.Y -= 0.1f; break;
                case Key.System: {
                    if (!e.IsRepeat && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt) {
                        this.isOrbitActive = true;
                    }

                    break;
                }
                default: return;
            }

            this.OGLViewPort.InvalidateVisual();
            this.POS_LABEL.Content = $"Pos: \t{Math.Round(this.camPos.X, 2):F2} \t{Math.Round(this.camPos.Y, 2):F2} \t{Math.Round(this.camPos.Z, 2):F2}";
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            switch (e.Key) {
                case Key.System: {
                    if (!e.IsRepeat && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt) {
                        this.isOrbitActive = false;
                    }

                    break;
                }
                default: return;
            }

            this.OGLViewPort.InvalidateVisual();
        }

        private Point? lastMouse;

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            Point currPos = e.GetPosition(this);
            if (this.lastMouse is Point lastPos && e.LeftButton == MouseButtonState.Pressed) {
                float changeX = (float) Maths.Map(currPos.X - lastPos.X, 0d, this.ActualWidth, -1d, 1d);
                float changeY = (float) Maths.Map(currPos.Y - lastPos.Y, 0d, this.ActualHeight, 1d, -1d);

                this.mX = (float) Maths.Map(currPos.X, 0d, this.ActualWidth, -1d, 1d);
                this.mY = (float) Maths.Map(currPos.Y, 0d, this.ActualHeight, 1d, -1d);

                // this.camLook.Y -= (1f - changeX);
                // if (this.camLook.Y > Maths.PI) {
                //     this.camLook.Y = Maths.PI_NEG + 0.0001f;
                // }
                // else if (this.camLook.Y < Maths.PI_NEG) {
                //     this.camLook.Y = Maths.PI - 0.0001f;
                // }
                // this.camLook.X -= (1f - changeY);
                // if (this.camLook.X > Maths.PI_HALF) {
                //     this.camLook.X = Maths.PI_HALF;
                // }
                // else if (this.camLook.X < Maths.PI_NEG / 2) {
                //     this.camLook.X = Maths.PI_NEG_HALF;
                // }
            }

            this.lastMouse = currPos;
            this.OGLViewPort.InvalidateVisual();
        }

        // TODO: Could have ViewPortViewModel, which stores a reference to an interface (implemented by an
        // OpenGL control which references the SceneViewModel so that it can draw the objects, selection, etc)
        // Those are just ideas so far

        private void OnPaintViewPort(object sender, DrawEventArgs e) {
            this.ogl.MakeCurrent(true);

            // Update hidden window, if the size has changed
            this.ogl.UpdateSize(e.Width, e.Height);
            this.camera.UpdateSize(e.Width, e.Height);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            {
                this.shader.Use();
                Matrix4x4 modelMatrix = MatrixUtils.LocalToWorld(new Vector3(this.mX, this.mY, 0f), Vector3.Zero, new Vector3(1f));

                Matrix4x4 view = Matrix4x4.Identity;
                {
                    Vector3 eye = this.camPos;// new Vector3(4f, 3f, 3f);
                    Vector3 center = new Vector3(0f);
                    Vector3 up = new Vector3(0f, 1f, 0f);
                    Vector3 f = Vector3.Normalize(center - eye);
                    Vector3 s = Vector3.Normalize(Vector3.Cross(f, up));
                    Vector3 u = Vector3.Cross(s, f);
                    view.M11 =  s.X;
                    view.M12 =  u.X;
                    view.M13 = -f.X;
                    view.M21 =  s.Y;
                    view.M22 =  u.Y;
                    view.M23 = -f.Y;
                    view.M31 =  s.Z;
                    view.M32 =  u.Z;
                    view.M33 = -f.Z;
                    view.M41 = -Vector3.Dot(s, eye);
                    view.M42 = -Vector3.Dot(u, eye);
                    view.M43 =  Vector3.Dot(f, eye);
                }

                Matrix4x4 projection = Matrix4x4.Identity;
                {
                    float rad = Maths.DegreesToRadians(90f);
                    float aspect = (float) e.Width / e.Height;
                    float tanHalfFovy = (float) Math.Tan(rad / 2);
                    const float zNear = 0.01f;
                    const float zFar = 1000f;
                    projection.M11 = 1 / (aspect * tanHalfFovy);
                    projection.M22 = 1 / (tanHalfFovy);
                    projection.M33 = - (zFar + zNear) / (zFar - zNear);
                    projection.M34 = - 1;
                    projection.M43 = - (2 * zFar * zNear) / (zFar - zNear);
                }

                // Matrix4x4 matrix = projection * view * modelMatrix;
                Matrix4x4 matrix = modelMatrix * view * projection;
                this.shader.SetUniformMatrix4("mat", ref matrix);
                this.triangle.Render(this.camera, this.shader);

                Vector4 vec1 = Vector4.Transform(new Vector3(-1.0f, -1.0f, 0.0f), matrix);
                Vector4 vec2 = Vector4.Transform(new Vector3(1.0f, -1.0f, 0.0f), matrix);
                Vector4 vec3 = Vector4.Transform(new Vector3(0.0f,  1.0f, 0.0f), matrix);
            }

            // GL.UseProgram(0); // Use your shader program
            // GL.BindVertexArray(this.vao); // Bind the VAO containing cube vertices
            // GL.DrawArrays(PrimitiveType.Triangles, 0, this.verts.Length / 3);
            // GL.BindVertexArray(0); // Unbind VAO

            // read pixels from OpenGL's back buffer, then write into draw event's back buffer (aka the WPF bitmap)
            // The resulting pixels will be flipped vertically, because OpenGL origin direction is bottom-left, where as
            // windowing apps (like WPF) use the top left.
            // However the LayoutTransform property of the control can be used to scale the layout by -1 in the Y axis to flip
            // it back. It's most likely faster than doing a manual pixel copy, because the layout transformation (i'm guessing) is happening in DirectX

            GL.Flush();
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.ReadPixels(0, 0, e.Width, e.Height, PixelFormat.Bgra, PixelType.UnsignedByte, e.BackBuffer);
            this.ogl.MakeCurrent(false);

            // draw into an offscreen buffer
            // read pixels from OpenGL and paste them into the bitmap
        }
    }
}