namespace BelkaCloudDownloader.Google.Drive
{
    /// <summary>
    /// Google Drive downloader settings.
    /// </summary>
    public sealed class DriveDownloaderSettings
    {
        /// <summary>
        /// Gets or sets valid and non-expired Refresh Token from Google Drive Sync.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets directory where we shall store downloaded files.
        /// </summary>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to download files from cloud or only meta-information.
        /// </summary>
        public bool DoDownloadFiles { get; set; }
    }
}
