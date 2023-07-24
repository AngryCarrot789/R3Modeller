using System;
using System.Collections.Generic;
using System.Linq;

namespace R3Modeller.Core.PropertyEditing {
    /// <summary>
    /// A class which contains a collection of child groups and editors
    /// </summary>
    public class PropertyGroupViewModel : BasePropertyObjectViewModel {
        private readonly Dictionary<string, PropertyGroupViewModel> idToGroupMap;
        private readonly Dictionary<string, BasePropertyEditorViewModel> idToEditorMap;
        private readonly List<object> valueList;
        private bool isExpanded;

        public IReadOnlyList<object> Values => this.valueList;

        public string Id { get; }

        /// <summary>
        /// Whether or not this group is expanded, showing the child groups and editors
        /// </summary>
        public bool IsExpanded {
            get => this.isExpanded;
            set => this.RaisePropertyChanged(ref this.isExpanded, value);
        }

        public PropertyGroupViewModel(Type applicableType, string id) : base(applicableType) {
            this.Id = id;
            this.valueList = new List<object>();
            this.idToGroupMap = new Dictionary<string, PropertyGroupViewModel>();
            this.idToEditorMap = new Dictionary<string, BasePropertyEditorViewModel>();
        }

        /// <summary>
        /// Creates and adds a new child group object to this group
        /// </summary>
        /// <param name="applicableType">The applicable type. Must be assignable to the current group's applicable type</param>
        /// <param name="id"></param>
        /// <param name="isExpandedByDefault"></param>
        /// <returns></returns>
        public PropertyGroupViewModel CreateSubGroup(Type applicableType, string id, bool isExpandedByDefault = true) {
            //                                  i think this is the right way around...
            if (this.ApplicableType != null && !applicableType.IsAssignableFrom(this.ApplicableType)) {
                throw new Exception($"The target type is not assignable to the current applicable type: {applicableType} # {this.ApplicableType}");
            }

            if (this.idToGroupMap.ContainsKey(id))
                throw new Exception($"Group already exists with the ID: {id}");

            PropertyGroupViewModel group = new PropertyGroupViewModel(applicableType, id) {
                isExpanded = isExpandedByDefault
            };

            this.idToGroupMap[id] = group;
            this.valueList.Add(@group);
            return group;
        }

        public BasePropertyEditorViewModel GetEditorByName(string name) {
            return this.idToEditorMap.TryGetValue(name, out BasePropertyEditorViewModel editor) ? editor : null;
        }

        public PropertyGroupViewModel GetGroupByName(string name) {
            return this.idToGroupMap.TryGetValue(name, out PropertyGroupViewModel g) ? g : null;
        }

        public void AddPropertyEditor(string id, BasePropertyEditorViewModel editor) {
            if (this.idToEditorMap.ContainsKey(id))
                throw new Exception($"Editor already exists with the name: {id}");

            this.idToEditorMap[id] = editor;
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

        public void SetupHierarchyState(List<object> input) {
            // TODO: maybe calculate every possible type from the given input (scanning each object's hierarchy
            // and adding each type to a HashSet), and then using that to check for applicability.
            // It would probably be slower for single selections, which is most likely what will be used...
            // but the performance difference for multi select would make it worth it tbh

            foreach (object propertyObject in this.Values) {
                if (propertyObject is PropertyGroupViewModel group) {
                    group.IsCurrentlyApplicable = input.Any(x => group.IsApplicable(x));
                    if (group.IsCurrentlyApplicable) {
                        group.SetupHierarchyState(input);
                    }
                }
                else if (propertyObject is BasePropertyEditorViewModel editor) {
                    // TODO: maybe only load handlers for applicable objects, and ignore the other ones?
                    editor.IsCurrentlyApplicable = input.All(x => editor.IsApplicable(x));
                    if (editor.IsCurrentlyApplicable) {
                        editor.LoadHandlers(input);
                    }
                }
            }
        }
    }
}