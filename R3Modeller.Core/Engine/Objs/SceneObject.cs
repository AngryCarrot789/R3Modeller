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
        protected readonly List<SceneObject> items;

        public SceneObject Parent => this.parent;
        public IReadOnlyList<SceneObject> Items => this.items;

        public string DisplayName;

        public bool IsRoot => this.parent == null;

        public SceneObject() {
            this.pos = Vector3.Zero;
            this.scale = Vector3.One;
            this.rotation = Vector3.Zero;
            this.items = new List<SceneObject>();
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

        public void AddItem(SceneObject obj) => this.InsertItemAt(this.items.Count, obj);

        public void InsertItemAt(int index, SceneObject obj) {
            if (ReferenceEquals(this, obj))
                throw new Exception("Cannot add ourself to our children collection");
            if (this.items.Contains(obj))
                throw new Exception("Item already stored in this object");

            ValidateHasNoParent(obj);
            this.items.Insert(index, obj);
            obj.parent = this;
            obj.OnAddedToGraph();
        }

        public bool RemoveItem(SceneObject obj) {
            int index = this.items.IndexOf(obj);
            if (index == -1) {
                return false;
            }

            this.RemoveItemAt(index);
            return true;
        }

        public void RemoveItemAt(int index) {
            SceneObject obj = this.items[index];
            ValidateOwnsObject(this, obj);
            this.items.RemoveAt(index);
            try {
                obj.OnRemovedFromGraph(false);
            }
            finally {
                obj.parent = null;
            }
        }

        // Primarily used to convert a "friendly" object into a standard mesh
        /// <summary>
        /// Removes the item at the given index, and then inserts the given object at that index. This is a more efficient
        /// implementation than calling <see cref="RemoveItemAt"/> and then <see cref="InsertItemAt"/>
        /// </summary>
        /// <param name="index">The index of the object to replace</param>
        /// <param name="obj">The object to add to this object</param>
        /// <returns>The object that was replaced/removed</returns>
        public SceneObject ReplaceItemAt(int index, SceneObject obj) {
            SceneObject oldObj = this.items[index];
            if (ReferenceEquals(oldObj, obj))
                throw new Exception("Cannot replace an object with itself");
            if (this.items.Contains(obj))
                throw new Exception("Object is already in this object");

            ValidateHasNoParent(obj);
            ValidateOwnsObject(this, oldObj);

            this.items[index] = obj;
            try {
                oldObj.OnRemovedFromGraph(true);
            }
            finally { // OnRemovedFromGraph should result in an app crash
                oldObj.parent = null;
            }

            obj.parent = this;
            obj.OnAddedToGraph();
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

        /// <summary>
        /// Called when this object is added to the scene graph, either as a root object or a child of a parent. <see cref="Parent"/> will be set before this call
        /// </summary>
        protected virtual void OnAddedToGraph() {
            this.UpdateModelMatrix();
        }

        /// <summary>
        /// <para>
        /// Called when this object is moved from one object to another. <see cref="Parent"/> and <see cref="oldParent"/> will not be null.
        /// Use <see cref="IsRoot"/> to check if on <see cref="oldParent"/> to check if this object was moved from the root collection deeper into the hierarchy
        /// </para>
        /// </summary>
        /// <param name="oldParent">The previous parent</param>
        protected virtual void OnParentChanged(SceneObject oldParent) {
            this.UpdateModelMatrix();
        }

        /// <summary>
        /// Called when this object is removed from the scene graph (the parent object), or the root. <see cref="Parent"/> will be set after this call
        /// </summary>
        /// <param name="isBeingReplaced">This item is being replaced with another at the same index</param>
        protected virtual void OnRemovedFromGraph(bool isBeingReplaced) {

        }

        public virtual void Render(Camera camera) {
            this.RenderChildren(camera);
        }

        public virtual void RenderChildren(Camera camera) {
            foreach (SceneObject obj in this.items) {
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

            foreach (SceneObject obj in this.items) {
                obj.UpdateModelMatrix();
            }
        }
    }
}