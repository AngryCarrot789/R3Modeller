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
        public static readonly DependencyProperty DataSourcesProperty =
            DependencyProperty.Register(
                "DataSources",
                typeof(IEnumerable),
                typeof(PropertyEditor),
                new PropertyMetadata(null, (d, e) => ((PropertyEditor) d).OnDataSourceChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue)));

        public static readonly DependencyProperty ApplicableDataSourcesProperty =
            DependencyProperty.Register(
                "ApplicableDataSources",
                typeof(IEnumerable),
                typeof(PropertyEditor),
                new PropertyMetadata(null));

        // INPUT
        public IEnumerable DataSources {
            get => (IEnumerable) this.GetValue(DataSourcesProperty);
            set => this.SetValue(DataSourcesProperty, value);
        }

        // OUTPUT

        public IEnumerable ApplicableDataSources {
            get => (IEnumerable) this.GetValue(ApplicableDataSourcesProperty);
            set => this.SetValue(ApplicableDataSourcesProperty, value);
        }

        private readonly bool isInDesigner;

        public PropertyEditor() {
            this.isInDesigner = DesignerProperties.GetIsInDesignMode(this);
            this.Loaded += this.OnLoaded;
        }

        private void OnDataSourceChanged(IEnumerable oldItems, IEnumerable newItems) {
            if (oldItems is INotifyCollectionChanged)
                ((INotifyCollectionChanged) oldItems).CollectionChanged -= this.OnDataSourceCollectionChanged;
            if (newItems is INotifyCollectionChanged)
                ((INotifyCollectionChanged) newItems).CollectionChanged += this.OnDataSourceCollectionChanged;
            this.ClearInternal();
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {

        }

        private void OnDataSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.ClearInternal();
        }

        private void ClearInternal() {
            IEnumerable items = this.DataSources;
            List<object> list = items != null ? items.Cast<object>().ToList() : new List<object>();

            // Release bindings from the editor, then setup objects, then re-assign to the original value
            // this.ClearValue(ApplicableDataSourcesProperty);
            R3PropertyEditorRegistry.Instance.SetupObjects(list);
            // this.ApplicableDataSources = new List<object>(R3PropertyEditorRegistry.Instance.Root.Values);
        }
    }
}