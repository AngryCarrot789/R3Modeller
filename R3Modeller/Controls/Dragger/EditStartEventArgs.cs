using System.Windows;

namespace R3Modeller.Controls.Dragger {
    public class EditStartEventArgs : RoutedEventArgs {
        public EditStartEventArgs() : base(NumberDragger.EditStartedEvent) {
            
        }
    }
}