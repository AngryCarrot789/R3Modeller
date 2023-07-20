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
            this.project = project;
            this.Root = new SceneObject() {DisplayName = "<root>"};
        }
    }
}