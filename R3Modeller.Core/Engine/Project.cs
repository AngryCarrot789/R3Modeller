using System.Numerics;
using R3Modeller.Core.Engine.Objs;

namespace R3Modeller.Core.Engine {
    /// <summary>
    /// A class which contains information about an R3 project
    /// </summary>
    public class Project {
        public SceneGraph Scene { get; }

        public Project() {
            this.Scene = new SceneGraph(this);
        }
    }
}