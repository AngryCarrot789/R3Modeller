using System.Numerics;
using System.Windows.Input;
using R3Modeller.Core.Engine.Objs.ViewModels;

namespace R3Modeller.Core.PropertyEditing.Editors {
    public class PositionEditorViewModel : BasePropertyEditorViewModel {
        private Vector3 pos;

        public float PosX {
            get => this.pos.X;
            set {
                float oldVal = this.pos.X;
                this.pos.X = value;
                this.RaisePropertyChanged();
                this.OnModifiedPosX(oldVal, value);
            }
        }

        public float PosY {
            get => this.pos.Y;
            set {
                float oldVal = this.pos.Y;
                this.pos.Y = value;
                this.RaisePropertyChanged();
                this.OnModifiedPosY(oldVal, value);
            }
        }

        public float PosZ {
            get => this.pos.Z;
            set {
                float oldVal = this.pos.Z;
                this.pos.Z = value;
                this.RaisePropertyChanged();
                this.OnModifiedPosZ(oldVal, value);
            }
        }

        // the user has their mouse down
        private bool isEditingX;
        private bool isEditingY;
        private bool isEditingZ;

        public ICommand BeginEditXCommand { get; }
        public ICommand BeginEditYCommand { get; }
        public ICommand BeginEditZCommand { get; }
        public ICommand FinishEditXCommand { get; }
        public ICommand FinishEditYCommand { get; }
        public ICommand FinishEditZCommand { get; }

        public PositionEditorViewModel() {
            this.BeginEditXCommand = new RelayCommand(() => this.isEditingX = true, () => !this.isEditingX);
            this.BeginEditYCommand = new RelayCommand(() => this.isEditingY = true, () => !this.isEditingY);
            this.BeginEditZCommand = new RelayCommand(() => this.isEditingZ = true, () => !this.isEditingZ);
            this.FinishEditXCommand = new RelayCommand(() => this.isEditingX = false, () => this.isEditingX);
            this.FinishEditYCommand = new RelayCommand(() => this.isEditingY = false, () => this.isEditingY);
            this.FinishEditZCommand = new RelayCommand(() => this.isEditingZ = false, () => this.isEditingZ);
        }

        private void OnModifiedPosX(float oldVal, float newVal) {
            if (!this.HasHandlers) {
                return;
            }

            if (this.Handlers.Count == 1) {
                ((SceneObjectViewModel) this.Handlers[0]).Pos = this.pos;
            }
            else {
                foreach (PositionEditorHandlerData obj in this.GetHandlersData<PositionEditorHandlerData>()) {
                    Vector3 p = obj.Handler.Pos;
                    obj.Handler.Pos = new Vector3(this.isEditingX ? (p.X + (newVal - oldVal)) : newVal, p.Y, p.Z);
                }
            }
        }

        private void OnModifiedPosY(float oldVal, float newVal) {
            if (!this.HasHandlers) {
                return;
            }

            if (this.Handlers.Count == 1) {
                ((SceneObjectViewModel) this.Handlers[0]).Pos = this.pos;
            }
            else {
                foreach (PositionEditorHandlerData obj in this.GetHandlersData<PositionEditorHandlerData>()) {
                    Vector3 p = obj.Handler.Pos;
                    obj.Handler.Pos = new Vector3(p.X, this.isEditingY ? (p.Y + (newVal - oldVal)) : newVal, p.Z);
                }
            }
        }

        private void OnModifiedPosZ(float oldVal, float newVal) {
            if (!this.HasHandlers) {
                return;
            }

            if (this.Handlers.Count == 1) {
                ((SceneObjectViewModel) this.Handlers[0]).Pos = this.pos;
            }
            else {
                foreach (PositionEditorHandlerData obj in this.GetHandlersData<PositionEditorHandlerData>()) {
                    Vector3 p = obj.Handler.Pos;
                    obj.Handler.Pos = new Vector3(p.X, p.Y, this.isEditingZ ? (p.Z + (newVal - oldVal)) : newVal);
                }
            }
        }

        protected override void OnHandlersLoaded() {
            base.OnHandlersLoaded();
            this.pos = this.Handlers.Count == 1 ? ((SceneObjectViewModel) this.Handlers[0]).Pos : new Vector3();
        }

        protected override BasePropertyEditorViewModel NewInstance() {
            return new PositionEditorViewModel();
        }

        protected override PropertyHandler NewHandler(object target) => new PositionEditorHandlerData((SceneObjectViewModel) target);

        private class PositionEditorHandlerData : PropertyHandler {
            public new SceneObjectViewModel Handler => (SceneObjectViewModel) base.Handler;

            public PositionEditorHandlerData(object handler) : base(handler) {
            }
        }
    }
}