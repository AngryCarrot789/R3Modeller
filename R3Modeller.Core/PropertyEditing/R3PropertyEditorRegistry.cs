using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows.Input;
using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.PropertyEditing.Editors;

namespace R3Modeller.Core.PropertyEditing {
    public class R3PropertyEditorRegistry : PropertyEditorRegistry {
        public static R3PropertyEditorRegistry Instance { get; } = new R3PropertyEditorRegistry();

        private readonly Dictionary<Type, PropertyGroupViewModel> entries;

        public ObservableCollection<PropertyGroupViewModel> ApplicableGroups { get; } = new ObservableCollection<PropertyGroupViewModel>();

        private R3PropertyEditorRegistry() {
            this.entries = new Dictionary<Type, PropertyGroupViewModel>();

            PropertyGroupViewModel typeGroup = this.RegisterType(typeof(SceneObjectViewModel), "Scene Object");
            PropertyGroupViewModel transformation = typeGroup.GetSubGroup("Transformation");
            transformation.AddPropertyEditor("Position", new PositionEditorViewModel());
        }

        private PropertyGroupViewModel RegisterType(Type type, string name) {
            if (!this.entries.TryGetValue(type, out PropertyGroupViewModel group))
                this.entries[type] = @group = new PropertyGroupViewModel(name);
            return @group;
        }

        static Type FindCommonBaseType(IEnumerable<object> objects) {
            Type commonType = null;
            foreach (object obj in objects) {
                Type type = obj.GetType();
                if (commonType == null) {
                    commonType = type;
                }
                else {
                    if (!type.IsAssignableFrom(commonType)) {
                        while (commonType != null && !commonType.IsAssignableFrom(type)) {
                            commonType = commonType.BaseType;
                        }
                    }
                }
            }

            return commonType;
        }

        private static IEnumerable<PropertyGroupViewModel> GetApplicableGroups(IEnumerable<object> objects, Dictionary<Type, PropertyGroupViewModel> typeToGroupEntry) {
            HashSet<PropertyGroupViewModel> applicableGroups = new HashSet<PropertyGroupViewModel>();
            foreach (object obj in objects) {
                // Include the group entry associated with the current type and all its ancestors
                Type type = obj.GetType();
                while (type != null) {
                    if (typeToGroupEntry.TryGetValue(type, out PropertyGroupViewModel groupEntry)) {
                        applicableGroups.Add(groupEntry);
                    }

                    type = type.BaseType;
                }
            }

            return applicableGroups;
        }

        // Function to find all the applicable group entries for a given type
        private List<PropertyGroupViewModel> GetApplicableGroups(Type type) {
            List<PropertyGroupViewModel> applicableGroups = new List<PropertyGroupViewModel>();
            while (type != null) {
                if (this.entries.TryGetValue(type, out PropertyGroupViewModel groupEntry)) {
                    applicableGroups.Add(groupEntry);
                }

                type = type.BaseType;
            }

            return applicableGroups;
        }

        // Function to find common group entries for a collection of objects
        private List<PropertyGroupViewModel> FindCommonGroups(IEnumerable<object> objects) {
            List<PropertyGroupViewModel> commonGroups = null;
            foreach (object obj in objects) {
                Type objectType = obj.GetType();
                List<PropertyGroupViewModel> applicableGroups = this.GetApplicableGroups(objectType);
                if (commonGroups == null) {
                    commonGroups = applicableGroups;
                }
                else {
                    commonGroups = commonGroups.Intersect(applicableGroups).ToList();
                }
            }

            return commonGroups ?? new List<PropertyGroupViewModel>();
        }

        public List<PropertyGroupViewModel> SetupObjects(List<object> dataSources) {
            foreach (PropertyGroupViewModel group in this.ApplicableGroups) {
                group.ClearRecursive();
            }

            this.ApplicableGroups.Clear();

            List<PropertyGroupViewModel> groups = this.FindCommonGroups(dataSources);
            foreach (PropertyGroupViewModel group in groups) {
                group.LoadDataSourcesRecursive(dataSources);
                this.ApplicableGroups.Add(group);
            }

            return groups;
        }
    }
}