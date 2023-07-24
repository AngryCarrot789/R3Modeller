using System;
using System.ComponentModel;
using System.Numerics;
using System.Windows.Input;
using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.PropertyEditing.Editors.Scenes {
    public class TransformationEditorViewModel : BasePropertyEditorViewModel {
        private Vector3 pos;

        private Vector3 scale;

        private float pitch;
        private float yaw;
        private float roll;
        private Quaternion rot;

        // A lot of this code may end up being duplicated between the scene object view model and this class...

        public float PosX {
            get => this.pos.X;
            set => this.OnModifiedPos(this.pos, this.pos.WithX(value));
        }

        public float PosY {
            get => this.pos.Y;
            set => this.OnModifiedPos(this.pos, this.pos.WithY(value));
        }

        public float PosZ {
            get => this.pos.Z;
            set => this.OnModifiedPos(this.pos, this.pos.WithZ(value));
        }

        public float Pitch {
            get => this.pitch;
            set {
                this.pitch = value;
                this.OnModifiedRotation(this.rot, this.GetQuaternionForYawPitchRoll());
            }
        }

        public float Yaw {
            get => this.yaw;
            set {
                this.yaw = value;
                this.OnModifiedRotation(this.rot, this.GetQuaternionForYawPitchRoll());
            }
        }

        public float Roll {
            get => this.roll;
            set {
                this.roll = value;
                this.OnModifiedRotation(this.rot, this.GetQuaternionForYawPitchRoll());
            }
        }

        public float ScaleX { get => this.scale.X; set => this.OnModifiedScale(this.scale, this.scale.WithX(value)); }
        public float ScaleY { get => this.scale.Y; set => this.OnModifiedScale(this.scale, this.scale.WithY(value)); }
        public float ScaleZ { get => this.scale.Z; set => this.OnModifiedScale(this.scale, this.scale.WithZ(value)); }

        // the user has their mouse down
        private bool isEditingPosX;
        private bool isEditingPosY;
        private bool isEditingPosZ;
        private bool isEditingScaleX;
        private bool isEditingScaleY;
        private bool isEditingScaleZ;
        private bool isEditingYaw;
        private bool isEditingPitch;
        private bool isEditingRoll;

        public ICommand BeginEditPosXCommand { get; }
        public ICommand BeginEditPosYCommand { get; }
        public ICommand BeginEditPosZCommand { get; }
        public ICommand FinishEditPosXCommand { get; }
        public ICommand FinishEditPosYCommand { get; }
        public ICommand FinishEditPosZCommand { get; }

        public ICommand BeginEditScaleXCommand { get; }
        public ICommand BeginEditScaleYCommand { get; }
        public ICommand BeginEditScaleZCommand { get; }
        public ICommand FinishEditScaleXCommand { get; }
        public ICommand FinishEditScaleYCommand { get; }
        public ICommand FinishEditScaleZCommand { get; }

        public ICommand BeginEditYawCommand { get; }
        public ICommand BeginEditPitchCommand { get; }
        public ICommand BeginEditRollCommand { get; }
        public ICommand FinishEditYawCommand { get; }
        public ICommand FinishEditPitchCommand { get; }
        public ICommand FinishEditRollCommand { get; }

        // private readonly PropertyChangedEventHandler handlerPropertyChangedEventHandler;

        public TransformationEditorViewModel(Type applicableType) : base(applicableType) {
            // this.handlerPropertyChangedEventHandler = this.OnHandlerPropertyChanged;
            // This is why the property editors are mostly singletons for each view port LOL
            this.BeginEditPosXCommand = new RelayCommand(() => this.isEditingPosX = true, () => !this.isEditingPosX);
            this.BeginEditPosYCommand = new RelayCommand(() => this.isEditingPosY = true, () => !this.isEditingPosY);
            this.BeginEditPosZCommand = new RelayCommand(() => this.isEditingPosZ = true, () => !this.isEditingPosZ);
            this.FinishEditPosXCommand = new RelayCommand(() => this.isEditingPosX = false, () => this.isEditingPosX);
            this.FinishEditPosYCommand = new RelayCommand(() => this.isEditingPosY = false, () => this.isEditingPosY);
            this.FinishEditPosZCommand = new RelayCommand(() => this.isEditingPosZ = false, () => this.isEditingPosZ);
            this.BeginEditScaleXCommand = new RelayCommand(() => this.isEditingScaleX = true, () => !this.isEditingScaleX);
            this.BeginEditScaleYCommand = new RelayCommand(() => this.isEditingScaleY = true, () => !this.isEditingScaleY);
            this.BeginEditScaleZCommand = new RelayCommand(() => this.isEditingScaleZ = true, () => !this.isEditingScaleZ);
            this.FinishEditScaleXCommand = new RelayCommand(() => this.isEditingScaleX = false, () => this.isEditingScaleX);
            this.FinishEditScaleYCommand = new RelayCommand(() => this.isEditingScaleY = false, () => this.isEditingScaleY);
            this.FinishEditScaleZCommand = new RelayCommand(() => this.isEditingScaleZ = false, () => this.isEditingScaleZ);
            this.BeginEditYawCommand = new RelayCommand(() => this.isEditingYaw = true, () => !this.isEditingYaw);
            this.BeginEditPitchCommand = new RelayCommand(() => this.isEditingPitch = true, () => !this.isEditingPitch);
            this.BeginEditRollCommand = new RelayCommand(() => this.isEditingRoll = true, () => !this.isEditingRoll);
            this.FinishEditYawCommand = new RelayCommand(() => this.isEditingYaw = false, () => this.isEditingYaw);
            this.FinishEditPitchCommand = new RelayCommand(() => this.isEditingPitch = false, () => this.isEditingPitch);
            this.FinishEditRollCommand = new RelayCommand(() => this.isEditingRoll = false, () => this.isEditingRoll);
        }

        private void OnModifiedPos(Vector3 oldVal, Vector3 newVal) {
            // more convenient and cleaner to update the pos here than in the property setter block
            this.pos = newVal;
            this.RaisePropertyChanged(nameof(this.PosX));
            this.RaisePropertyChanged(nameof(this.PosY));
            this.RaisePropertyChanged(nameof(this.PosZ));
            if (this.IsEmpty) {
                return;
            }

            if (this.Handlers.Count == 1) {
                ((SceneObjectViewModel) this.Handlers[0]).Pos = newVal;
            }
            else {
                Vector3 change = newVal - oldVal;
                foreach (TransformationHandlerData obj in this.GetHandlersData<TransformationHandlerData>()) {
                    Vector3 p = obj.Handler.Pos;
                    if (this.isEditingPosX || this.isEditingPosY || this.isEditingPosZ) {
                        p += change;
                    }
                    else {
                        p = newVal;
                    }

                    obj.Handler.Pos = p;
                }
            }
        }

        private void OnModifiedRotation(Quaternion oldVal, Quaternion newVal) {
            // more convenient and cleaner to update the pos here than in the property setter block
            this.rot = newVal;
            this.RaisePropertyChanged(nameof(this.Pitch));
            this.RaisePropertyChanged(nameof(this.Yaw));
            this.RaisePropertyChanged(nameof(this.Roll));
            if (this.IsEmpty) {
                return;
            }

            if (this.Handlers.Count == 1) {
                ((SceneObjectViewModel) this.Handlers[0]).Rotation = newVal;
            }
            else {
                Quaternion change = Quaternion.Inverse(oldVal) * newVal;
                foreach (TransformationHandlerData obj in this.GetHandlersData<TransformationHandlerData>()) {
                    Quaternion rotation;
                    if (this.isEditingYaw || this.isEditingPitch || this.isEditingRoll) {
                        rotation = obj.Handler.Rotation * change;
                    }
                    else {
                        rotation = newVal;
                    }

                    obj.Handler.Rotation = rotation;
                }
            }
        }

        private void OnModifiedScale(Vector3 oldVal, Vector3 newVal) {
            // more convenient and cleaner to update the scale here than in the property setter block
            this.scale = newVal;
            this.RaisePropertyChanged(nameof(this.ScaleX));
            this.RaisePropertyChanged(nameof(this.ScaleY));
            this.RaisePropertyChanged(nameof(this.ScaleZ));
            if (this.IsEmpty) {
                return;
            }

            if (this.Handlers.Count == 1) {
                ((SceneObjectViewModel) this.Handlers[0]).Scale = newVal;
            }
            else {
                // Vector3 change;
                // if (this.isEditingScaleX || this.isEditingScaleY || this.isEditingScaleZ) {
                //     change = newVal / oldVal;
                // }
                // else {
                //     // If not editing any individual component, set the entire scale directly
                //     change = newVal;
                // }
                // foreach (TransformationHandlerData obj in this.GetHandlersData<TransformationHandlerData>()) {
                //     Vector3 s = obj.Handler.Scale;
                //     if (this.isEditingScaleX) {
                //         s.X *= change.X;
                //     }
                //     if (this.isEditingScaleY) {
                //         s.Y *= change.Y;
                //     }
                //     if (this.isEditingScaleZ) {
                //         s.Z *= change.Z;
                //     }
                //     obj.Handler.Scale = s;
                // }
                Vector3 change = newVal - oldVal;
                foreach (TransformationHandlerData obj in this.GetHandlersData<TransformationHandlerData>()) {
                    Vector3 s = obj.Handler.Scale;
                    if (this.isEditingScaleX || this.isEditingScaleY || this.isEditingScaleZ) {
                        s += change;
                    }
                    else {
                        s = newVal;
                    }
                    obj.Handler.Scale = s;
                }
            }
        }

        private Quaternion GetQuaternionForYawPitchRoll() {
            double radPitch = this.pitch * (Math.PI / 180.0);
            double radYaw = this.yaw * (Math.PI / 180.0);
            double radRoll = this.roll * (Math.PI / 180.0);
            return Quaternion.CreateFromYawPitchRoll((float) radYaw, (float) radPitch, (float) radRoll);
        }

        protected override void OnHandlersLoaded() {
            base.OnHandlersLoaded();
            if (this.IsEmpty) {
                return;
            }

            if (this.IsMultiSelection) {
                this.pos = new Vector3();
                this.scale = Vector3.One;
                this.rot = Quaternion.Identity;
            }
            else {
                SceneObjectViewModel first = (SceneObjectViewModel) this.Handlers[0];
                this.pos = first.Pos;
                this.scale = first.Scale;
                this.rot = first.Rotation;
            }

            Quaternion q = this.rot;
            // Extract yaw, pitch, and roll from the quaternion
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            this.roll = (float) (Math.Atan2(sinr_cosp, cosr_cosp) * (180.0 / Math.PI));

            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
                this.pitch = (float) (Math.Sign(sinp) * (Math.PI / 2) * (180.0 / Math.PI)); // Use 90 degrees if out of range
            else
                this.pitch = (float) (Math.Asin(sinp) * (180.0 / Math.PI));

            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            this.yaw = (float) (Math.Atan2(siny_cosp, cosy_cosp) * (180.0 / Math.PI));

            this.RaisePropertyChanged(nameof(this.PosX));
            this.RaisePropertyChanged(nameof(this.PosY));
            this.RaisePropertyChanged(nameof(this.PosZ));
            this.RaisePropertyChanged(nameof(this.ScaleX));
            this.RaisePropertyChanged(nameof(this.ScaleY));
            this.RaisePropertyChanged(nameof(this.ScaleZ));
            this.RaisePropertyChanged(nameof(this.Pitch));
            this.RaisePropertyChanged(nameof(this.Yaw));
            this.RaisePropertyChanged(nameof(this.Roll));
            // foreach (object handler in this.Handlers) {
            //     ((INotifyPropertyChanged) handler).PropertyChanged += this.handlerPropertyChangedEventHandler;
            // }
        }

        protected override void OnClearHandlers() {
            base.OnClearHandlers();
            // foreach (object handler in this.Handlers) {
            //     ((INotifyPropertyChanged) handler).PropertyChanged -= this.handlerPropertyChangedEventHandler;
            // }
        }

        // private void OnHandlerPropertyChanged(object sender, PropertyChangedEventArgs e) {
        //     switch (e.PropertyName) {
        //         case nameof(SceneObjectViewModel.Pos): {
        //             this.pos = ((SceneObjectViewModel) sender).Pos;
        //             this.RaisePropertyChanged(nameof(this.PosX));
        //             this.RaisePropertyChanged(nameof(this.PosY));
        //             this.RaisePropertyChanged(nameof(this.PosZ));
        //             break;
        //         }
        //         case nameof(SceneObjectViewModel.Scale): {
        //             this.RaisePropertyChanged(nameof(this.ScaleX));
        //             this.RaisePropertyChanged(nameof(this.ScaleY));
        //             this.RaisePropertyChanged(nameof(this.ScaleZ));
        //             break;
        //         }
        //         case nameof(SceneObjectViewModel.Rotation): {
        //             this.RaisePropertyChanged(nameof(this.Pitch));
        //             this.RaisePropertyChanged(nameof(this.Yaw));
        //             this.RaisePropertyChanged(nameof(this.Roll));
        //             break;
        //         }
        //     }
        // }

        protected override BasePropertyEditorViewModel NewInstance() {
            return new TransformationEditorViewModel(this.ApplicableType);
        }

        protected override PropertyHandler NewHandler(object target) => new TransformationHandlerData((SceneObjectViewModel) target);

        private class TransformationHandlerData : PropertyHandler {
            public new SceneObjectViewModel Handler => (SceneObjectViewModel) base.Handler;

            public TransformationHandlerData(object handler) : base(handler) {
            }
        }
    }
}