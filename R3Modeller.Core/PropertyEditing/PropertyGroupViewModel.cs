using System.Collections.Generic;

namespace R3Modeller.Core.PropertyEditing {
    public class PropertyGroupViewModel {
        public readonly string name;

        private readonly Dictionary<string, PropertyGroupViewModel> groupMap;
        private readonly Dictionary<string, BasePropertyEditorViewModel> valueMap;
        private readonly List<object> valueList;

        public IReadOnlyList<object> Values => this.valueList;

        public string Name => this.name;

        public PropertyGroupViewModel(string name) {
            this.name = name;
            this.valueList = new List<object>();
            this.groupMap = new Dictionary<string, PropertyGroupViewModel>();
            this.valueMap = new Dictionary<string, BasePropertyEditorViewModel>();
        }

        [NotNull]
        public PropertyGroupViewModel GetSubGroup(string groupName, bool create = true) {
            if (!this.groupMap.TryGetValue(groupName, out PropertyGroupViewModel group)) {
                if (!create)
                    return null;
                this.groupMap[groupName] = group = new PropertyGroupViewModel(groupName);
                this.valueList.Add(group);
            }

            return group;
        }

        public BasePropertyEditorViewModel GetEditor(string name) {
            return this.valueMap.TryGetValue(name, out BasePropertyEditorViewModel editor) ? editor : null;
        }

        public void AddPropertyEditor(string name, BasePropertyEditorViewModel editor) {
            this.valueMap[name] = editor;
            this.valueList.Add(editor);
        }

        public void LoadDataSourcesRecursive(IReadOnlyCollection<object> dataSources) {
            foreach (BasePropertyEditorViewModel entry in this.valueMap.Values) {
                entry.LoadHandlers(dataSources);
            }

            foreach (PropertyGroupViewModel entry in this.groupMap.Values) {
                entry.LoadDataSourcesRecursive(dataSources);
            }
        }

        public void ClearRecursive() {
            foreach (BasePropertyEditorViewModel entry in this.valueMap.Values) {
                entry.Clear();
            }

            foreach (PropertyGroupViewModel entry in this.groupMap.Values) {
                entry.ClearRecursive();
            }
        }
    }
}