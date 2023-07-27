using System;
using System.Collections.Generic;
using System.Numerics;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine.Objs {
    public class SceneObject {
        // bit1 = pos, bit2 = scale, bit3 = rotation
        private bool isPositionAbs;
        private bool isScaleAbs;
        private bool isRotationAbs;

        // the properties for this specific object
        private Vector3 relativePos;
        private Vector3 relativeScale;
        private Vector3 relativePYR;

        // the properties that may be based on the parent object
        private Vector3 worldPos;
        private Vector3 worldScale;
        private Quaternion worldRotation;

        /// <summary>
        /// This object's position. This may be relative or
        /// </summary>
        public Vector3 RelativePosition {
            get => this.relativePos;
            set {
                this.relativePos = value;
                this.UpdateTransformation();
            }
        }

        public Vector3 RelativeScale {
            get => this.relativeScale;
            set {
                this.relativeScale = value;
                this.UpdateTransformation();
            }
        }

        public Vector3 RelativePitchYawRoll {
            get => this.relativePYR;
            set {
                this.relativePYR = value;
                this.UpdateTransformation();
            }
        }

        /// <summary>
        /// The calculated absolute position, updated via <see cref="UpdateTransformation"/>
        /// </summary>
        public Vector3 WorldPosition => this.worldPos;

        /// <summary>
        /// The calculated absolute scale, updated via <see cref="UpdateTransformation"/>
        /// </summary>
        public Vector3 WorldScale => this.worldScale;

        /// <summary>
        /// The calculated absolute rotation, updated via <see cref="UpdateTransformation"/>
        /// </summary>
        public Quaternion WorldRotation => this.worldRotation;

        public bool IsPositionAbsolute {
            get => this.isPositionAbs;
            set {
                this.isPositionAbs = value;
                this.UpdateTransformation();
            }
        }

        public bool IsScaleAbsolute {
            get => this.isScaleAbs;
            set {
                this.isScaleAbs = value;
                this.UpdateTransformation();
            }
        }

        public bool IsRotationAbsolute {
            get => this.isRotationAbs;
            set {
                this.isRotationAbs = value;
                this.UpdateTransformation();
            }
        }

        // The model matrix containing our + parent chain transformation
        protected Matrix4x4 modelMatrix;

        protected SceneObject parent;
        protected readonly List<SceneObject> items;

        public SceneObject Parent => this.parent;

        public SceneObject TopLevelParent {
            get {
                SceneObject top = null, p = this.parent;
                for (; p != null && !p.IsRoot; p = p.parent) // "p != null" should be false unless this is called on the root container
                    top = p;
                return top;
            }
        }

        public IReadOnlyList<SceneObject> Items => this.items;

        public string DisplayName;
        public bool IsVisible;
        public bool IsObjectSelected;

        /// <summary>
        /// Whether or not this is this root scene object container. The scene graph is stored in a single
        /// hidden scene object, and this property returns true if we are that hidden object
        /// </summary>
        public bool IsRoot => this.parent == null;

        public SceneObject() {
            this.relativePos = Vector3.Zero;
            this.relativeScale = Vector3.One;
            this.relativePYR = Vector3.Zero;
            this.items = new List<SceneObject>();
            this.DisplayName = this.GetType().Name;
            this.IsVisible = true;
            this.UpdateTransformation();
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

        public void SetTransformation(Vector3 pos, Vector3 scale, Vector3 rotation) {
            this.relativePos = pos;
            this.relativeScale = scale;
            this.relativePYR = rotation;
            this.UpdateTransformation();
        }

        public void SetRotation(Vector3 rotation) {
            this.relativePYR = rotation;
            this.UpdateTransformation();
        }

        /// <summary>
        /// Called when this object is added to the scene graph, either as a root object or a child of a parent. <see cref="Parent"/> will be set before this call
        /// </summary>
        protected virtual void OnAddedToGraph() {
            this.UpdateTransformation();
        }

        /// <summary>
        /// <para>
        /// Called when this object is moved from one object to another. <see cref="Parent"/> and <see cref="oldParent"/> will not be null.
        /// Use <see cref="IsRoot"/> to check if on <see cref="oldParent"/> to check if this object was moved from the root collection deeper into the hierarchy
        /// </para>
        /// </summary>
        /// <param name="oldParent">The previous parent</param>
        protected virtual void OnParentChanged(SceneObject oldParent) {
            this.UpdateTransformation();
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
                if (obj.IsVisible) {
                    obj.Render(camera);
                }
            }
        }

        /// <summary>
        /// Updates this object's data which is based on the parent object, e.g. absolute
        /// matrix, absolute position, etc, and also update all child objects
        /// </summary>
        public void UpdateTransformation() {
            Vector3 rotVec = this.relativePYR;
            double radPitch = rotVec.X * (Math.PI / 180.0);
            double radYaw = rotVec.Y * (Math.PI / 180.0);
            double radRoll = rotVec.Z * (Math.PI / 180.0);
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll((float) radYaw, (float) radPitch, (float) radRoll);
            if (this.parent == null) {
                this.worldPos = this.relativePos;
                this.worldScale = this.relativeScale;
                this.worldRotation = rotation;
            }
            else {
                if (this.isPositionAbs) {
                    this.worldPos = this.relativePos;
                }
                else {
                    this.worldPos = this.parent.worldPos + Vector3.Transform(this.relativePos, this.parent.worldRotation);
                }

                if (this.isRotationAbs) {
                    this.worldRotation = rotation;
                }
                else {
                    this.worldRotation = this.parent.worldRotation * rotation;
                }

                if (this.isScaleAbs) {
                    this.worldScale = this.relativeScale;
                }
                else {
                    this.worldScale = this.parent.worldScale * this.relativeScale;
                }
            }

            Matrix4x4 s = Matrix4x4.CreateScale(this.worldScale);
            Matrix4x4 r = Matrix4x4.CreateFromQuaternion(this.worldRotation);
            Matrix4x4 t = Matrix4x4.CreateTranslation(this.worldPos);
            this.modelMatrix = s * r * t;
            foreach (SceneObject obj in this.items) {
                obj.UpdateTransformation();
            }
        }

        public void DisposeRecursive() {
            using (ExceptionStack stack = new ExceptionStack("Exception disposing scene object")) {
                this.DisposeRecursive(stack);
            }
        }

        protected void DisposeRecursive(ExceptionStack stack) {
            this.DisposeCore(stack);
            foreach (SceneObject item in this.items) {
                item.DisposeRecursive(stack);
            }
        }

        protected virtual void DisposeCore(ExceptionStack stack) {

        }

        // z = 2 + 3i
        // w = 1 - 1i
        // x = z * w = (2 + 3i) * (1 - 1i)
        // (3+1) + (3i+1) + (3i+1) + (3i + 1i)

        // x = 5 + 1i

        /*
         * (2x + 3) * (5x - 8)
         * First: (2x) * (5x) = 10x^2 (because 2*5 * x*x)
         * Outer: (2x) * (-8) = -16x (because 2*-8, and include the x)
         * Inner: (3)  * (5x) = 15x (because 3*5, and include the x)
         * Last:  (3)  * (-8) = -24 (because 3*-8 obviously)
         * subtract 16x because it's negative, add 15x because it's positive, and subtract 24 because it's negative
         * 10x^2 - 16x + 15x - 24
         * Handle the addition first: -16x + 15x = -1x which can be written as -x because -1x == -1*x
         * 10x^2 -1x - 24
         */

                /*
                 * (x+3) * (2x-1)
                 *
                 */
            }
        }