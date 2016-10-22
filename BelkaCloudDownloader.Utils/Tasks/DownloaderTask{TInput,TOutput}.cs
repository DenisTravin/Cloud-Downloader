namespace BelkaCloudDownloader.Utils
{
    using System.Threading.Tasks;

    /// <summary>
    /// Abstract task that has template method for task execution. Updates <see cref="TaskStatusAccumulator"/> object
    /// - sets FailureOccurred if task returned null, or SuccessItem if result is not null. Descendants are expected
    /// to return null on non-critical errors or throw exceptions otherwise.
    /// </summary>
    /// <typeparam name="TInput">Input data type of a task.</typeparam>
    /// <typeparam name="TOutput">Output data type of a task.</typeparam>
    public abstract class DownloaderTask<TInput, TOutput> : AbstractTask
        where TOutput : class
    {
        /// <summary>
        /// Executes task asynchronously, reports progress and logs activity.
        /// </summary>
        /// <param name="data">Input data for a task.</param>
        /// <returns>Output data of a task, null if task is failed.</returns>
        public async Task<TOutput> Run(TInput data)
        {
            this.Progress = 0;
            this.Log.LogDebugMessage($"Running '{this.GetType().Name}' task on '{data}'.");
            var result = await this.DoRun(data);
            if (result == null)
            {
                this.StatusAccumulator?.FailureOccurred();
                this.Log.LogDebugMessage($"Task '{this.GetType().Name}' failed.");
            }
            else
            {
                this.StatusAccumulator?.SuccessItem();
                this.Log.LogDebugMessage($"Task '{this.GetType().Name}' finished.");
            }

            this.Progress = this.Estimation;
            return result;
        }

        /// <summary>
        /// Abstract method that shall be overridden in descendants to actually do work.
        /// </summary>
        /// <param name="data">Input data for a task.</param>
        /// <returns>Output data of a task, null if task is failed.</returns>
        protected abstract Task<TOutput> DoRun(TInput data);
    }
}
