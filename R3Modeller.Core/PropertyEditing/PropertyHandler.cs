namespace R3Modeller.Core.PropertyEditing {
    /// <summary>
    /// A class for storing data about a specific handler
    /// </summary>
    public abstract class PropertyHandler {
        /// <summary>
        /// The handler that can be modified by the property
        /// </summary>
        public object Handler { get; }

        protected PropertyHandler(object handler) {
            this.Handler = handler;
        }
    }
}