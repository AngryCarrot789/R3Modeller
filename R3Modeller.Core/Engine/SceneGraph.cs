using System;
using System.Collections.Generic;
using R3Modeller.Core.Engine.Objs;

namespace R3Modeller.Core.Engine {
    /// <summary>
    /// A class which contains information about an R3 project's scene, which is a collection of
    /// </summary>
    public class SceneGraph {
        public readonly List<SceneObject> rootList;

        public SceneGraph(Project project) {
            this.rootList = new List<SceneObject>();
        }

        /// <summary>
        /// Adds an item to the internal root list. This function just does a few precondition checks
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="Exception"></exception>
        public void AddItem(SceneObject obj) {
            if (this.rootList.Contains(obj))
                throw new Exception("Item already added");
            SceneObject.ValidateHasNoParent(obj);
            this.rootList.Add(obj);
        }
    }
}