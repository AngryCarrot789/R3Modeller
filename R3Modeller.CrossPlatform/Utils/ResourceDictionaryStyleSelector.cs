using System;
using System.Windows;
using System.Windows.Controls;

namespace R3Modeller.CrossPlatform.Utils {
    public class ResourceDictionaryStyleSelector : StyleSelector {
        public ResourceDictionary ResourceDictionary { get; set; }

        public override Style SelectStyle(object item, DependencyObject container) {
            if (item != null && this.ResourceDictionary != null && this.GetStyle(item.GetType(), out Style style)) {
                return style;
            }

            return base.SelectStyle(item, container);
        }

        private bool GetStyle(Type type, out Style style) {
            for (Type t = type; t != null; t = t.BaseType) {
                if (this.ResourceDictionary[t] is Style s) {
                    style = s;
                    return true;
                }
            }

            style = null;
            return false;
        }
    }
}