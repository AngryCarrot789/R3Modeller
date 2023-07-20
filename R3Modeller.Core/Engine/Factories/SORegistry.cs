using System;
using R3Modeller.Core.Engine.Objs;
using R3Modeller.Core.Engine.Objs.ViewModels;

namespace R3Modeller.Core.Engine.Factories {
    public class SORegistry : ModelRegistry<SceneObject, SceneObjectViewModel> {
        public static SORegistry Instance { get; } = new SORegistry();

        private SORegistry() {
            this.Register<SceneObject, SceneObjectViewModel>("so_null");
            this.Register<TriangleObject, TriangleObjectViewModel>("so_tri");
            this.Register<FloorPlaneObject, FloorPlaneObjectViewModel>("so_plane");
            this.Register<WavefrontObject, WavefrontObjectViewModel>("so_wavefront");
        }

        public new void Register<TModel, TViewModel>(string id) where TModel : SceneObject where TViewModel : SceneObjectViewModel {
            base.Register<TModel, TViewModel>(id);
        }

        public SceneObject CreateModel(string id) {
            return (SceneObject) Activator.CreateInstance(base.GetModelType(id));
        }

        public SceneObjectViewModel CreateViewModel(string id) {
            return (SceneObjectViewModel) Activator.CreateInstance(base.GetViewModelType(id));
        }

        public SceneObjectViewModel CreateViewModelFromModel(SceneObject model) {
            return (SceneObjectViewModel) Activator.CreateInstance(base.GetViewModelTypeFromModel(model), model);
        }
    }
}