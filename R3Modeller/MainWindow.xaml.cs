using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using R3Modeller.Controls;
using System.Windows;
using System.Windows.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using R3Modeller.Core;
using R3Modeller.Core.Engine;
using R3Modeller.Core.Engine.SceneGraph;
using R3Modeller.Core.Utils;
using R3Modeller.Views;
using Vector3 = System.Numerics.Vector3;

namespace R3Modeller {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx {
        private readonly OGLContextWrapper ogl;
        // private readonly uint vao;
        // private readonly float[] verts;

        private readonly Camera camera;
        private readonly TriangleObject triangle;
        private readonly FloorPlaneObject floor;

        private readonly LineObject lineX;
        private readonly LineObject lineY;
        private readonly LineObject lineZ;

        private bool isOrbitActive;

        public MainWindow(OGLContextWrapper ogl) {
            this.ogl = ogl;
            this.InitializeComponent();
            this.ogl.MakeCurrent(true);

            GL.ClearColor(0.2f, 0.4f, 0.8f, 1.0f);
            // disabled so that both faces are rendered
            // GL.Enable(EnableCap.CullFace);
            // GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);

            this.camera = new Camera();
            this.camera.SetYawPitch(0.45f, -0.35f);
            this.triangle = new TriangleObject();

            TriangleObject tri = new TriangleObject();
            tri.SetPosition(new Vector3(3f, 2f, 3f));
            this.triangle.AddChild(tri);

            this.floor = new FloorPlaneObject();
            this.lineX = new LineObject(new Vector3(), new Vector3(1f, 0f, 0f));
            this.lineY = new LineObject(new Vector3(), new Vector3(0f, 1f, 0f));
            this.lineZ = new LineObject(new Vector3(), new Vector3(0f, 0f, 1f));

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
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            base.OnPreviewKeyDown(e);
            Vector3 pos = this.camera.target;
            switch (e.Key) {
                case Key.W: pos.Z -= 0.1f; break;
                case Key.A: pos.X -= 0.1f; break;
                case Key.S: pos.Z += 0.1f; break;
                case Key.D: pos.X += 0.1f; break;
                case Key.Space:     pos.Y += 0.1f; break;
                case Key.LeftShift: pos.Y -= 0.1f; break;
                case Key.System: {
                    if (!e.IsRepeat && e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt) {
                        this.isOrbitActive = true;
                    }

                    break;
                }
                default: return;
            }

            this.camera.SetTarget(pos);
            this.UpdateTextInfo();
            this.OGLViewPort.InvalidateVisual();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
            base.OnPreviewMouseWheel(e);
            if (e.Delta == 0) {
                return;
            }

            float oldRange = this.camera.orbitRange;
            float multiplier = e.Delta > 0 ? (1f - 0.25f) : (1f + 0.25f);
            float newRange = Maths.Clamp(oldRange * multiplier, 2f, 750f);
            if (Math.Abs(newRange - oldRange) > 0.001f) {
                this.camera.SetOrbitRange(newRange);
                this.OGLViewPort.InvalidateVisual();
            }
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            this.Focus();
            this.CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
            this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            Point currPos = e.GetPosition(this);
            if (this.lastMouse is Point lastPos && this.isOrbitActive) {
                float changeX = 1f + (float) Maths.Map(currPos.X - lastPos.X, 0d, this.ActualWidth, -1d, 1d);
                float changeY = 1f - (float) Maths.Map(currPos.Y - lastPos.Y, 0d, this.ActualHeight, 1d, -1d);

                const float sensitivity = 1.75f;
                const float epsilon = 0.00001f;
                if (e.LeftButton == MouseButtonState.Pressed) {
                    float yaw = this.camera.yaw;
                    float pitch = this.camera.pitch;
                    yaw -= (changeX * sensitivity);
                    if (yaw > Maths.PI) {
                        yaw = Maths.PI_NEG + epsilon;
                    }
                    else if (yaw < Maths.PI_NEG) {
                        yaw = Maths.PI - epsilon;
                    }

                    pitch -= (changeY * sensitivity);
                    if (pitch > Maths.PI_HALF) {
                        pitch = Maths.PI_HALF - epsilon;
                    }
                    else if (pitch < Maths.PI_NEG_HALF) {
                        pitch = Maths.PI_NEG_HALF + epsilon;
                    }

                    this.camera.SetYawPitch(yaw, pitch);
                    this.UpdateTextInfo();
                    this.OGLViewPort.InvalidateVisual();
                }
                else if (e.RightButton == MouseButtonState.Pressed) {
                    this.camera.SetTarget(this.camera.target - new Vector3(changeX * sensitivity * (this.camera.orbitRange / 2f), 0f, changeY * sensitivity * (this.camera.orbitRange / 2f)));
                    this.UpdateTextInfo();
                    this.OGLViewPort.InvalidateVisual();
                }
            }

            this.lastMouse = currPos;
        }

