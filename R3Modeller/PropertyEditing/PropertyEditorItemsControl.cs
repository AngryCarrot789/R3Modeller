using System.Windows;
using System.Windows.Controls;

namespace R3Modeller.PropertyEditing {
    public class PropertyEditorItemsControl : ItemsControl {
        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is PropertyEditorItemContainer;
        }

        protected override DependencyObject GetContainerForItemOverride() {
            return new PropertyEditorItemContainer();
        }
    }
}