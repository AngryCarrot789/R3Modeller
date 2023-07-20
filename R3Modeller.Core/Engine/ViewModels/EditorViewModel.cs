namespace R3Modeller.Core.Engine.ViewModels {
    public class EditorViewModel : BaseViewModel {
        private ProjectViewModel project;

        /// <summary>
        /// The currently opened project. Will never be null
        /// </summary>
        public ProjectViewModel Project {
            get => this.project;
            set => this.RaisePropertyChanged(ref this.project, value);
        }
    }
}