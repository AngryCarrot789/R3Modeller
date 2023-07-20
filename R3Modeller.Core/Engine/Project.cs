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

        public static Project CreateDefault() {
            Project project = new Project();
            FloorPlaneObject floor = new FloorPlaneObject();
            floor.SetScale(new Vector3(5f));

            project.Scene.rootList.Add(floor);

            TriangleObject tri1 = new TriangleObject();
            tri1.SetTransformation(new Vector3(1f, 0f, 0f), new Vector3(1.5f), Vector3.Zero);

            TriangleObject tri2 = new TriangleObject();
            tri2.SetPosition(new Vector3(4, 2, 4));
            tri1.AddChild(tri2);

            project.Scene.rootList.Add(tri1);

            return project;
        }
    }
}