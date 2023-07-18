using System;
using System.Windows;
using System.Windows.Controls;

namespace R3Modeller.Utils {
    public class ResourceDictionaryDataTemplateSelector : DataTemplateSelector {
        public ResourceDictionary ResourceDictionary { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            if (item != null && this.ResourceDictionary != null && this.GetDataTemplate(item.GetType(), out DataTemplate template)) {
                return template;
            }

            return base.SelectTemplate(item, container);
        }

        private bool GetDataTemplate(Type type, out DataTemplate template) {
            for (Type t = type; t != null; t = t.BaseType) {
                DataTemplateKey key = new DataTemplateKey(t);
                if (this.ResourceDictionary[key] is DataTemplate dt) {
                    template = dt;
                    return true;
                }
            }

            template = null;
            return false;
        }
    }
}