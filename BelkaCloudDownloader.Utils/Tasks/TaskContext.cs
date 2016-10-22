namespace BelkaCloudDownloader.Utils
{
    using System;
    using System.Threading;

    internal class TaskContext : ITaskContext
    {
        private readonly TaskStatusAccumulator statusAccumulator;

        public TaskContext(IIoLogger log, CancellationToken cancellationToken, TaskStatusAccumulator statusAccumulator)
        {
            this.Log = log;
            this.CancellationToken = cancellationToken;
            this.statusAccumulator = statusAccumulator;
        }

        /// <inheritdoc/>
        public IIoLogger Log { get; }

        /// <inheritdoc/>
        public CancellationToken CancellationToken { get; }

        /// <inheritdoc/>
        public void Fail(string message, Exception cause = null)
        {
            if (cause != null)
            {
                this.Log.LogError(message, cause);
            }
            else
            {
                this.Log.LogError(message);
            }

            this.statusAccumulator.FailureOccurred();
        }
    }
}
