using R3Modeller.Core.Notifications;

namespace R3Modeller.Core.Engine.ViewModels {
    public class EditorViewModel : BaseViewModel {
        private ProjectViewModel project;

        /// <summary>
        /// The currently opened project. Will never be null
        /// </summary>
        public ProjectViewModel Project {
            get => this.project;
            private set => this.RaisePropertyChanged(ref this.project, value);
        }

        public NotificationPanelViewModel NotificationPanel { get; }

        public RenderViewportViewModel RenderViewport { get; }

        public EditorViewModel(RenderViewport viewport, INotificationHandler handler) {
            this.RenderViewport = new RenderViewportViewModel(viewport, this);
            this.NotificationPanel = new NotificationPanelViewModel(handler);
        }

        public void SetProject(ProjectViewModel project) {
            if (this.project != null) {
                // prompt save
            }

            this.Project = project;
        }

        public void InvalidateAllViewports(bool schedule) {
            this.RenderViewport.Model.RenderTarget.InvalidateRender(schedule);
        }
    }
}