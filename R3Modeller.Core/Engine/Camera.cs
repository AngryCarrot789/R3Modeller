using System;
using System.Numerics;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine {
    public class Camera {
        private int lastW;
        private int lastH;

        public float yaw;
        public float pitch;

        public float near;
        public float far;
        public float fov;
        public float orbitRange;

        // actual position of the camera
        public Vector3 pos;

        // the target orbit position. this affects pos
        public Vector3 target;

        // the camera direction based on yaw and pitch
        public Vector3 direction;

        // view matrix
        public Matrix4x4 view;

        // projection matrix
        public Matrix4x4 proj;

        public Camera(float fov = 60f, float near = 0.01f, float far = 1000f) {
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
            this.orbitRange = Maths.Clamp(range, 0.0001f, 5000f);
            this.UpdateViewMatrix();
        }

        public void SetFov(float fov = 60f) {
            this.fov = Maths.Clamp(fov, 5f, 175f);
            this.UpdateProjectionMatrix();
        }

        public void SetDrawDistance(float near = 0.01f, float far = 1000f) {
            this.near = near;
            this.far = far;
            this.UpdateProjectionMatrix();
        }

        public void SetInfo(float fov = 60f, float near = 0.01f, float far = 1000f) {
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
            this.direction = new Vector3(
                (float) (Math.Cos(-this.pitch) * Math.Sin(this.yaw)),
                (float) Math.Sin(-this.pitch),
                (float) (Math.Cos(-this.pitch) * Math.Cos(this.yaw))
            );

            this.pos = this.target + this.direction * this.orbitRange;
            // this.view = Matrix4x4.CreateLookAt(this.pos, this.target, Vector3.UnitY);

            // inlined LookAt function with a few slight optimisations
            Vector3 zaxis = Vector3.Normalize(this.pos - this.target);
            Vector3 xaxis = Vector3.Normalize(new Vector3(zaxis.Z, 0f, -zaxis.X));
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);

            Matrix4x4 result;
            result.M11 = xaxis.X;
            result.M12 = yaxis.X;
            result.M13 = zaxis.X;
            result.M14 = 0.0f;
            result.M21 = xaxis.Y;
            result.M22 = yaxis.Y;
            result.M23 = zaxis.Y;
            result.M24 = 0.0f;
            result.M31 = xaxis.Z;
            result.M32 = yaxis.Z;
            result.M33 = zaxis.Z;
            result.M34 = 0.0f;
            result.M41 = -Vector3.Dot(xaxis, this.pos);
            result.M42 = -Vector3.Dot(yaxis, this.pos);
            result.M43 = -Vector3.Dot(zaxis, this.pos);
            result.M44 = 1.0f;
            this.view = result;
        }

        private void UpdateProjectionMatrix() {
            float fovRad = Maths.Deg2Rad(this.fov);
            float aspect = (float) this.lastW / this.lastH;
            float tfov = (float) Math.Tan(fovRad / 2);
            Matrix4x4 mat = Matrix4x4.Identity;
            mat.M11 = 1f / (aspect * tfov);
            mat.M22 = 1f / tfov;
            mat.M33 = -(this.far + this.near) / (this.far - this.near);
            mat.M34 = -1f;
            mat.M43 = -(2f * this.far * this.near) / (this.far - this.near);
            this.proj = mat;
        }
    }
}