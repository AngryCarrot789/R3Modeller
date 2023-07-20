using System;
using System.Numerics;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine {
    public class Camera {
        public int LastVpWidth;
        public int LastVpHeight;
        public float Near;
        public float Far;
        public float FOV;

        public Vector3 pos;


        public Matrix4x4 proj;
        public Matrix4x4 view;

        /// <summary>
        /// The pre-multiplied projection and view matrices
        /// </summary>
        public Matrix4x4 Matrix => this.proj * this.view;

        public Camera(float near = 0.01f, float far = 1000f, float fov = 80f) {
            this.pos = new Vector3();
            this.Near = near;
            this.Far = far;
            this.FOV = fov;
            this.proj = Matrix4x4.Identity;
            this.view = Matrix4x4.Identity;
        }

        public void UpdateView() {
            // this.view = Matrix4x4.CreateLookAt(this.pos, )
        }

        public void UpdateSize(int w, int h) {
            if (this.LastVpWidth != w || this.LastVpHeight != h) {
                this.LastVpWidth = w;
                this.LastVpHeight = h;
                this.UpdateProjection();
            }
        }

        public void UpdateProjection() {
            float fovRads = 1.0f / (float) Math.Tan(this.FOV * Maths.PI / 360.0f);
            float aspect = (float) this.LastVpWidth / this.LastVpHeight;
            float distance = this.Near - this.Far;

            // Matrix4x4 mat = new Matrix4x4();
            // mat.M11 = fovRads * aspect;
            // mat.M22 = fovRads;
            // mat.M33 = (this.Near + this.Far) / distance;
            // mat.M34 = (2 * this.Near * this.Far) / distance;
            // mat.M43 = -1.0f;

            Matrix4x4 mat = new Matrix4x4 {
                M11 = fovRads / aspect,
                M22 = fovRads,
                M33 = (this.Near + this.Far) / distance,
                M34 = (2 * this.Near * this.Far) / distance,
                M43 = -1.0f,
                M44 = 0.0f
            };

            this.proj = mat;
        }
    }
}