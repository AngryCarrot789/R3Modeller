using System;
using System.Windows.Input;
using R3Modeller.Core.PropertyEditing;

namespace R3Modeller.Core.Engine.ViewModels {
    public class CameraViewModel : BaseViewModel, IPropertyEditReceiver {
        public Camera Model { get; }

        public float OrbitRange {
            get => this.Model.orbitRange;
            set {
                this.Model.SetOrbitRange(value);
                this.RaisePropertyChanged(nameof(this.OrbitRange));
                this.OnOrbitRangeUpdated();
            }
        }

        public float Fov {
            get => this.Model.fov;
            set {
                this.Model.SetFov(value);
                this.RaisePropertyChanged(nameof(this.Fov));
                this.OnFovUpdated();
            }
        }

        private bool isModifyingOrbitRange;
        private bool isModifyingFov;

        public RelayCommand BeginOrbitRangeModificationCommand { get; }
        public RelayCommand EndOrbitRangeModificationCommand { get; }
        public RelayCommand BeginFovModificationCommand { get; }
        public RelayCommand EndFovModificationCommand { get; }

        public RenderViewportViewModel Viewport { get; }

        public CameraViewModel(Camera model, RenderViewportViewModel viewport) {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.Viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
            this.BeginOrbitRangeModificationCommand = new RelayCommand(() => this.isModifyingOrbitRange = true, () => !this.isModifyingOrbitRange);
            this.EndOrbitRangeModificationCommand = new RelayCommand(() => {
                this.isModifyingOrbitRange = false;
                this.OnOrbitRangeUpdated();
            }, () => this.isModifyingOrbitRange);

            this.BeginFovModificationCommand = new RelayCommand(() => this.isModifyingFov = true, () => !this.isModifyingFov);
            this.EndFovModificationCommand = new RelayCommand(() => {
                this.isModifyingFov = false;
                this.OnFovUpdated();
            }, () => this.isModifyingFov);
        }

        static CameraViewModel() {
        }

        private void OnOrbitRangeUpdated() {
            if (!this.isModifyingOrbitRange) {
                this.Viewport.RecalculateVisibleObjects();
            }

            this.Viewport.InvalidateRender();
        }

        private void OnFovUpdated() {
            if (!this.isModifyingFov) {
                this.Viewport.RecalculateVisibleObjects();
            }

            this.Viewport.InvalidateRender();
        }

        public void OnExternalPropertyModified(BasePropertyEditorViewModel handler, string property) {

        }
    }
}