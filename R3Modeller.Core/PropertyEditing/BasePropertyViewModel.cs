using System;
using System.Collections.Generic;
using System.Linq;

namespace R3Modeller.Core.PropertyEditing {
    public abstract class BasePropertyViewModel : BaseViewModel {
        private readonly Dictionary<object, PropertyHandler> handlers;

        public IReadOnlyDictionary<object, PropertyHandler> Handlers => this.handlers;

        public bool HasHandlers => this.Handlers == null || this.Handlers.Count < 1;

        protected BasePropertyViewModel() {
            this.handlers = new Dictionary<object, PropertyHandler>();
        }

        public void LoadHandlers(IEnumerable<object> targets) {
            this.handlers.Clear();
            foreach (object entry in targets) {
                if (this.handlers.ContainsKey(entry)) {
                    this.handlers.Clear();
                    throw new Exception("Duplicate handler object");
                }

                PropertyHandler handler = this.NewHandler(entry);
                this.handlers[entry] = handler;
            }

            this.RaisePropertyChanged(nameof(this.HasHandlers));
        }

        protected abstract PropertyHandler NewHandler(object target);

        protected T GetHandler<T>(object target) where T : PropertyHandler {
            return (T) this.handlers[target];
        }

        protected IEnumerable<T> GetHandlers<T>() where T : PropertyHandler {
            return this.handlers.Values.Cast<T>();
        }
    }
}