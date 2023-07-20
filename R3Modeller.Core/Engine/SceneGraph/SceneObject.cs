using System;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;

namespace R3Modeller.Core.Engine.SceneGraph {
    public class SceneObject {
        private Vector3 pos;
        private Vector3 scale;
        private Vector3 euler;

        public Vector3 Pos => this.pos;
        public Vector3 Scale => this.scale;
        public Vector3 Euler => this.euler;

        // The model matrix containing our transformation
        protected Matrix4x4 currentModelMatrix;
        // The model matrix containing our + parent chain transformation
        protected Matrix4x4 modelMatrix;

        protected SceneObject parent;
        protected readonly List<SceneObject> children;

        public SceneObject() {
            this.pos = Vector3.Zero;
            this.scale = Vector3.One;
            this.euler = Vector3.Zero;
            this.children = new List<SceneObject>();
            this.UpdateModelMatrix();
        }

        public void AddChild(SceneObject obj) {
            if (ReferenceEquals(this, obj)) {
                throw new Exception("Cannot add ourself to our children collection");
            }

            if (this.children.Contains(obj)) {
                throw new Exception("Item already added");
            }

            SceneObject oldParent = obj.parent;
            obj.parent = this;
            this.children.Add(obj);
            obj.OnParentChanged(oldParent, this);
        }

        public void SetTransformation(Vector3 pos, Vector3 scale, Vector3 euler) {
            this.pos = pos;
            this.scale = scale;
            this.euler = euler;
            this.UpdateModelMatrix();
        }

        public void SetPosition(Vector3 pos) {
            this.pos = pos;
            this.UpdateModelMatrix();
        }

        public void SetScale(Vector3 scale) {
            this.scale = scale;
            this.UpdateModelMatrix();
        }

        public void SetRotation(Vector3 euler) {
            this.euler = euler;
            this.UpdateModelMatrix();
        }

        protected virtual void OnParentChanged(SceneObject oldParent, SceneObject newParent) {
            this.UpdateModelMatrix();
        }

        public virtual void Render(Camera camera) {
            this.RenderChildren(camera);
        }

        public virtual void RenderChildren(Camera camera) {
            foreach (SceneObject obj in this.children) {
                obj.Render(camera);
            }
        }

        protected void UpdateModelMatrix() {
            Matrix4x4 matrix = MatrixUtils.LocalToWorld(this.pos, this.euler, this.scale);
            this.currentModelMatrix = matrix;
            if (this.parent == null) {
                this.modelMatrix = matrix;
            }
            else {
                this.modelMatrix = this.parent.modelMatrix * matrix;
            }

            foreach (SceneObject obj in this.children) {
                obj.UpdateModelMatrix();
            }
        }
    }
}