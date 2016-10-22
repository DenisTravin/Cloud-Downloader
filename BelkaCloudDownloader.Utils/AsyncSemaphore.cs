namespace BelkaCloudDownloader.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    /// <summary>
    /// Asynchronous semaphore (substitution for SemaphoreSlim from .NET 4.5), based on AsyncSemaphore by Stephen Toub
    /// (http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266983.aspx)
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    public sealed class AsyncSemaphore
    {
        /// <summary>
        /// Task that does nothing, but allows other task to continue.
        /// </summary>
        private static readonly Task Completed = TaskEx.FromResult(true);

        /// <summary>
        /// A queue of tasks that are waiting on semaphore.
        /// </summary>
        private readonly Queue<TaskCompletionSource<bool>> waitingTasks = new Queue<TaskCompletionSource<bool>>();

        /// <summary>
        /// Currently available "slots" within critical section.
        /// </summary>
        private int currentCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSemaphore"/> class.
        /// </summary>
        /// <param name="count">A number of tasks simultaneously allowed into critical section.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        public AsyncSemaphore(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            this.currentCount = count;
        }

        /// <summary>
        /// If awaited, blocks caller if there are too many tasks already in critical section, otherwise
        /// returns immediately.
        /// </summary>
        /// <returns>Task to be awaited.</returns>
        public Task WaitAsync()
        {
            lock (this.waitingTasks)
            {
                if (this.currentCount > 0)
                {
                    --this.currentCount;
                    return AsyncSemaphore.Completed;
                }

                var waiter = new TaskCompletionSource<bool>();
                this.waitingTasks.Enqueue(waiter);
                return waiter.Task;
            }
        }

        /// <summary>
        /// Releases semaphore and allows to continue next task in a queue.
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (this.waitingTasks)
            {
                if (this.waitingTasks.Count > 0)
                {
                    toRelease = this.waitingTasks.Dequeue();
                }
                else
                {
                    ++this.currentCount;
                }
            }

            toRelease?.SetResult(true);
        }
    }
}
