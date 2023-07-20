namespace R3Modeller.Core.Engine.Objs.ViewModels {
    public class FloorPlaneObjectViewModel : SceneObjectViewModel {
        public new FloorPlaneObject Model => (FloorPlaneObject) base.Model;

        public FloorPlaneObjectViewModel(FloorPlaneObject model) : base(model) {
        }
    }
}