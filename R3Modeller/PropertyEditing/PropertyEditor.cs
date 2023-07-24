using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using R3Modeller.Core.PropertyEditing;

namespace R3Modeller.PropertyEditing {
    public class PropertyEditor : Control {
        public static readonly DependencyProperty EditorRegistryProperty =
            DependencyProperty.Register(
                "EditorRegistry",
                typeof(PropertyEditorRegistry),
                typeof(PropertyEditor),
                new PropertyMetadata(null, (d, e) => {
                    if (e.NewValue is PropertyEditorRegistry editor) {
                        ((PropertyEditor) d).ApplicableItems = editor.Root.Values;
                    }
                }));

        public static readonly DependencyProperty InputItemsProperty =
            DependencyProperty.Register(
                "InputItems",
                typeof(IEnumerable),
                typeof(PropertyEditor),
                new PropertyMetadata(null, (d, e) => ((PropertyEditor) d).OnDataSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue)));

        public static readonly DependencyProperty ApplicableItemsProperty =
            DependencyProperty.Register(
                "ApplicableItems",
                typeof(IEnumerable),
                typeof(PropertyEditor),
                new PropertyMetadata(null));

        public PropertyEditorRegistry EditorRegistry {
            get => (PropertyEditorRegistry) this.GetValue(EditorRegistryProperty);
            set => this.SetValue(EditorRegistryProperty, value);
        }

        // INPUT
        public IEnumerable InputItems {
            get => (IEnumerable) this.GetValue(InputItemsProperty);
            set => this.SetValue(InputItemsProperty, value);
        }

        // OUTPUT

        public IEnumerable ApplicableItems {
            get => (IEnumerable) this.GetValue(ApplicableItemsProperty);
            set => this.SetValue(ApplicableItemsProperty, value);
        }

        private readonly bool isInDesigner;

        public PropertyEditor() {
            this.isInDesigner = DesignerProperties.GetIsInDesignMode(this);
        }

        private void OnDataSourceChanged(IEnumerable oldItems, IEnumerable newItems) {
            if (oldItems is INotifyCollectionChanged)
                ((INotifyCollectionChanged) oldItems).CollectionChanged -= this.OnDataSourceCollectionChanged;
            if (newItems is INotifyCollectionChanged)
                ((INotifyCollectionChanged) newItems).CollectionChanged += this.OnDataSourceCollectionChanged;
            this.SetupObjects();
        }

        private void OnDataSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.SetupObjects();
        }

        private void SetupObjects() {
            if (this.EditorRegistry is PropertyEditorRegistry registry) {
                IEnumerable items = this.InputItems;
                List<object> list = items != null ? items.Cast<object>().ToList() : new List<object>();
                registry.SetupObjects(list);
            }
        }
    }
}