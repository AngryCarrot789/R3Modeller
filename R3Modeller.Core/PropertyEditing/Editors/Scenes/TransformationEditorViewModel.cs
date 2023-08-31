using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Input;
using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.Utils;

namespace R3Modeller.Core.PropertyEditing.Editors.Scenes {
    public class TransformationEditorViewModel : BasePropertyEditorViewModel {
        private Vector3 pos;
        private Vector3 scale;
        private Vector3 rot;
        private bool? isAbsolutePos;
        private bool? isAbsoluteScale;
        private bool? isAbsoluteRotation;

        // A lot of this code may end up being duplicated between the scene object view model and this class...

        public float PosX { get => this.pos.X; set => this.OnModifiedPos(this.pos, this.pos.WithX(value)); }
        public float PosY { get => this.pos.Y; set => this.OnModifiedPos(this.pos, this.pos.WithY(value)); }
        public float PosZ { get => this.pos.Z; set => this.OnModifiedPos(this.pos, this.pos.WithZ(value)); }
        public float Pitch { get => this.rot.X; set => this.OnModifiedRotation(this.rot, this.rot.WithX(value)); }
        public float Yaw { get => this.rot.Y; set => this.OnModifiedRotation(this.rot, this.rot.WithY(value)); }
        public float Roll { get => this.rot.Z; set => this.OnModifiedRotation(this.rot, this.rot.WithZ(value)); }
        public float ScaleX { get => this.scale.X; set => this.OnModifiedScale(this.scale, this.scale.WithX(value)); }
        public float ScaleY { get => this.scale.Y; set => this.OnModifiedScale(this.scale, this.scale.WithY(value)); }
        public float ScaleZ { get => this.scale.Z; set => this.OnModifiedScale(this.scale, this.scale.WithZ(value)); }

        public bool? IsAbsolutePos {
            get => this.isAbsolutePos;
            set {
                bool? old = this.isAbsolutePos;
                if (!old.HasValue && !value.HasValue || old.HasValue && value.HasValue && old.Value == value.Value) {
                    return;
                }

                bool val = value ?? false;
                this.RaisePropertyChanged(ref this.isAbsolutePos, value);
                foreach (object handler in this.Handlers) {
                    ((SceneObjectViewModel) handler).IsPositionAbsolute = val;
                }
            }
        }

        public bool? IsAbsoluteScale {
            get => this.isAbsoluteScale;
            set {
                bool? old = this.isAbsoluteScale;
                if (!old.HasValue && !value.HasValue || old.HasValue && value.HasValue && old.Value == value.Value) {
                    return;
                }

                bool val = value ?? false;
                this.RaisePropertyChanged(ref this.isAbsoluteScale, value);
                foreach (object handler in this.Handlers) {
                    ((SceneObjectViewModel) handler).IsScaleAbsolute = val;
                }
            }
        }

        public bool? IsAbsoluteRotation {
            get => this.isAbsoluteRotation;
            set {
                bool? old = this.isAbsoluteRotation;
                if (!old.HasValue && !value.HasValue || old.HasValue && value.HasValue && old.Value == value.Value) {
                    return;
                }

                bool val = value ?? false;
                this.RaisePropertyChanged(ref this.isAbsoluteRotation, value);
                foreach (object handler in this.Handlers) {
                    ((SceneObjectViewModel) handler).IsRotationAbsolute = val;
                }
            }
        }

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

