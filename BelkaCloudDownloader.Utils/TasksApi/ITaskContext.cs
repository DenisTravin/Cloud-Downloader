namespace BelkaCloudDownloader.Utils
{
    using System;
    using System.Threading;

    /// <summary>
    /// External context for downloader task execution. Provides logger, cancellation token and some utility functions
    /// to a task, hiding underlying complexity of tasks engine.
    /// </summary>
    public interface ITaskContext
    {
        /// <summary>
        /// Gets logger object that shall be used by a task to log debug information.
        /// </summary>
        IIoLogger Log { get; }

        /// <summary>
        /// Gets cancellation token that task shall check to be able to be cancelled.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Marks task status as failed, writing error message to a log.
        /// </summary>
        /// <param name="message">Message with details about error that caused a task to fail.</param>
        /// <param name="cause">Exception (if present) that caused a task to fail.</param>
        void Fail(string message, Exception cause = null);
    }
}
