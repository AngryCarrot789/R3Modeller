using System;

namespace R3Modeller.Core.Interactivity {
    [Flags]
    public enum FileDropType {
        None,
        Copy,
        Move,
        All = Copy | Move
    }
}