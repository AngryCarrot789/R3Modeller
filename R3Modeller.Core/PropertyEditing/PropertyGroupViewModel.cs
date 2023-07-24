using System;
using System.Collections.Generic;
using System.Linq;

namespace R3Modeller.Core.PropertyEditing {
    public class PropertyGroupViewModel : BasePropertyObjectViewModel {
        private readonly Dictionary<string, PropertyGroupViewModel> nameToGroupMap;
        private readonly Dictionary<string, BasePropertyEditorViewModel> nameToEditorMap;
        private readonly List<object> valueList;
        private bool isExpanded;

        public IReadOnlyList<object> Values => this.valueList;

        public string Name { get; }

        /// <summary>
        /// Whether or not this group is expanded, showing the child groups and editors
        /// </summary>
        public bool IsExpanded {
            get => this.isExpanded;
            set => this.RaisePropertyChanged(ref this.isExpanded, value);
        }

        public PropertyGroupViewModel(Type applicableType, string name) : base(applicableType) {
            this.Name = name;
            this.valueList = new List<object>();
            this.nameToGroupMap = new Dictionary<string, PropertyGroupViewModel>();
            this.nameToEditorMap = new Dictionary<string, BasePropertyEditorViewModel>();
        }

        public PropertyGroupViewModel CreateSubGroup(Type type, string groupName, bool isExpandedByDefault = true) {
            PropertyGroupViewModel group = new PropertyGroupViewModel(type, groupName) {
                isExpanded = isExpandedByDefault
            };

            this.nameToGroupMap[groupName] = group;
            this.valueList.Add(@group);
            return group;
        }

        public BasePropertyEditorViewModel GetEditorByName(string name) {
            return this.nameToEditorMap.TryGetValue(name, out BasePropertyEditorViewModel editor) ? editor : null;
        }

        public PropertyGroupViewModel GetGroupByName(string name) {
            return this.nameToGroupMap.TryGetValue(name, out PropertyGroupViewModel g) ? g : null;
        }

        public void AddPropertyEditor(string name, BasePropertyEditorViewModel editor) {
            this.nameToEditorMap[name] = editor;
            this.valueList.Add(editor);
        }

        public void ClearHandlersRecursive() {
            foreach (object propertyObject in this.Values) {
                if (propertyObject is BasePropertyEditorViewModel editor) {
                    editor.ClearHandlers();
                }
                else if (propertyObject is PropertyGroupViewModel group) {
                    group.ClearHandlersRecursive();
                }
            }
        }

        // public List<PropertyGroupViewModel> GetApplicableGroups(Type type) {
        //     List<PropertyGroupViewModel> applicableGroups = new List<PropertyGroupViewModel>();
        //     while (type != null) {
        //         if (this.entries.TryGetValue(type, out PropertyGroupViewModel groupEntry)) {
        //             applicableGroups.Add(groupEntry);
        //         }
        //         type = type.BaseType;
        //     }
        //     return applicableGroups;
        // }
        //
        // public List<PropertyGroupViewModel> FindCommonGroups(IEnumerable<object> objects) {
        //     List<PropertyGroupViewModel> commonGroups = null;
        //     foreach (object obj in objects) {
        //         Type objectType = obj.GetType();
        //         List<PropertyGroupViewModel> applicableGroups = this.GetApplicableGroups(objectType);
        //         if (commonGroups == null) {
        //             commonGroups = applicableGroups;
        //         }
        //         else {
        //             commonGroups = commonGroups.Intersect(applicableGroups).ToList();
        //         }
        //     }
        //     return commonGroups ?? new List<PropertyGroupViewModel>();
        // }

        public void SetupHierarchyState(List<object> input) {
            foreach (object propertyObject in this.Values) {
                if (propertyObject is PropertyGroupViewModel group) {
                    group.IsCurrentlyApplicable = input.Any(x => group.ApplicableType.IsInstanceOfType(x));
                    if (group.IsCurrentlyApplicable) {
                        group.SetupHierarchyState(input);
                    }
                }
                else if (propertyObject is BasePropertyEditorViewModel editor) {
                    // TODO: maybe only load handlers for applicable objects, and ignore the other ones?
                    editor.IsCurrentlyApplicable = input.All(x => editor.ApplicableType.IsInstanceOfType(x));
                    if (editor.IsCurrentlyApplicable) {
                        editor.LoadHandlers(input);
                    }
                }
            }
        }
    }
}