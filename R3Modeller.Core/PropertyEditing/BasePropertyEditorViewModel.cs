using System;
using System.Collections.Generic;
using System.Linq;

namespace R3Modeller.Core.PropertyEditing {
    /// <summary>
    /// The base property editor view model class for handling a single (or multiple) properties, and updating a collection of handlers
    /// </summary>
    public abstract class BasePropertyEditorViewModel : BaseViewModel {
        private readonly Dictionary<object, PropertyHandler> handlerToDataMap;
        private readonly List<object> handlerList;

        public IReadOnlyList<object> Handlers => this.handlerList;

        public bool HasHandlers =>  this.handlerList != null && this.handlerList.Count > 0;

        public bool IsMultiSelection => this.handlerList != null && this.handlerList.Count > 1;

        protected BasePropertyEditorViewModel() {
            this.handlerToDataMap = new Dictionary<object, PropertyHandler>();
            this.handlerList = new List<object>();
        }

        public void Clear() {
            this.OnClearHandlers();
            this.handlerToDataMap.Clear();
            this.RaisePropertyChanged(nameof(this.HasHandlers));
            this.RaisePropertyChanged(nameof(this.IsMultiSelection));
        }

        public void LoadHandlers(IEnumerable<object> targets) {
            this.OnClearHandlers();
            this.handlerList.Clear();
            this.handlerToDataMap.Clear();
            foreach (object entry in targets) {
                if (this.handlerToDataMap.ContainsKey(entry)) {
                    this.handlerList.Clear();
                    this.handlerToDataMap.Clear();
                    throw new Exception("Duplicate handler object");
                }

                this.handlerToDataMap[entry] = null;
                this.handlerList.Add(entry);
            }

            this.OnHandlersLoaded();
            this.RaisePropertyChanged(nameof(this.HasHandlers));
            this.RaisePropertyChanged(nameof(this.IsMultiSelection));
        }

        /// <summary>
        /// Creates a new instance of the property handler for a specific target.
        /// This is invoked on demand when an instance is required
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected abstract PropertyHandler NewHandler(object target);

        /// <summary>
        /// Called just before the handlers are cleared
        /// </summary>
        protected virtual void OnClearHandlers() {

        }

        /// <summary>
        /// Called just after all handlers are fulled loaded
        /// </summary>
        protected virtual void OnHandlersLoaded() {

        }

        protected virtual PropertyHandler GetHandlerData(object target) {
            PropertyHandler data = this.handlerToDataMap[target];
            if (data == null)
                this.handlerToDataMap[target] = data = this.NewHandler(target);
            return data;
        }

        protected IEnumerable<T> GetHandlersData<T>() where T : PropertyHandler {
            return this.handlerList.Select(this.GetHandlerData).Cast<T>();
        }

        protected IEnumerable<T> GetHandlers<T>() where T : BaseViewModel {
            return this.handlerList.Cast<T>();
        }

        public BasePropertyEditorViewModel Clone() {
            BasePropertyEditorViewModel editor = this.NewInstance();
            editor.handlerList.AddRange(this.handlerList);
            foreach (KeyValuePair<object,PropertyHandler> entry in this.handlerToDataMap) {
                editor.handlerToDataMap[entry.Key] = entry.Value;
            }

            return editor;
        }

        protected abstract BasePropertyEditorViewModel NewInstance();
    }
}