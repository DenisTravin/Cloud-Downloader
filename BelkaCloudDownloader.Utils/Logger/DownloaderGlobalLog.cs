namespace BelkaCloudDownloader.Utils
{
    /// <summary>
    /// Factory for logger objects.
    /// </summary>
    public static class DownloaderGlobalLog
    {
        /// <summary>
        /// Gets or sets global logger. Should be used when no other log is available.
        /// </summary>
        public static IIoLogger Log { get; set; } = new LoggerAdapter();
    }
}
