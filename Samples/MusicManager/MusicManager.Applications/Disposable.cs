using System;
using System.Threading;

namespace Waf.MusicManager.Applications
{
    public abstract class Disposable : IDisposable
    {
        private int isDisposed;

        public void Dispose()
        {
            DisposeCore(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
        }

        private void DisposeCore(bool isDisposing)
        {
            if (Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0)
            {
                Dispose(isDisposing);
            }
        }
    }
}
