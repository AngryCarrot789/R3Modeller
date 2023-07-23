using System.ComponentModel;

namespace R3Modeller.Core.PropertyEditing {
    public interface IPropertyEditReceiver : INotifyPropertyChanged {
        void OnExternalPropertyModified(BasePropertyEditorViewModel handler, string property);
    }
}