using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Collections;
using OpenTK.Graphics.OpenGL;

namespace R3Modeller.Core.PropertyEditing {
    public class NumberDraggerViewModel : BasePropertyEditorViewModel {
        private double value;
        public double Value {
            get => this.value;
            set {
                double oldValue = this.value;
                this.RaisePropertyChanged(ref this.value, value);
                this.OnValueChanged(oldValue, value);
            }
        }

        private double minValue;
        public double MinValue {
            get => this.minValue;
            set => this.RaisePropertyChanged(ref this.minValue, value);
        }

        private double maxValue;
        public double MaxValue {
            get => this.maxValue;
            set => this.RaisePropertyChanged(ref this.maxValue, value);
        }

        private bool isEditingValue; // the user has their mouse down

        public ICommand BeginValueModificationCommand { get; }
        public ICommand EndValueModificationCommand { get; }

        public NumberDraggerViewModel() {
            this.BeginValueModificationCommand = new RelayCommand(() => this.isEditingValue = true, () => !this.isEditingValue);
            this.EndValueModificationCommand = new RelayCommand(() => this.isEditingValue = false, () => this.isEditingValue);
        }

        static NumberDraggerViewModel() {

        }

        private void OnValueChanged(double oldValue, double newValue) {
            // if (!this.HasHandlers) {
            //     return;
            // }
//
            // if (this.Handlers.Count == 1) {
            //     this.Handlers[0].setter(newValue);
            // }
            // else if (this.isEditingValue) {
            //     double change = newValue - oldValue;
            //     foreach ((Func<double> getter, Action<double> setter) handler in this.Handlers) {
            //         double val = handler.getter();
            //         handler.setter(val + change);
            //     }
            // }
            // else {
            //     foreach ((Func<double> getter, Action<double> setter) handler in this.Handlers) {
            //         handler.setter(newValue);
            //     }
            // }
        }

        protected override PropertyHandler NewHandler(object target) {
            return new ObjectData(target, null, null);
        }

        private class ObjectData : PropertyHandler {
            public readonly Func<object, double> getter;
            public readonly Action<object, double> setter;

            public double accumulator;

            public ObjectData(object handler, Func<object, double> getter, Action<object, double> setter) : base(handler) {
                this.getter = getter;
                this.setter = setter;
            }
        }
    }
}