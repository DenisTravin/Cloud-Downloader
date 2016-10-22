namespace BelkaCloudDownloader.Utils
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface that shall be implemented by a downloader task to actually download something from a cloud service.
    /// </summary>
    /// <typeparam name="TInput">Type of input data for downloading process, like user credentials
    ///     or drive service.</typeparam>
    /// <typeparam name="TOutput">Type of output data of a task, usually it is downloaded data.</typeparam>
    public interface ITask<in TInput, TOutput>
    {
        /// <summary>
        /// Run a task. It shall initiate downloading process and return awaitable <see cref="Task"/> object. Shall be
        /// implemented as cancellable asynchronous operation. Shall use objects from context --- log to a supplied
        /// logger object, respect cancellation token.
        /// </summary>
        /// <param name="data">Input data for downloading process, like user credentials or drive service.</param>
        /// <param name="context">Provides context for task execution, like logger object and cancellation token.
        ///     </param>
        /// <returns>Awaitable <see cref="Task"/> object with downloaded data.</returns>
        Task<TOutput> Run(TInput data, ITaskContext context);
    }
}
