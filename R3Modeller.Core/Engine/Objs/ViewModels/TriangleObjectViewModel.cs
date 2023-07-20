namespace R3Modeller.Core.Engine.Objs.ViewModels {
    public class TriangleObjectViewModel : SceneObjectViewModel {
        public new TriangleObject Model => (TriangleObject) base.Model;

        public TriangleObjectViewModel(TriangleObject model) : base(model) {
        }
    }
}