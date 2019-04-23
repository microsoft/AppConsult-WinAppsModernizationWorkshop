using System;
using System.Threading;
using Windows.Foundation;
using Waf.MusicManager.Domain;

namespace Waf.MusicManager.Applications
{
    public static class AsyncOperationExtensions
    {
        public static void Wait<T>(this IAsyncOperation<T> asyncOperation)
        {
            GetResult(asyncOperation, CancellationToken.None);
        }
        
        public static TResult GetResult<TResult>(this IAsyncOperation<TResult> asyncOperation)
        {
            return GetResult(asyncOperation, CancellationToken.None);
        }

        public static TResult GetResult<TResult>(this IAsyncOperation<TResult> asyncOperation, CancellationToken cancellationToken)
        {
            return TaskUtility.GetResult(asyncOperation.AsTask(cancellationToken));
        }
    }
}
