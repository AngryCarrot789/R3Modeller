using System;
using System.IO;
using System.Runtime.CompilerServices;
using R3Modeller.Controls;
using System.Windows;
using ObjectLoader.Loaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace R3Modeller {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly OGLContextWrapper ogl;

        public MainWindow(OGLContextWrapper ogl) {
            this.ogl = ogl;
            this.InitializeComponent();
            ObjLoaderFactory factory = new ObjLoaderFactory();


            // this.ogl.MakeCurrent(true);
            //
            // this.ogl.MakeCurrent(false);
        }

        // TODO: Could have ViewPortViewModel, which stores a reference to an interface (implemented by an
        // OpenGL control which references the SceneViewModel so that it can draw the objects, selection, etc)
        // Those are just ideas so far

        private void OnPaintViewPort(object sender, DrawEventArgs e) {
            this.ogl.MakeCurrent(true);

            // Update hidden window, if the size has changed
            this.ogl.UpdateSize(e.Width, e.Height);
            GL.Viewport(0, 0, e.Width, e.Height);

            // Without scaling Y by -1, the rendered image would be flipped (vertically), most likely because
            // OpenGL's "origin direction" (aka the direction towards negative) is the bottom left, whereas
            // windows are the top left
            GL.PushMatrix();
            GL.Scale(1f, -1f, 1f);

            // clear screen then draw an RGB triangle
            GL.ClearColor(0.7f, 0.7f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1f, 0f, 0f);
            GL.Vertex3(-0.5f, -0.5f, 0f);
            GL.Color3(0f, 1f, 0f);
            GL.Vertex3( 0.0f,  0.5f, 0f);
            GL.Color3(0f, 0f, 1f);
            GL.Vertex3( 0.5f, -0.5f, 0f);
            GL.End();
            GL.Flush();

            GL.PopMatrix();

            // read pixels from OpenGL's back buffer, then write into draw event's back buffer (aka the WPF bitmap)
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.ReadPixels(0, 0, e.Width, e.Height, PixelFormat.Bgra, PixelType.UnsignedByte, e.BackBuffer);
            this.ogl.MakeCurrent(false);

            // draw into an offscreen buffer
            // read pixels from OpenGL and paste them into the bitmap
        }
    }
}