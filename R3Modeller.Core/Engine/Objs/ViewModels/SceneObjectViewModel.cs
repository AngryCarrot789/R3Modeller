using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using R3Modeller.Core.Engine.Factories;
using R3Modeller.Core.Engine.ViewModels;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.Engine.Objs.ViewModels {
    public class SceneObjectViewModel : BaseViewModel {
        private SceneObjectViewModel parent;
        private readonly ObservableCollection<SceneObjectViewModel> children;

        /// <summary>
        /// This scene object's underlying model object
        /// </summary>
        public SceneObject Model { get; }

        /// <summary>
        /// A collection of child scene objects stored in this scene object
        /// </summary>
        public ReadOnlyObservableCollection<SceneObjectViewModel> Children { get; }

        public Vector3 Pos {
            get => this.Model.pos;
            set {
                this.Model.SetPosition(value);
                this.RaisePropertyChanged(nameof(this.Pos));
                this.RaisePropertyChanged(nameof(this.PosX));
                this.RaisePropertyChanged(nameof(this.PosY));
                this.RaisePropertyChanged(nameof(this.PosZ));
                this.Project.OnRenderInvalidated();
            }
        }

        public Vector3 Rotation {
            get => this.Model.rotation;
            set {
                this.Model.SetRotation(value);
                this.RaisePropertyChanged(nameof(this.Rotation));
                this.RaisePropertyChanged(nameof(this.RotationX));
                this.RaisePropertyChanged(nameof(this.RotationY));
                this.RaisePropertyChanged(nameof(this.RotationZ));
                this.Project.OnRenderInvalidated();
            }
        }

        public Vector3 Scale {
            get => this.Model.scale;
            set {
                this.Model.SetScale(value);
                this.RaisePropertyChanged(nameof(this.Scale));
                this.RaisePropertyChanged(nameof(this.ScaleX));
                this.RaisePropertyChanged(nameof(this.ScaleY));
                this.RaisePropertyChanged(nameof(this.ScaleZ));
                this.Project.OnRenderInvalidated();
            }
        }

        public float PosX { get => this.Pos.X; set => this.Pos = this.Pos.SetX(value); }
        public float PosY { get => this.Pos.Y; set => this.Pos = this.Pos.SetY(value); }
        public float PosZ { get => this.Pos.Z; set => this.Pos = this.Pos.SetZ(value); }

        public float RotationX { get => this.Rotation.X; set => this.Rotation = this.Rotation.SetX(value); }
        public float RotationY { get => this.Rotation.Y; set => this.Rotation = this.Rotation.SetY(value); }
        public float RotationZ { get => this.Rotation.Z; set => this.Rotation = this.Rotation.SetZ(value); }

        public float ScaleX { get => this.Scale.X; set => this.Scale = this.Scale.SetX(value); }
        public float ScaleY { get => this.Scale.Y; set => this.Scale = this.Scale.SetY(value); }
        public float ScaleZ { get => this.Scale.Z; set => this.Scale = this.Scale.SetZ(value); }

        /// <summary>
        /// The project associated with this scene object. Should only really be set once
        /// </summary>
        public ProjectViewModel Project { get; set; }

        /// <summary>
        /// This object's parent object. Null if this object is a root object
        /// </summary>
        public SceneObjectViewModel Parent {
            get => this.parent;
            set => this.RaisePropertyChanged(ref this.parent, value);
        }

        public string DisplayName {
            get => this.Model.DisplayName;
            set => this.RaisePropertyChanged(ref this.Model.DisplayName, value);
        }

        public SceneObjectViewModel(SceneObject model) {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.children = new ObservableCollection<SceneObjectViewModel>();
            this.Children = new ReadOnlyObservableCollection<SceneObjectViewModel>(this.children);
            foreach (SceneObject root in model.Children) {
                this.AddInternal(SORegistry.Instance.CreateViewModelFromModel(root));
            }
        }

        public void Add(SceneObjectViewModel obj) {
            ValidateHasNoParent(obj);
            if (this.children.Contains(obj)) {
                throw new Exception("Object is already stored in this object");
            }

            this.AddInternal(obj);
        }

        private void AddInternal(SceneObjectViewModel obj) {
            obj.parent = this;
            this.children.Add(obj);
            obj.RaisePropertyChanged(nameof(obj.Parent));
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
    }
}