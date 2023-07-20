namespace R3Modeller.Core.Rendering {
    /// <summary>
    /// An interface for a view port that can be rendered into
    /// </summary>
    public interface IRenderTarget {
        /// <summary>
        /// Notified the render target that its render is invalid, and it should re-draw the scene
        /// </summary>
        /// <param name="schedule">
        /// True to asynchronously dispatch the invalidation (render will happen at some point in the future),
        /// or false to instead block until the scene is rendered
        /// </param>
        void InvalidateRender(bool schedule = false);
    }
}