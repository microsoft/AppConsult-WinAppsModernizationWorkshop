using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Waf.MusicManager.Domain
{
    public static class TaskUtility
    {
        public static TResult GetResult<TResult>(this Task<TResult> task)
        {
            // This ensures the same exception behavior as it would be with 'await'. No AggregateException - just the first one that occurred.
            return task.GetAwaiter().GetResult();
        }

        // Similar as Task.WhenAll but the task completes after the first one throws an exception (does not wait for all other tasks to complete).
        public static Task WhenAllFast(IEnumerable<Task> tasks)
        {
            if (tasks == null) { throw new ArgumentNullException(nameof(tasks)); }

            var tasksArray = tasks.ToArray();
            var taskCompletionSource = new TaskCompletionSource<object>();
            int count = tasksArray.Length;

            void ObserveException(Exception ex)
            {
                // Nothing to do.
            }

            foreach (var task in tasksArray)
            {
                task.ContinueWith(t =>
                {
                    if (taskCompletionSource.Task.IsCompleted)
                    {
                        ObserveException(t.Exception);
                        return;
                    }

                    if (t.IsCanceled)
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                    else if (t.IsFaulted)
                    {
                        taskCompletionSource.TrySetException(t.Exception.Flatten().InnerExceptions);
                    }
                    else
                    {
                        // Decrement the count and continue if this was the last task.
                        if (Interlocked.Decrement(ref count) == 0)
                        {
                            taskCompletionSource.SetResult(null);
                        }
                    }

                }, TaskContinuationOptions.ExecuteSynchronously);
            }

            return taskCompletionSource.Task;
        }
    }
}
