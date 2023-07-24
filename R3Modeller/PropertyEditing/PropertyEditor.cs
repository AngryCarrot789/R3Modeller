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

        public static readonly DependencyPropertyKey ApplicableGroupsProperty = DependencyProperty.RegisterReadOnly("ApplicableGroups", typeof(IEnumerable<PropertyGroupViewModel>), typeof(PropertyEditor), new PropertyMetadata(default(IEnumerable<PropertyGroupViewModel>)));

        // INPUT
        public IEnumerable DataSources {
            get => (IEnumerable) this.GetValue(DataSourcesProperty);
            set => this.SetValue(DataSourcesProperty, value);
        }

        // OUTPUT
        public IEnumerable<PropertyGroupViewModel> ApplicableGroups {
            get => (IEnumerable<PropertyGroupViewModel>) this.GetValue(ApplicableGroupsProperty.DependencyProperty);
            private set => this.SetValue(ApplicableGroupsProperty, value);
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
            this.ApplicableGroups = R3PropertyEditorRegistry.Instance.SetupObjects(list);
        }
    }
}