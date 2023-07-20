using System;
using System.Numerics;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine.Utils {
    public class Camera {
        public float yaw;
        public float pitch;

        private int lastW;
        private int lastH;

        private float near;
        private float far;
        private float fov;
        public float orbitRange;

        public Vector3 pos;
        public Vector3 target;

        // view matrix
        public Matrix4x4 view;

        // projection matrix
        public Matrix4x4 proj;

        public Camera(float fov = 50f, float near = 0.01f, float far = 100f) {
            this.near = near;
            this.far = far;
            this.fov = fov;
            this.orbitRange = 10f;
            this.UpdateMatrices();
        }

        public void UpdateSize(int width, int height) {
            if (this.lastW == width && this.lastH == height) {
                return;
            }

            this.lastW = width;
            this.lastH = height;
            this.UpdateProjectionMatrix();
        }

        public void SetTarget(Vector3 target) {
            this.target = target;
            this.UpdateViewMatrix();
        }

        public void SetOrbitRange(float range) {
            this.orbitRange = range;
            this.UpdateViewMatrix();
        }

        public void SetInfo(float fov = 60f, float near = 0.01f, float far = 100f) {
            this.fov = fov;
            this.near = near;
            this.far = far;
            this.UpdateProjectionMatrix();
        }

        public void SetYawPitch(float yaw, float pitch) {
            this.yaw = yaw;
            this.pitch = pitch;
            this.UpdateViewMatrix();
        }

        private void UpdateMatrices() {
            this.UpdateViewMatrix();
            this.UpdateProjectionMatrix();
        }

        /*
            Create view matrix looking at center
            Vector3 center = new Vector3(0f);
            Vector3 up = new Vector3(0f, 1f, 0f);
            Create view matrix looking at center
            Vector3 eye = this.camPos;
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
         */

        private void UpdateViewMatrix() {
            Vector3 direction = new Vector3(
                (float) (Math.Cos(-this.pitch) * Math.Sin(this.yaw)),
                (float) Math.Sin(-this.pitch),
                (float) (Math.Cos(-this.pitch) * Math.Cos(this.yaw))
            );

            this.pos = this.target + direction * this.orbitRange;
            this.view = Matrix4x4.CreateLookAt(this.pos, this.target, Vector3.UnitY);
        }

        private void UpdateProjectionMatrix() {
            float fov = Maths.Deg2Rad(60f);
            float aspect = (float) this.lastW / this.lastH;
            float tanHalfFov = (float) Math.Tan(fov / 2);
            Matrix4x4 mat = Matrix4x4.Identity;
            mat.M11 = 1 / (aspect * tanHalfFov);
            mat.M22 = 1 / (tanHalfFov);
            mat.M33 = - (this.far + this.near) / (this.far - this.near);
            mat.M34 = - 1;
            mat.M43 = - (2 * this.far * this.near) / (this.far - this.near);
            this.proj = mat;
        }
    }
}