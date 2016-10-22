namespace BelkaCloudDownloader.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains utility methods for working with async tasks.
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// Helper method that takes a list of some data and a function that creates async task for every data item,
        /// and processes them in chunks to avoid launching too many tasks at once. Something like asynchronous "map" function.
        /// </summary>
        /// <param name="data">A list of file info structures to process.</param>
        /// <param name="action">A function that returns a task that does something with every file.</param>
        /// <param name="chunkSize">Number of tasks to be launched simultaneously.</param>
        /// <param name="cancellationToken">Cancellation token that can be used to abort tasks.</param>
        /// <typeparam name="TInput">A type of data to process.</typeparam>
        /// <typeparam name="TResult">A type of return value of a task.</typeparam>
        /// <returns>A list of processed data.</returns>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        public static async Task<IList<TResult>> ProcessInChunks<TInput, TResult>(
            IList<TInput> data,
            Func<TInput, Task<TResult>> action,
            int chunkSize,
            CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(cancellationToken);

            var groupedData = new List<List<TInput>>();
            var count = 0;
            while (count < data.Count)
            {
                var chunk = data.Skip(count).Take(chunkSize);
                groupedData.Add(chunk.ToList());
                count += chunkSize;
            }

            var result = new List<TResult>();
            foreach (var chunk in groupedData)
            {
                result.AddRange(
                    await taskFactory.ContinueWhenAll(
                        chunk.Select(action).ToArray(),
                        t => t.Select(task => task.Result)));

                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }

        /// <summary>
        /// Executes given asynchronous task and waits some time after so that total amount of time spent will be no less than
        /// specified cooldown. Useful to fulfill server requirements like "no more than one request in a second". Task execution and
        /// waiting are cancellable. Does not provide accurate resolution, up to 25 ms "misses" are possible.
        /// </summary>
        /// <typeparam name="TOutput">Type of result of a task.</typeparam>
        /// <param name="task">A task to be executed. Must take <see cref="CancellationToken"/> to be able to be cancelled.</param>
        /// <param name="token">Cancellation token allowing to cancel an operation or cooldown.</param>
        /// <param name="cooldown">Required duration of operation, in milliseconds.</param>
        /// <returns>Output of a task.</returns>
        /// <exception cref="Exception">A "task" function throws an exception.</exception>
        public static async Task<TOutput> ExecuteWithCooldown<TOutput>(
            Func<CancellationToken, Task<TOutput>> task,
            CancellationToken token,
            int cooldown)
        {
            var start = DateTime.Now;
            var result = await task(token);
            var end = DateTime.Now;
            var taskDuration = (int)(end - start).TotalMilliseconds;
            var remaining = cooldown - taskDuration;
            if (remaining <= 0)
            {
                return result;
            }

            await TaskEx.Delay(remaining, token);

            return result;
        }
    }
}
