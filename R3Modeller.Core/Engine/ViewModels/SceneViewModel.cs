using System;
using System.Collections.ObjectModel;
using R3Modeller.Core.Engine.Factories;
using R3Modeller.Core.Engine.Objs;
using R3Modeller.Core.Engine.Objs.ViewModels;

namespace R3Modeller.Core.Engine.ViewModels {
    public class SceneViewModel : BaseViewModel {
        private readonly ObservableCollection<SceneObjectViewModel> rootObjects;

        /// <summary>
        /// A collection of root scene objects
        /// </summary>
        public ReadOnlyObservableCollection<SceneObjectViewModel> RootObjects { get; }

        public SceneGraph Model { get; set; }

        public SceneViewModel(SceneGraph model) {
            this.Model = model;
            this.rootObjects = new ObservableCollection<SceneObjectViewModel>();
            this.RootObjects = new ReadOnlyObservableCollection<SceneObjectViewModel>(this.rootObjects);
            foreach (SceneObject root in model.rootList) {
                this.AddInternal(SORegistry.Instance.CreateViewModelFromModel(root));
            }
        }

        private void AddInternal(SceneObjectViewModel obj) {
            if (obj.Parent != null) {
                throw new Exception("Expected object's parent to be null");
            }

            if (obj.Model.Parent != null) {
                throw new Exception("Expected object model's parent to be null");
            }

            this.rootObjects.Add(obj);
        }
    }
}