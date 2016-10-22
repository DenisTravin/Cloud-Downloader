namespace BelkaCloudDownloader.Utils
{
    /// <summary>
    /// Static class with all constants used in BelkaCloudDownloader.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// How many threads shall be used to simultaneously make requests to Google services.
        /// </summary>
        public const int DownloadThreadsCount = 20;

        /// <summary>
        /// How many tasks are allowed to be launched on one batch. This is needed because if several thousand tasks will be started simultaneously,
        /// they will be cancelled forever even if they are waiting on semaphore (every cancelled task throws TaskCancellationException and it kills performance).
        /// </summary>
        public const int TaskChunkSize = 30;

        /// <summary>
        /// Application name, used to identify application for Google APIs.
        /// </summary>
        public const string ApplicationName = "Cloud Downloader";
    }
}