        public TransformationEditorViewModel(Type applicableType) : base(applicableType) {
            // This is why the property editors are gonna be singletons for each view port LOL
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

        public bool? CalculateDefaultPosition() => GetEqualValue(this.Handlers, (x) => ((SceneObjectViewModel) x).IsPositionAbsolute, out bool v) ? v : (bool?) null;
        public bool? CalculateDefaultScale() => GetEqualValue(this.Handlers, (x) => ((SceneObjectViewModel) x).IsScaleAbsolute, out bool v) ? v : (bool?) null;
        public bool? CalculateDefaultRotation() => GetEqualValue(this.Handlers, (x) => ((SceneObjectViewModel) x).IsRotationAbsolute, out bool v) ? v : (bool?) null;

#if RELEASE
        public override bool IsApplicable(object value) => value is SceneObjectViewModel;
#endif

        private void OnModifiedPos(Vector3 oldVal, Vector3 newVal) {
            this.pos = newVal;
            this.RaisePositionChanged();
            if (this.Handlers.Count > 1 && (this.isEditingPosX || this.isEditingPosY || this.isEditingPosZ)) {
                Vector3 change = newVal - oldVal;
                foreach (object target in this.Handlers) {
                    ((SceneObjectViewModel) target).Pos += change;
                }
            }
            else {
                foreach (object target in this.Handlers) {
                    ((SceneObjectViewModel) target).Pos = newVal;
                }
            }
        }

        private void OnModifiedRotation(Vector3 oldVal, Vector3 newVal) {
            this.rot = newVal;
            this.RaiseRotationChanged();
            if (this.Handlers.Count > 1 && (this.isEditingYaw || this.isEditingPitch || this.isEditingRoll)) {
                Vector3 change = newVal - oldVal;
                foreach (object target in this.Handlers) {
                    ((SceneObjectViewModel) target).PitchYawRoll += change;
                }
            }
            else {
                foreach (object target in this.Handlers) {
                    ((SceneObjectViewModel) target).PitchYawRoll = newVal;
                }
            }
        }

        // private void OnModifiedRotation(Quaternion oldVal, Quaternion newVal) {
        //     // more convenient and cleaner to update the pos here than in the property setter block
        //     this.rot = newVal;
        //     this.RaisePropertyChanged(nameof(this.Pitch));
        //     this.RaisePropertyChanged(nameof(this.Yaw));
        //     this.RaisePropertyChanged(nameof(this.Roll));
        //     if (this.IsEmpty) {
        //         return;
        //     }
        //
        //     if (this.Handlers.Count == 1) {
        //         ((SceneObjectViewModel) this.Handlers[0]).PitchYawRoll = newVal;
        //     }
        //     else {
        //         Quaternion change = Quaternion.Inverse(oldVal) * newVal;
        //         foreach (HandlerData obj in this.GetHandlersData<HandlerData>()) {
        //             Quaternion rotation;
        //             if (this.isEditingYaw || this.isEditingPitch || this.isEditingRoll) {
        //                 rotation = obj.Handler.PitchYawRoll * change;
        //             }
        //             else {
        //                 rotation = newVal;
        //             }
        //
        //             obj.Handler.PitchYawRoll = rotation;
        //         }
        //     }
        // }

        private void OnModifiedScale(Vector3 oldVal, Vector3 newVal) {
            this.scale = newVal;
            this.RaiseScaleChanged();
            if (this.Handlers.Count > 1 && (this.isEditingScaleX || this.isEditingScaleY || this.isEditingScaleZ)) {
                Vector3 change = newVal - oldVal;
                foreach (object target in this.Handlers) {
                    ((SceneObjectViewModel) target).Scale += change;
                }
            }
            else {
                foreach (object target in this.Handlers) {
                    ((SceneObjectViewModel) target).Scale = newVal;
                }
            }
        }

        protected override void OnHandlersLoaded() {
            base.OnHandlersLoaded();
            if (this.IsEmpty) {
                return;
            }

            this.pos = GetEqualValue(this.Handlers, x => ((SceneObjectViewModel) x).Pos, out Vector3 a) ? a : Vector3.Zero;
            this.scale = GetEqualValue(this.Handlers, x => ((SceneObjectViewModel) x).Scale, out Vector3 b) ? b : Vector3.One;
            this.rot = GetEqualValue(this.Handlers, x => ((SceneObjectViewModel) x).PitchYawRoll, out Vector3 c) ? c : Vector3.Zero;

            if (!this.IsEmpty) {
                this.isAbsolutePos = this.CalculateDefaultPosition();
                this.isAbsoluteScale = this.CalculateDefaultScale();
                this.isAbsoluteRotation = this.CalculateDefaultRotation();
                this.RaisePropertyChanged(nameof(this.IsAbsolutePos));
                this.RaisePropertyChanged(nameof(this.IsAbsoluteScale));
                this.RaisePropertyChanged(nameof(this.IsAbsoluteRotation));
            }

            this.RaisePositionChanged();
            this.RaiseRotationChanged();
            this.RaiseScaleChanged();
        }

        private void RaisePositionChanged() {
            this.RaisePropertyChanged(nameof(this.PosX));
            this.RaisePropertyChanged(nameof(this.PosY));
            this.RaisePropertyChanged(nameof(this.PosZ));
        }

        private void RaiseRotationChanged() {
            this.RaisePropertyChanged(nameof(this.Pitch));
            this.RaisePropertyChanged(nameof(this.Yaw));
            this.RaisePropertyChanged(nameof(this.Roll));
        }

        private void RaiseScaleChanged() {
            this.RaisePropertyChanged(nameof(this.ScaleX));
            this.RaisePropertyChanged(nameof(this.ScaleY));
            this.RaisePropertyChanged(nameof(this.ScaleZ));
        }

        protected override PropertyHandler NewHandler(object target) => new TEHandlerData((SceneObjectViewModel) target);

        private class TEHandlerData : PropertyHandler {
            public new SceneObjectViewModel Handler => (SceneObjectViewModel) base.Target;

            public TEHandlerData(object target) : base(target) {
            }
        }
    }
}