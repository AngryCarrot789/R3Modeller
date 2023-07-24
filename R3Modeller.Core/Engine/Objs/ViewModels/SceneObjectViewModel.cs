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

        // storage for the rotation properties
        private float yaw;
        private float pitch;
        private float roll;

        /// <summary>
        /// This scene object's underlying model object
        /// </summary>
        public SceneObject Model { get; }

        /// <summary>
        /// A collection of child scene objects stored in this scene object
        /// </summary>
        public ReadOnlyObservableCollection<SceneObjectViewModel> Children { get; }

        // TODO: figure out another way to store the text "n objects selected" as
        // converters don't really work well with observable collections
        public string PropertyEditorText { get; set; }

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

        public Quaternion Rotation {
            get => this.Model.RelativeRotation;
            set {
                this.Model.SetRotation(value);
                this.RaisePropertyChanged(nameof(this.Rotation));
                this.RaisePropertyChanged(nameof(this.Yaw));
                this.RaisePropertyChanged(nameof(this.Pitch));
                this.RaisePropertyChanged(nameof(this.Roll));
                this.UpdateYawPitchRollForQuaternion();
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

        public float PosX { get => this.Pos.X; set => this.Pos = this.Pos.SetX(value); }
        public float PosY { get => this.Pos.Y; set => this.Pos = this.Pos.SetY(value); }
        public float PosZ { get => this.Pos.Z; set => this.Pos = this.Pos.SetZ(value); }

        public float Yaw {
            get { return this.yaw; }
            set {
                this.yaw = value;
                this.UpdateQuaternionForYawPitchRoll();
                this.RaisePropertyChanged(nameof(this.Yaw));
            }
        }

        public float Pitch {
            get { return this.pitch; }
            set {
                this.pitch = value;
                this.UpdateQuaternionForYawPitchRoll();
                this.RaisePropertyChanged(nameof(this.Pitch));
            }
        }

        public float Roll {
            get { return this.roll; }
            set {
                this.roll = value;
                this.UpdateQuaternionForYawPitchRoll();
                this.RaisePropertyChanged(nameof(this.Roll));
            }
        }

        public float ScaleX { get => this.Scale.X; set => this.Scale = this.Scale.SetX(value); }
        public float ScaleY { get => this.Scale.Y; set => this.Scale = this.Scale.SetY(value); }
        public float ScaleZ { get => this.Scale.Z; set => this.Scale = this.Scale.SetZ(value); }

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

        public SceneObjectViewModel(SceneObject model) {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.children = new ObservableCollection<SceneObjectViewModel>();
            this.Children = new ReadOnlyObservableCollection<SceneObjectViewModel>(this.children);
            this.children.CollectionChanged += (sender, args) => {
                this.PropertyEditorText = $"{this.children.Count} item{Lang.S(this.children.Count)} selected";
                this.RaisePropertyChanged(nameof(this.PropertyEditorText));
            };
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

        private void UpdateQuaternionForYawPitchRoll() {
            double radYaw = this.yaw * (Math.PI / 180.0);
            double radPitch = this.pitch * (Math.PI / 180.0);
            double radRoll = this.roll * (Math.PI / 180.0);
            this.Rotation = Quaternion.CreateFromYawPitchRoll((float) radYaw, (float) radPitch, (float) radRoll);
        }

        private void UpdateYawPitchRollForQuaternion() {
            Quaternion q = this.Rotation;
            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            double rollRad = Math.Atan2(2.0 * (q.Y * q.Z + q.W * q.X), (sqw - sqx - sqy + sqz));
            this.roll = (float)(rollRad * (180.0 / Math.PI));

            double pitchRad = Math.Asin(-2.0 * (q.X * q.Z - q.W * q.Y));
            this.pitch = (float)(pitchRad * (180.0 / Math.PI));

            double yawRad = Math.Atan2(2.0 * (q.X * q.Y + q.W * q.Z), (sqw + sqx - sqy - sqz));
            this.yaw = (float)(yawRad * (180.0 / Math.PI));
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