        public void UpdateTextInfo() {
            Vector3 tgt = this.camera.target;
            this.POS_LABEL.Content = $"Target Pos: \t{Math.Round(tgt.X, 2):F2} \t{Math.Round(tgt.Y, 2):F2} \t{Math.Round(tgt.Z, 2):F2}";
            this.ROT_LABEL.Content = $"Yaw:        \t{Math.Round(this.camera.yaw, 2):F2} \tPitch: \t{Math.Round(this.camera.pitch, 2):F2}";
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

            // Render scene
            {
                this.triangle.Render(this.camera);
                this.floor.Render(this.camera);
            }

            // Render XYZ axis
            {
                // TODO: cache these matrices maybe
                Vector3 position = Rotation.GetOrbitPosition(this.camera.yaw, this.camera.pitch, 10f);
                Matrix4x4 lineModelView = Matrix4x4.CreateLookAt(position, new Vector3(), Vector3.UnitY);

                // Calculates the screen position of the axis preview origin
                Vector3 pos = new Vector3(Maths.Map(60f, 0, e.Width, 1f, -1f), Maths.Map(60f, 0, e.Height, 1f, -1f), 0f);

                // Calculate the model-view-matrix of the line
                // Translation is done at the end to apply translation after projection
                Matrix4x4 lineMvp = lineModelView * this.camera.proj * Matrix4x4.CreateTranslation(pos);
                this.lineX.DrawAt(lineMvp, new Vector3(1f, 0f, 0f));
                this.lineY.DrawAt(lineMvp, new Vector3(0f, 1f, 0f));
                this.lineZ.DrawAt(lineMvp, new Vector3(0f, 0f, 1f));
            }

            // GL.UseProgram(0); // Use your shader program
            // GL.BindVertexArray(this.vao); // Bind the VAO containing cube vertices
            // GL.DrawArrays(PrimitiveType.Triangles, 0, this.verts.Length / 3);
            // GL.BindVertexArray(0); // Unbind VAO

            // Read pixels from OpenGL's back buffer, then write into draw event's back buffer (aka the WPF bitmap)
            // The resulting pixels will be flipped vertically, because OpenGL origin direction is bottom-left, where as
            // windowing apps (like WPF) use the top left.
            // However the LayoutTransform property of the control can be used to scale the layout by -1 in the Y axis to flip
            // it back. It's most likely faster than doing a manual pixel copy, because the layout transformation (i'm guessing) is happening in DirectX

            GL.Flush();
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.ReadPixels(0, 0, e.Width, e.Height, PixelFormat.Bgra, PixelType.UnsignedByte, e.BackBuffer);
            this.ogl.MakeCurrent(false);
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            double diff = e.NewValue - e.OldValue;
            this.triangle.SetPosition(this.triangle.Pos + new Vector3((float) diff));
            this.OGLViewPort.InvalidateVisual();
        }
    }
}