namespace BelkaCloudDownloader.Google
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Google;
    using Utils;

    /// <summary>
    /// Utility methods for async requests to Google API.
    /// TODO: It actually shall be thread-safe non-static class that keeps queue of requests for given Google API and can throttle their execution.
    /// </summary>
    public static class GoogleRequestHelper
    {
        /// <summary>
        /// Different Google services have different request quotas, but 210 ms will be good as cooldown even for Google Plus with
        /// 500 requests for 100 s quota.
        /// </summary>
        public const int Cooldown = 210;

        /// <summary>
        /// Executes given asynchronous task and waits some time after so that total amount of time spent will be no less than
        /// specified cooldown. Useful to fulfill server requirements like "no more than one request in a second". Task execution and
        /// waiting are cancellable. Does not provide accurate resolution, up to 25 ms "misses" are possible. Uses exponential backoff
        /// in case of API error.
        /// </summary>
        /// <typeparam name="TOutput">Type of result of a task.</typeparam>
        /// <param name="task">A task to be executed. Must take <see cref="CancellationToken"/> to be able to be cancelled.</param>
        /// <param name="token">Cancellation token allowing to cancel an operation or cooldown.</param>
        /// <param name="cooldown">Cooldown of an operation in milliseconds.</param>
        /// <returns>Output of a task.</returns>
        /// <exception cref="InternalErrorException">Exponential backoff failed to throw exception after last attempt.</exception>
        /// <exception cref="GoogleApiException">There is unrecoverable error during communication with remote server.</exception>
        public static async Task<TOutput> Execute<TOutput>(
            Func<CancellationToken, Task<TOutput>> task,
            CancellationToken token,
            int cooldown = GoogleRequestHelper.Cooldown)
        {
            const int maxAttempts = 6;
            int backoffTime = GoogleRequestHelper.Cooldown;
            int attempts = 0;
            while (attempts < maxAttempts)
            {
                ++attempts;
                try
                {
                    var start = DateTime.Now;
                    var result = await task(token);
                    var end = DateTime.Now;
                    var taskDuration = (int)(end - start).TotalMilliseconds;
                    var remaining = GoogleRequestHelper.Cooldown - taskDuration;
                    if (remaining <= 0)
                    {
                        return result;
                    }

                    await TaskEx.Delay(remaining, token);

                    return result;
                }
                catch (GoogleApiException)
                {
                    if (attempts == maxAttempts)
                    {
                        throw;
                    }

                    await TaskEx.Delay(backoffTime, token);
                    backoffTime *= 2;
                }
            }

            throw new InternalErrorException("Exponential backoff failed to throw exception after last attempt.");
        }
    }
}
