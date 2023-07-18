using System;

namespace R3Modeller.Core.Utils {
    public interface IRealDisposable : IDisposable {
        bool IsDisposed { get; }

        void Dispose(bool isDisposing);
    }
}