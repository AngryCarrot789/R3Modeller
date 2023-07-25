using System;
using System.Collections.Generic;
using System.Linq;

namespace R3Modeller.Core.PropertyEditing {
    /// <summary>
    /// The base property editor view model class for handling a single (or multiple) properties, and updating a collection of handlers
    /// </summary>
    public abstract class BasePropertyEditorViewModel : BasePropertyObjectViewModel {
        private readonly Dictionary<object, PropertyHandler> handlerToDataMap;
        private readonly List<object> handlerList;

        /// <summary>
        /// The list of active handlers
        /// </summary>
        public IReadOnlyList<object> Handlers => this.handlerList;

        /// <summary>
        /// The unordered backing collection of property handler data. This collection may be incomplete as handler data is created on-demand
        /// </summary>
        public IReadOnlyCollection<PropertyHandler> HandlerData => this.handlerToDataMap.Values;

        /// <summary>
        /// Whether or not there are handlers currently using this property editor. Inverse of <see cref="IsEmpty"/>
        /// </summary>
        public bool HasHandlers => this.handlerList.Count > 0;

        /// <summary>
        /// Whether or not there are no handlers currently using this property editor. Inverse of <see cref="HasHandlers"/>
        /// </summary>
        public bool IsEmpty => this.handlerList.Count < 1;

        /// <summary>
        /// Whether or not this editor has more than 1 handler
        /// </summary>
        public bool IsMultiSelection => this.handlerList.Count > 1;

        protected BasePropertyEditorViewModel(Type applicableType) : base(applicableType) {
            this.handlerToDataMap = new Dictionary<object, PropertyHandler>();
            this.handlerList = new List<object>();
        }

        /// <summary>
        /// Attempts to extract a value which is equal across all objects, using the given getter function
        /// </summary>
        /// <param name="objects">Input objects</param>
        /// <param name="getter">Getter function</param>
        /// <param name="equal">
        /// The value that is equal across all objects (if <see cref="T"/> is a reference type,
        /// it will be a reference to to <see cref="objects"/>[0]'s value)
        /// </param>
        /// <typeparam name="T">Type of object to get</typeparam>
        /// <returns>True if there is 1 or more objects and they all contain the an value, otherwise false</returns>
        public static bool GetValueForObjects<T>(IReadOnlyList<object> objects, Func<object, T> getter, out T equal) {
            if (objects == null || objects.Count < 1) {
                equal = default;
                return false;
            }
            else if (objects.Count > 1) { // handle multiple selection separately to reduce usage of EqualityComparer
                EqualityComparer<T> comparator = EqualityComparer<T>.Default;
                equal = getter(objects[0]);
                for (int i = 1, c = objects.Count; i < c; i++) {
                    if (!comparator.Equals(getter(objects[i]), equal)) {
                        return false;
                    }
                }

                return true;
            }
            else {
                equal = getter(objects[0]);
                return true;
            }
        }

        public void ClearHandlers() {
            if (this.handlerList.Count < 1)
            {
                return;
            }

            this.OnClearHandlers();
            this.handlerList.Clear();
            this.handlerToDataMap.Clear();
            this.RaisePropertyChanged(nameof(this.HasHandlers));
            this.RaisePropertyChanged(nameof(this.IsEmpty));
            this.RaisePropertyChanged(nameof(this.IsMultiSelection));
        }

        public void LoadHandlers(IEnumerable<object> targets) {
            this.OnClearHandlers();
            foreach (object entry in targets) {
                this.handlerToDataMap[entry] = null;
                this.handlerList.Add(entry);
            }

            this.OnHandlersLoaded();
            this.RaisePropertyChanged(nameof(this.HasHandlers));
            this.RaisePropertyChanged(nameof(this.IsEmpty));
            this.RaisePropertyChanged(nameof(this.IsMultiSelection));
        }

        /// <summary>
        /// Creates a new instance of the property handler for a specific target.
        /// This is invoked on demand when an instance is required
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual PropertyHandler NewHandler(object target) => new PropertyHandler(target);

        /// <summary>
        /// Creates an instance of the <see cref="PropertyHandler"/> for each object currently loaded, to save dynamic creation
        /// <para>
        /// There are very few reason to use this
        /// </para>
        /// </summary>
        protected void PreallocateHandlerData() {
            foreach (object obj in this.handlerList) {
                this.handlerToDataMap[obj] = this.NewHandler(obj);
            }
        }

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

        protected virtual PropertyHandler GetHandlerData(int index) {
            object target = this.Handlers[index];
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
    }
}