using R3Modeller.Core.Settings.ViewModels;

namespace R3Modeller.Core {
    public class ApplicationViewModel : BaseViewModel {
        public ApplicationSettings Settings { get; }

        public ApplicationViewModel() {
            this.Settings = new ApplicationSettings();
        }
    }
}