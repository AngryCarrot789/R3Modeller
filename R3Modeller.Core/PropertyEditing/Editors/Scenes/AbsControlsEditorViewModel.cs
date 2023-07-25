using System;
using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.PropertyEditing.Editors.Primitives;

namespace R3Modeller.Core.PropertyEditing.Editors.Scenes {
    public class AbsControlsEditorViewModel: BasePropertyEditorViewModel {
        private bool? isAbsolutePos;
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

        private bool? isAbsoluteScale;
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

        private bool? isAbsoluteRotation;
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

        public AbsControlsEditorViewModel(Type applicableType) : base(applicableType) {

        }

        public bool? CalculateDefaultPosition() => CheckBoxEditorViewModel.GetDefaultBool(this.Handlers, (x) => ((SceneObjectViewModel) x).IsPositionAbsolute);
        public bool? CalculateDefaultScale() => CheckBoxEditorViewModel.GetDefaultBool(this.Handlers, (x) => ((SceneObjectViewModel) x).IsScaleAbsolute);
        public bool? CalculateDefaultRotation() => CheckBoxEditorViewModel.GetDefaultBool(this.Handlers, (x) => ((SceneObjectViewModel) x).IsRotationAbsolute);

        protected override void OnHandlersLoaded() {
            base.OnHandlersLoaded();
            if (!this.IsEmpty) {
                this.isAbsolutePos = this.CalculateDefaultPosition();
                this.isAbsoluteScale = this.CalculateDefaultScale();
                this.isAbsoluteRotation = this.CalculateDefaultRotation();
                this.RaisePropertyChanged(nameof(this.IsAbsolutePos));
                this.RaisePropertyChanged(nameof(this.IsAbsoluteScale));
                this.RaisePropertyChanged(nameof(this.IsAbsoluteRotation));
            }
        }
    }
}