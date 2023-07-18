using System;

namespace R3Modeller.Core.ThreadSafety {
    [Flags]
    public enum LockType {
        None = 0,
        Automation = 1,
        Property = 2
    }
}