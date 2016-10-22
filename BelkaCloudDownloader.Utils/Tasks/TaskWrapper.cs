namespace BelkaCloudDownloader.Utils
{
    using System.Threading.Tasks;

    public class TaskWrapper<TInput, TOutput> : DownloaderTask<TInput, TOutput>
        where TOutput : class
    {
        private readonly ITask<TInput, TOutput> task;

        public TaskWrapper(ITask<TInput, TOutput> task)
        {
            this.task = task;
        }

        /// <summary>
        /// Executes an underlying task.
        /// </summary>
        /// <param name="data">Input data for a task.</param>
        /// <returns>Output data of a task, null if task is failed.</returns>
        protected override Task<TOutput> DoRun(TInput data)
        {
            var context = new TaskContext(this.Log, this.CancellationToken, this.StatusAccumulator);
            return this.task.Run(data, context);
        }
    }
}
