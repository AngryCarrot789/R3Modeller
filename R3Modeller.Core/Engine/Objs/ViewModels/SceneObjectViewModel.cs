using System;
using System.Collections.ObjectModel;
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
            get => this.Model.RelativePosition;
            set {
                this.Model.RelativePosition = value;
                this.RaisePropertyChanged(nameof(this.Pos));
                this.RaisePropertyChanged(nameof(this.PosX));
                this.RaisePropertyChanged(nameof(this.PosY));
                this.RaisePropertyChanged(nameof(this.PosZ));
                this.Project.OnRenderInvalidated();
            }
        }

        public Vector3 PitchYawRoll {
            get => this.Model.RelativePitchYawRoll;
            set {
                this.Model.SetRotation(value);
                this.RaisePropertyChanged(nameof(this.PitchYawRoll));
                this.RaisePropertyChanged(nameof(this.Pitch));
                this.RaisePropertyChanged(nameof(this.Yaw));
                this.RaisePropertyChanged(nameof(this.Roll));
                this.Project.OnRenderInvalidated();
            }
        }

        public Vector3 Scale {
            get => this.Model.RelativeScale;
            set {
                this.Model.RelativeScale = value;
                this.RaisePropertyChanged(nameof(this.Scale));
                this.RaisePropertyChanged(nameof(this.ScaleX));
                this.RaisePropertyChanged(nameof(this.ScaleY));
                this.RaisePropertyChanged(nameof(this.ScaleZ));
                this.Project.OnRenderInvalidated();
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


        public bool IsPositionAbsolute {
            get => this.Model.IsPositionAbsolute;
            set {
                if (value == this.IsPositionAbsolute)
                    return;
                this.Model.IsPositionAbsolute = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.Pos));
                this.RaisePropertyChanged(nameof(this.PosX));
                this.RaisePropertyChanged(nameof(this.PosY));
                this.RaisePropertyChanged(nameof(this.PosZ));
            }
        }

        public bool IsScaleAbsolute {
            get => this.Model.IsScaleAbsolute;
            set {
                if (value == this.IsScaleAbsolute)
                    return;
                this.Model.IsScaleAbsolute = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.Scale));
                this.RaisePropertyChanged(nameof(this.ScaleX));
                this.RaisePropertyChanged(nameof(this.ScaleY));
                this.RaisePropertyChanged(nameof(this.ScaleZ));
            }
        }

        public bool IsRotationAbsolute {
            get => this.Model.IsRotationAbsolute;
            set {
                if (value == this.IsRotationAbsolute)
                    return;
                this.Model.IsRotationAbsolute = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.PitchYawRoll));
                this.RaisePropertyChanged(nameof(this.Pitch));
                this.RaisePropertyChanged(nameof(this.Yaw));
                this.RaisePropertyChanged(nameof(this.Roll));
            }
        }

        /// <summary>
        /// The project associated with this scene object. Should only really be set once
        /// </summary>
        public ProjectViewModel Project { get; set; }

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
                this.Project.OnRenderInvalidated();
            }
        }

        public SceneObjectViewModel(SceneObject model) {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.children = new ObservableCollection<SceneObjectViewModel>();
            this.Children = new ReadOnlyObservableCollection<SceneObjectViewModel>(this.children);
            for (int i = 0, c = model.Items.Count; i < c; i++) {
                this.InsertInternal(i, SORegistry.Instance.CreateViewModelFromModel(model.Items[i]));
            }
        }

        public void Add(SceneObjectViewModel obj) {
            ValidateHasNoParent(obj);
            if (this.children.Contains(obj)) {
                throw new Exception("Object is already stored in this object");
            }

            this.InsertInternal(this.children.Count, obj);
        }

        private void InsertInternal(int index, SceneObjectViewModel obj) {
            obj.parent = this;
            this.children.Insert(index, obj);
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

        public void SetProject(ProjectViewModel project) {
            this.Project = project;
            foreach (SceneObjectViewModel obj in this.children) {
                obj.SetProject(project);
            }
        }
    }
}