using System;
using System.Collections.Generic;
using System.Numerics;
using R3Modeller.Core.Engine.Utils;
using Vector3 = System.Numerics.Vector3;

namespace R3Modeller.Core.Engine.Objs {
    public class SceneObject {
        public Vector3 pos;
        public Vector3 scale;
        public Vector3 rotation;
        private RotationType rotationType;

        // The model matrix containing our transformation
        protected Matrix4x4 currentModelMatrix;

        // The model matrix containing our + parent chain transformation
        protected Matrix4x4 modelMatrix;

        protected SceneObject parent;
        protected readonly List<SceneObject> children;

        public SceneObject Parent => this.parent;
        public IEnumerable<SceneObject> Children => this.children;

        public string DisplayName;

        public SceneObject() {
            this.pos = Vector3.Zero;
            this.scale = Vector3.One;
            this.rotation = Vector3.Zero;
            this.children = new List<SceneObject>();
            this.UpdateModelMatrix();
            this.DisplayName = this.GetType().Name;
        }

        public static void ValidateOwnsObject(SceneObject @this, SceneObject obj) {
            if (!ReferenceEquals(@this, obj.parent)) {
                throw new Exception("Expected object's parent to be equal to the current object instance");
            }
        }

        public static void ValidateHasNoParent(SceneObject obj) {
            if (obj.parent != null) {
                throw new Exception("Expected object's parent to be null");
            }
        }

        public void AddChild(SceneObject obj) {
            if (ReferenceEquals(this, obj))
                throw new Exception("Cannot add ourself to our children collection");
            if (this.children.Contains(obj))
                throw new Exception("Item already added");

            ValidateHasNoParent(obj);
            this.children.Add(obj);
            obj.parent = this;
            obj.OnParentChanged(null, this);
        }

        public bool RemoveChild(SceneObject obj) {
            int index = this.children.IndexOf(obj);
            if (index == -1) {
                return false;
            }

            this.RemoveChildAt(index);
            return true;
        }

        public void RemoveChildAt(int index) {
            SceneObject obj = this.children[index];
            ValidateOwnsObject(this, obj);
            obj.parent = null;
            this.children.RemoveAt(index);
            obj.OnParentChanged(this, null);
        }

        // Primarily used to convert a "friendly" object into a standard mesh
        public SceneObject ReplaceChild(int index, SceneObject obj) {
            SceneObject oldObj = this.children[index];
            if (ReferenceEquals(oldObj, obj))
                throw new Exception("Cannot replace an object with itself");
            if (this.children.Contains(obj))
                throw new Exception("Object is already in this object");

            ValidateHasNoParent(obj);
            ValidateOwnsObject(this, oldObj);

            this.children[index] = obj;
            oldObj.parent = null;
            oldObj.OnParentChanged(this, null);

            obj.parent = this;
            oldObj.OnParentChanged(null, this);
            return oldObj;
        }

        public void SetTransformation(Vector3 pos, Vector3 scale, Vector3 euler) {
            this.pos = pos;
            this.scale = scale;
            this.rotation = euler;
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

        public void SetRotation(Vector3 euler, RotationType type = RotationType.Euler) {
            this.rotation = euler;
            this.rotationType = type;
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

        /// <summary>
        /// Updates the model object for this scene object, and all of the child objects
        /// <para>
        /// The model matrix contains information about this object's transformation
        /// </para>
        /// <para>
        /// This should be called whenever the pos, rotation (or rotation type) or scale changes
        /// </para>
        /// </summary>
        public void UpdateModelMatrix() {
            Matrix4x4 rotate;
            switch (this.rotationType) {
                case RotationType.Bearings: rotate = MatrixUtils.CreateRotationYPR(this.rotation); break;
                default: rotate = MatrixUtils.CreateRotationYXZ(this.rotation); break;
            }

            Matrix4x4 matrix = Matrix4x4.CreateTranslation(this.pos) * rotate * Matrix4x4.CreateScale(this.scale);
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