namespace BelkaCloudDownloader.Utils
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// General "something went bad" exception, root of a hierarchy of recoverable exceptions.
    /// </summary>
    [Serializable]
    public class CloudDownloaderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudDownloaderException"/> class.
        /// </summary>
        public CloudDownloaderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudDownloaderException"/> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        public CloudDownloaderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudDownloaderException"/> class.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public CloudDownloaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudDownloaderException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        /// <exception cref="SerializationException">The class name is null or
        ///     <see cref="P:System.Exception.HResult" /> is zero (0). </exception>
        public CloudDownloaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
