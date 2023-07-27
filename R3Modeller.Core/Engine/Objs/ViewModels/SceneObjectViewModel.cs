using System;
using System.Collections.ObjectModel;
using System.Numerics;
using R3Modeller.Core.Engine.Factories;
using R3Modeller.Core.Engine.ViewModels;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine.Objs.ViewModels {
    public class SceneObjectViewModel : BaseViewModel {
        private SceneObjectViewModel parent;
        private readonly ObservableCollection<SceneObjectViewModel> items;

        /// <summary>
        /// This scene object's underlying model object
        /// </summary>
        public SceneObject Model { get; }

        /// <summary>
        /// A collection of child scene objects stored in this scene object
        /// </summary>
        public ReadOnlyObservableCollection<SceneObjectViewModel> Children { get; }

        public Vector3 Pos {
            get => this.Model.RelativeTranslation;
            set {
                this.Model.RelativeTranslation = value;
                this.RaisePositionChanged();
                this.Scene.OnRenderInvalidated();
            }
        }

        public Vector3 PitchYawRoll {
            get => this.Model.RelativePitchYawRoll;
            set {
                this.Model.SetRotation(value);
                this.RaiseRotationChanged();
                this.Scene.OnRenderInvalidated();
            }
        }

        public Vector3 Scale {
            get => this.Model.RelativeScale;
            set {
                this.Model.RelativeScale = value;
                this.RaiseScaleChanged();
                this.Scene.OnRenderInvalidated();
            }
        }

        public float PosX { get => this.Pos.X; set => this.Pos = this.Pos.WithX(value); }
        public float PosY { get => this.Pos.Y; set => this.Pos = this.Pos.WithY(value); }
        public float PosZ { get => this.Pos.Z; set => this.Pos = this.Pos.WithZ(value); }
        public float Pitch { get => this.PitchYawRoll.X; set => this.PitchYawRoll = this.PitchYawRoll.WithX(value); }
        public float Yaw { get => this.PitchYawRoll.Y; set => this.PitchYawRoll = this.PitchYawRoll.WithY(value); }
        public float Roll { get => this.PitchYawRoll.Z; set => this.PitchYawRoll = this.PitchYawRoll.WithZ(value); }
        public float ScaleX { get => this.Scale.X; set => this.Scale = this.Scale.WithX(value); }
        public float ScaleY { get => this.Scale.Y; set => this.Scale = this.Scale.WithY(value); }
        public float ScaleZ { get => this.Scale.Z; set => this.Scale = this.Scale.WithZ(value); }

        // public float AbsoluteWorldPosX => this.Model.WorldPosition.X;
        // public float AbsoluteWorldPosY => this.Model.WorldPosition.Y;
        // public float AbsoluteWorldPosZ => this.Model.WorldPosition.Z;
        // public float AbsoluteWorldScaleX => this.Model.WorldScale.X;
        // public float AbsoluteWorldScaleY => this.Model.WorldScale.Y;
        // public float AbsoluteWorldScaleZ => this.Model.WorldScale.Z;

        public bool IsPositionAbsolute {
            get => this.Model.IsPositionAbsolute;
            set {
                if (value == this.IsPositionAbsolute)
                    return;
                this.Model.IsPositionAbsolute = value;
                this.RaisePropertyChanged();
                this.RaisePositionChanged();
            }
        }

        public bool IsScaleAbsolute {
            get => this.Model.IsScaleAbsolute;
            set {
                if (value == this.IsScaleAbsolute)
                    return;
                this.Model.IsScaleAbsolute = value;
                this.RaisePropertyChanged();
                this.RaiseScaleChanged();
            }
        }

        public bool IsRotationAbsolute {
            get => this.Model.IsRotationAbsolute;
            set {
                if (value == this.IsRotationAbsolute)
                    return;
                this.Model.IsRotationAbsolute = value;
                this.RaisePropertyChanged();
                this.RaiseRotationChanged();
            }
        }

        /// <summary>
        /// The project associated with this scene object. Should only really be set once
        /// </summary>
        public SceneViewModel Scene { get; set; }

        public bool IsRoot => this.parent == null;

        /// <summary>
        /// This object's parent object. Null if this object is a root object
        /// </summary>
        public SceneObjectViewModel Parent {
            get => this.parent;
            set => this.RaisePropertyChanged(ref this.parent, value);
        }

        public SceneObjectViewModel TopLevelParent {
            get {
                SceneObjectViewModel top = null, p = this.parent;
                for (; p != null && !p.IsRoot; p = p.parent) // "p != null" should be false unless this is called on the root container
                    top = p;
                return top;
            }
        }

        public string DisplayName {
            get => this.Model.DisplayName;
            set => this.RaisePropertyChanged(ref this.Model.DisplayName, value);
        }

        public bool IsVisible {
            get => this.Model.IsVisible;
            set {
                this.RaisePropertyChanged(ref this.Model.IsVisible, value);
                this.Scene.OnRenderInvalidated();
            }
        }

        public bool IsObjectSelected {
            get => this.Model.IsObjectSelected;
            set {
                this.RaisePropertyChanged(ref this.Model.IsObjectSelected, value);
                this.Scene.OnRenderInvalidated();
            }
        }

        protected virtual bool CanRemoveFromParent => this.Model.CanRemoveFromParent;

        public SceneObjectViewModel(SceneObject model) {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.items = new ObservableCollection<SceneObjectViewModel>();
            this.Children = new ReadOnlyObservableCollection<SceneObjectViewModel>(this.items);
            for (int i = 0, c = model.Items.Count; i < c; i++) {
                this.InsertItemAt(i, SORegistry.Instance.CreateViewModelFromModel(model.Items[i]), false);
            }
        }

        public void AddItem(SceneObjectViewModel obj) => this.InsertItemAt(this.items.Count, obj);

        public void InsertItemAt(int index, SceneObjectViewModel obj, bool callModel = true) {
            if (ReferenceEquals(this, obj))
                throw new Exception("Cannot add ourself to our children collection");
            if (this.items.Contains(obj))
                throw new Exception("Item already stored in this object");

            ValidateHasNoParent(obj);
            if (callModel) {
                this.Model.InsertItemAt(index, obj.Model);
            }

            obj.parent = this;
            this.items.Insert(index, obj);
            obj.OnAddedToGraph();
            this.Scene?.OnObjectAdded(obj);
            obj.RaisePropertyChanged(nameof(obj.Parent));
        }

        public bool RemoveItem(SceneObjectViewModel obj, bool callModel = true) {
            int index = this.items.IndexOf(obj);
            if (index == -1) {
                return false;
            }

            this.RemoveItemAt(index, callModel);
            return true;
        }

        public void RemoveItemAt(int index, bool callModel = true) {
            SceneObjectViewModel obj = this.items[index];
            ValidateOwnsObject(this, obj);
            if (callModel) {
                ValidateChildModelAt(this, obj, index);
                this.Model.RemoveItemAt(index);
            }

            this.items.RemoveAt(index);
            obj.OnRemovedFromGraph(null);
            this.Scene?.OnObjectRemoved(obj);
            obj.parent = null;
            obj.RaisePropertyChanged(nameof(obj.Parent));
        }

        // Primarily used to convert a "friendly" object into a standard mesh
        /// <summary>
        /// Removes the item at the given index, and then inserts the given object at that index. This is a more efficient
        /// implementation than calling <see cref="RemoveItemAt"/> and then <see cref="InsertItemAt"/>
        /// </summary>
        /// <param name="index">The index of the object to replace</param>
        /// <param name="obj">The object to add to this object</param>
        /// <returns>The object that was replaced/removed</returns>
        public SceneObjectViewModel ReplaceItemAt(int index, SceneObjectViewModel obj, bool callModel = true) {
            SceneObjectViewModel oldObj = this.items[index]; // check this first for IOOB exception
            if (ReferenceEquals(oldObj, obj))
                throw new Exception("Cannot replace an object with itself");
            if (this.items.Contains(obj))
                throw new Exception("Object is already in this object");

            ValidateHasNoParent(obj);
            ValidateOwnsObject(this, oldObj);
            if (callModel) {
                ValidateChildModelAt(this, oldObj, index);
                this.Model.ReplaceItemAt(index, obj.Model);
            }

            this.items[index] = obj;

            oldObj.OnRemovedFromGraph(obj);
            oldObj.parent = null;

            obj.parent = this;
            obj.OnAddedToGraph();

            this.Scene?.OnObjectReplaced(oldObj, obj);

            oldObj.RaisePropertyChanged(nameof(oldObj.Parent));
            obj.RaisePropertyChanged(nameof(obj.Parent));
            return oldObj;
        }

        /// <summary>
        /// Clears this object's children collection, calling <see cref="OnClearingParentChildren"/> for each child object
        /// </summary>
        public void Clear() {
            this.Model.Clear();
            using (ExceptionStack stack = new ExceptionStack()) {
                for (int i = this.items.Count - 1; i >= 0; i--) {
                    SceneObjectViewModel obj = this.items[i];
                    if (!obj.CanRemoveFromParent) {
                        continue;
                    }

                    try {
                        obj.OnClearingParentChildren();
                    }
                    catch (Exception e) {
                        stack.Add(e);
                    }
                }

                try {
                    this.Scene?.OnObjectCleared(this);
                }
                catch (Exception e) {
                    stack.Add(e);
                }

                // Traverse backwards to hopefully reduce the array copying ()
                for (int i = this.items.Count - 1; i >= 0; i--) {
                    if (this.items[i].CanRemoveFromParent) {
                        this.items.RemoveAt(i);
                    }
                }

                // Just in case...
                ValidateModelCollectionSizes(this);
                for (int i = this.items.Count - 1; i >= 0; i--) {
                    ValidateChildModelAt(this, i);
                }
            }
        }

        // annoying having to have the exact same code for both viewmodels and models... but it's nessesary in many ways
        // primarly: how do you get a view model from a model in a performant way, preferably linear lookup?
        // well... you store a reference. but once models know about view models, that's where it gets iffy, because what if
        // you need multiple scene object view models per model? that would be bad design but still.
        // The same model should not exist more than once, but that isn't necessarily the same for view models as they
        // just bridge that gap between view and model

        /// <summary>
        /// Called when this object is added to the scene graph, either as a root object or a child of a parent. <see cref="Parent"/> will be set before this call
        /// </summary>
        protected virtual void OnAddedToGraph() {

        }

        /// <summary>
        /// Called when this object is removed from the scene graph (the parent object or root), or is replaced with
        /// another object (effectively removing this object). <see cref="Parent"/> will be set to null after this call
        /// <para>
        /// This object is removed from the parent's underlying collection before this call, so attempting to get the index
        /// of ourself in our parent will result in failure
        /// </para>
        /// </summary>
        /// <param name="replacement">
        /// The scene object that is about to replace us. Will be null
        /// if the current object is just being removed, not replaced
        /// </param>
        protected virtual void OnRemovedFromGraph(SceneObjectViewModel replacement) {

        }

        /// <summary>
        /// <para>
        /// Called when this object is moved from one object to another. <see cref="Parent"/> and <see cref="oldParent"/> will not be null.
        /// Use <see cref="IsRoot"/> on <see cref="oldParent"/> to check if this object was moved from the root collection to deeper into the hierarchy
        /// </para>
        /// </summary>
        /// <param name="oldParent">The previous parent</param>
        protected virtual void OnMovedBetweenObjects(SceneObjectViewModel oldParent) {

        }

        /// <summary>
        /// Called when this object's parent's children collection is cleared. <see cref="OnRemovedFromGraph"/> will
        /// not be called, and instead, this will get called. However, this function does just delegate to calling <see cref="OnRemovedFromGraph"/>
        /// <para>
        /// The parent's underlying collection will still contain this child before being removed, as the process is that this
        /// function is called for each child before actually clearing the children
        /// </para>
        /// </summary>
        protected virtual void OnClearingParentChildren() {
            this.OnRemovedFromGraph(null);
        }

        public static void ValidateOwnsObject(SceneObjectViewModel @this, SceneObjectViewModel obj) {
            if (!ReferenceEquals(@this, obj.parent)) {
                throw new Exception("Expected object's parent to be equal to the current object instance");
            }
        }

        public static void ValidateHasNoParent(SceneObjectViewModel obj) {
            if (obj.parent != null) {
                throw new Exception("Expected object's parent to be null");
            }
        }

        public void SetScene(SceneViewModel scene) {
            this.Scene = scene;
            foreach (SceneObjectViewModel obj in this.items) {
                obj.SetScene(scene);
            }
        }

        public void RaisePositionChanged() {
            this.RaisePropertyChanged(nameof(this.Pos));
            this.RaisePropertyChanged(nameof(this.PosX));
            this.RaisePropertyChanged(nameof(this.PosY));
            this.RaisePropertyChanged(nameof(this.PosZ));
        }

        public void RaiseScaleChanged() {
            this.RaisePropertyChanged(nameof(this.Scale));
            this.RaisePropertyChanged(nameof(this.ScaleX));
            this.RaisePropertyChanged(nameof(this.ScaleY));
            this.RaisePropertyChanged(nameof(this.ScaleZ));
        }

        public void RaiseRotationChanged() {
            this.RaisePropertyChanged(nameof(this.PitchYawRoll));
            this.RaisePropertyChanged(nameof(this.Pitch));
            this.RaisePropertyChanged(nameof(this.Yaw));
            this.RaisePropertyChanged(nameof(this.Roll));
        }

        internal static void SetScene(SceneObjectViewModel obj, SceneViewModel scene) => obj.Scene = scene;

        private static void ValidateModelCollectionSizes(SceneObjectViewModel obj) {
            if (obj.items.Count != obj.Model.Items.Count) {
                throw new Exception("View model desynchronized with model (mis-matched backing collection sizes)");
            }
        }

        private static void ValidateChildModelAt(SceneObjectViewModel parent, SceneObjectViewModel obj, int index) {
            if (!ReferenceEquals(obj.Model, parent.Model.Items[index])) {
                throw new Exception("View model desynchronized with model (child ViewModel's model does not match parent ViewModel's model collection at index)");
            }
        }

        private static void ValidateChildModelAt(SceneObjectViewModel parent, int index) => ValidateChildModelAt(parent, parent.items[index], index);
    }
}