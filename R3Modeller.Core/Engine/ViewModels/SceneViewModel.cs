using R3Modeller.Core.Engine.Objs.ViewModels;

namespace R3Modeller.Core.Engine.ViewModels {
    public class SceneViewModel : BaseViewModel {
        /// <summary>
        /// A collection of root scene objects
        /// </summary>
        public SceneObjectViewModel RootObject { get; }

        public SceneGraph Model { get; set; }

        public SceneViewModel(SceneGraph model) {
            this.Model = model;
            this.RootObject = new SceneObjectViewModel(model.Root);
        }
    }
}