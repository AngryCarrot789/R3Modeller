using R3Modeller.Core.Engine.Objs.ViewModels;
using R3Modeller.Core.Utils;
using System.Collections.ObjectModel;

namespace R3Modeller.Core.Engine.ViewModels {
public class SceneViewModel : BaseViewModel {
        /// <summary>
        /// A collection of root scene objects
        /// </summary>
        public SceneObjectViewModel RootObject { get; }

        public SceneGraph Model { get; set; }

        public ProjectViewModel Project { get; }

        public ObservableCollection<SceneObjectViewModel> SelectedItems { get; } = new ObservableCollection<SceneObjectViewModel>();

        public SceneViewModel(SceneGraph model, ProjectViewModel project) {
            this.Model = model;
            this.Project = project;
            this.RootObject = new SceneObjectViewModel(model.Root);
            this.RootObject.SetScene(this);
        }

        /// <summary>
        /// Called by a <see cref="SceneObjectViewModel"/>'s add function when a child object is added
        /// </summary>
        /// <param name="obj">The object that was added</param>
        public void OnObjectAdded(SceneObjectViewModel obj) {

        }

        /// <summary>
        /// Called by a <see cref="SceneObjectViewModel"/>'s remove function when a child object is removed
        /// </summary>
        /// <param name="obj">The object that was removed, with its parent still connected</param>
        public void OnObjectRemoved(SceneObjectViewModel obj) {

        }

        /// <summary>
        /// Called by a <see cref="SceneObjectViewModel"/>'s replace function when <see cref="oldItem"/> is replaced with <see cref="newItem"/>
        /// <para>
        /// The object whose collection was modified is accessible through <see cref="newItem"/>'s
        /// parent object. <see cref="oldItem"/>'s parent will be set to null
        /// </para>
        /// </summary>
        /// <param name="oldItem">Item that was originally stored and is now being removed</param>
        /// <param name="newItem">The item that is being added</param>
        public void OnObjectReplaced(SceneObjectViewModel oldItem, SceneObjectViewModel newItem) {

        }

        /// <summary>
        /// Called when the given object's child collection is just about to be cleared, but after all items have had their handler functions called
        /// </summary>
        /// <param name="obj">The object being cleared</param>
        public void OnObjectCleared(SceneObjectViewModel obj) {

        }

        public void OnRenderInvalidated() => this.Project.OnRenderInvalidated();
    }
}