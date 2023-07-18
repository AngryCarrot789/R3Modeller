namespace R3Modeller.Core.Rendering {
    public interface IRenderTarget {
        /// <summary>
        /// Uses any available view buffers, and sets its state to be in use, meaning the main UI cannot draw using it
        /// </summary>
        /// <param name="buffer">The available buffer</param>
        /// <returns>True if a buffer was available, otherwise false</returns>
        bool Allocate(out IViewBuffer buffer);

        /// <summary>
        /// Call as soon as the buffer usage from <see cref="Allocate"/> is completed, and you want to draw it to the main UI
        /// </summary>
        /// <param name="buffer">The buffer that was freed</param>
        void Free(IViewBuffer buffer);
    }
}