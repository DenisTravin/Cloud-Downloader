namespace BelkaCloudDownloader.Utils
{
    /// <summary>
    /// Enumeration with possible operation states that can be reported by downloader.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// Initial status of a downloader.
        /// </summary>
        Initialized,

        /// <summary>
        /// Authentication with server.
        /// </summary>
        Authenticating,

        /// <summary>
        /// Downloading meta-information about user account and files.
        /// </summary>
        DownloadingMetaInformation,

        /// <summary>
        /// Listing files, receiving meta-information about each one without actually downloading it.
        /// </summary>
        ListingFiles,

        /// <summary>
        /// Downloading files.
        /// </summary>
        DownloadingFiles,

        /// <summary>
        /// Downloading general account information.
        /// </summary>
        DownloadingAccountInfo,

        /// <summary>
        /// Downloading available geo-location data.
        /// </summary>
        DownloadingGeolocationData,

        /// <summary>
        /// Downloading calendar events.
        /// </summary>
        DownloadingCalendar,

        /// <summary>
        /// Successfully finished.
        /// </summary>
        Done,

        /// <summary>
        /// Finished with error.
        /// </summary>
        Error
    }
}
