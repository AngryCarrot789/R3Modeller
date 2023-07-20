namespace R3Modeller.Core.Engine.Objs.ViewModels {
    public class WavefrontObjectViewModel : SceneObjectViewModel {
        public new WavefrontObject Model => (WavefrontObject) base.Model;

        public WavefrontObjectViewModel(WavefrontObject model) : base(model) {
        }
    }
}