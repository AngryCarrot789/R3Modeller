using CommunityToolkit.Mvvm.ComponentModel;
using R3Modeller.Core.Notifications;

namespace R3Modeller.Core.Engine.ViewModels {
    public class EditorViewModel : ObservableObject {
        private ProjectViewModel project;

        /// <summary>
        /// The currently opened project. Will never be null
        /// </summary>
        public ProjectViewModel Project {
            get => this.project;
            private set => this.SetProperty(ref this.project, value);
        }

        public NotificationPanelViewModel NotificationPanel { get; }

        /// <summary>
        /// The main window's viewport. This will never be null
        /// </summary>
        public RenderViewportViewModel MainViewport { get; }

        public EditorViewModel(RenderViewport viewport, INotificationHandler handler) {
            this.MainViewport = new RenderViewportViewModel(viewport, this);
            this.NotificationPanel = new NotificationPanelViewModel(handler);
        }

        public void SetProject(ProjectViewModel project) {
            if (this.project != null) {
                // prompt save
            }

            this.Project = project;
        }

        public void InvalidateAllViewports(bool schedule) {
            this.MainViewport.Model.RenderTarget.InvalidateRender(schedule);
        }
    }
}