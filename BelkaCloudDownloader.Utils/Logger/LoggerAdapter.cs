namespace BelkaCloudDownloader.Utils
{
    using System;
    using log4net;

    /// <summary>
    /// Implementation of <see cref="IIoLogger"/> interface using log4net.
    /// </summary>
    public sealed class LoggerAdapter : IIoLogger
    {
        /// <summary>
        /// Underlying log4net logger.
        /// </summary>
        private readonly ILog log = LogManager.GetLogger("CloudDownloader");

        /// <summary>
        /// Gets a file where log messages will be written.
        /// </summary>
        public string LogFile => "CloudDownloader.log";

        /// <summary>
        /// Gets a value indicating whether trace messages are written to a log.
        /// </summary>
        public bool IsTraceEnabled => true;

        /// <summary>
        /// Flushes log contents on disk. Does nothing since log4net does that for us.
        /// </summary>
        public void Flush()
        {
        }

        /// <summary>
        /// Adds debug trace to the log.
        /// </summary>
        /// <param name="message">Message of the trace to be added.</param>
        public void Trace(string message)
        {
            this.log.Debug(message);
        }

        /// <summary>
        /// Adds debug trace to the log.
        /// </summary>
        /// <param name="message">Message of the trace to be added.</param>
        /// <param name="arguments">Additional arguments. Ignored in this implementation.</param>
        public void Trace(string message, params object[] arguments)
        {
            this.log.Debug(message);
        }

        /// <summary>
        /// Adds message to the log.
        /// </summary>
        /// <param name="message">Message string to be added.</param>
        public void LogMessage(string message)
        {
            this.log.Info(message);
        }

        /// <summary>
        /// Adds message to the log.
        /// </summary>
        /// <param name="message">Message string to be added.</param>
        /// <param name="arguments">Additional arguments. Ignored in this implementation.</param>
        public void LogMessage(string message, params object[] arguments)
        {
            this.log.Info(message);
        }

        /// <summary>
        /// Adds message to the log when built in debug configuration.
        /// </summary>
        /// <param name="message">Message string to be added.</param>
        /// <param name="arguments">Additional arguments. Ignored in this implementation.</param>
        public void LogDebugMessage(string message, params object[] arguments)
        {
#if DEBUG
            this.LogMessage(message, arguments);
#endif
        }

        /// <summary>
        /// Adds error info to the log.
        /// </summary>
        /// <param name="message">Message describing an error.</param>
        /// <param name="e">Exception that caused an error.</param>
        /// <param name="arguments">Additional arguments. Ignored in this implementation.</param>
        public void LogError(string message, Exception e, params object[] arguments)
        {
            this.log.Error(message, e);
        }

        /// <summary>
        /// Adds error info to the log.
        /// </summary>
        /// <param name="message">Message describing an error.</param>
        /// <param name="arguments">Additional arguments. Ignored in this implementation.</param>
        public void LogError(string message, params object[] arguments)
        {
            this.log.Error(message);
        }

        /// <summary>
        /// Adds error info to the log.
        /// </summary>
        /// <param name="message">Message describing an error.</param>
        /// <param name="e">Exception that caused an error.</param>
        public void LogError(string message, Exception e)
        {
            this.log.Error(message, e);
        }
    }
}
