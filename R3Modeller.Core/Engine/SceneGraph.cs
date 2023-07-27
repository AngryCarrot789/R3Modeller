using System;
using System.Collections.Generic;
using R3Modeller.Core.Engine.Objs;

namespace R3Modeller.Core.Engine {
/// <summary>
    /// A class which contains information about an R3 project's scene, which is a collection of objects
    /// </summary>
    public class SceneGraph {
        /// <summary>
        /// The scene's root object, which is primarily used to store the hierarchy of objects
        /// </summary>
        public readonly SceneObject Root;

        public readonly Project project;

        public SceneGraph(Project project) {
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.Root = new SceneObject() {DisplayName = "<root>"};
            SceneObject.SetScene(this.Root, this);
        }

        /// <summary>
        /// Called by a <see cref="SceneObject"/>'s add function when a child object is added
        /// </summary>
        /// <param name="obj">The object that was added</param>
        public void OnObjectAdded(SceneObject obj) {

        }

        /// <summary>
        /// Called by a <see cref="SceneObject"/>'s remove function when a child object is removed
        /// </summary>
        /// <param name="obj">The object that was removed, with its parent still connected</param>
        public void OnObjectRemoved(SceneObject obj) {

        }

        /// <summary>
        /// Called by a <see cref="SceneObject"/>'s replace function when <see cref="oldItem"/> is replaced with <see cref="newItem"/>
        /// <para>
        /// The object whose collection was modified is accessible through <see cref="newItem"/>'s
        /// parent object. <see cref="oldItem"/>'s parent will be set to null
        /// </para>
        /// </summary>
        /// <param name="oldItem">Item that was originally stored and is now being removed</param>
        /// <param name="newItem">The item that is being added</param>
        public void OnObjectReplaced(SceneObject oldItem, SceneObject newItem) {

        }

        /// <summary>
        /// Called when the given object's child collection is just about to be cleared, but after all items have had their handler functions called
        /// </summary>
        /// <param name="obj">The object being cleared</param>
        public void OnObjectCleared(SceneObject obj) {

        }
    }
}