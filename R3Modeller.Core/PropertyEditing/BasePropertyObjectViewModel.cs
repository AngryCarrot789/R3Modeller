using System;

namespace R3Modeller.Core.PropertyEditing {
    /// <summary>
    /// The base class for property groups and editors
    /// </summary>
    public class BasePropertyObjectViewModel : BaseViewModel {
        private bool isCurrentlyApplicable;

        /// <summary>
        /// Whether or not this item should be visible to the end user or not.
        /// Not taking this into account and showing it anyway may result a crashing
        /// </summary>
        public bool IsCurrentlyApplicable {
            get => this.isCurrentlyApplicable;
            set => this.RaisePropertyChanged(ref this.isCurrentlyApplicable, value);
        }
        
        /// <summary>
        /// The lowest applicable type. This will be null for the root group container. A valid group will contain a non-null applicable type
        /// </summary>
        public Type ApplicableType { get; }

        public BasePropertyObjectViewModel(Type applicableType) {
            this.ApplicableType = applicableType;
        }

        /// <summary>
        /// Whether or not this object is applicable to the given object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool IsApplicable(object value) {
            return this.ApplicableType.IsInstanceOfType(value);
        }
    }
}