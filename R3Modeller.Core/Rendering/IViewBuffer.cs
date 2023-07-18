namespace R3Modeller.Core.Rendering {
    /// <summary>
    /// A handle to a UI object which handles the drawing states
    /// </summary>
    public interface IViewBuffer {
        /// <summary>
        /// The time at which this buffer was last used. This is updated after <see cIRenderTarget.UseBuffert.Use"/> is invoked
        /// </summary>
        long LastUsage { get; }
    }
}