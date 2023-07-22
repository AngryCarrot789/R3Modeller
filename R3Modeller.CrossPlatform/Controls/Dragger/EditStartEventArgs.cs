using System.Windows;

namespace R3Modeller.CrossPlatform.Controls.Dragger {
    public class EditStartEventArgs : RoutedEventArgs {
        public EditStartEventArgs() : base(NumberDragger.EditStartedEvent) {
            
        }
    }
}