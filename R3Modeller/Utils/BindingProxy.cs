using System.Windows;

namespace R3Modeller.Utils {
    public class BindingProxy : Freezable {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy));

        public object Data {
            get => this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore() => new BindingProxy();
    }
}