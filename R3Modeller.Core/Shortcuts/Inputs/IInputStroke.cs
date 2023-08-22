using System;

namespace R3Modeller.Core.Shortcuts.Inputs {
    /// <summary>
    /// An interface defining behaviour for input strokes
    /// </summary>
    public interface IInputStroke : IEquatable<IInputStroke> {
        /// <summary>
        /// This input stroke is keyboard-based
        /// </summary>
        bool IsKeyboard { get; }

        /// <summary>
        /// This input stroke is mouse-based
        /// </summary>
        bool IsMouse { get; }
    }
}