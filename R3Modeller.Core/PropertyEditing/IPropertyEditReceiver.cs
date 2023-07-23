using System.ComponentModel;

namespace R3Modeller.Core.PropertyEditing {
    public interface IPropertyEditReceiver : INotifyPropertyChanged {
        void OnExternalPropertyModified(BasePropertyViewModel handler, string property);
    }
}