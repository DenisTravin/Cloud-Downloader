namespace BelkaCloudDownloader.Google.Drive
{
    /// <summary>
    /// Static class with all constants used by Google Drive downloader.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Id of our client registered to test account, which login and password is distributed
        /// on a need-to-know basis.
        /// </summary>
        public const string GoogleDriveSyncClientId
            = "624569318111-1m17qahhguethf94gp1c8l463nm3l6t7.apps.googleusercontent.com";

        /// <summary>
        /// Secret of our client.
        /// </summary>
        public const string GoogleDriveSyncClientSecret = "5o0UEVrRoSdQYjftJBkfX91K";

        /// <summary>
        /// We do not know a size for Google Docs file prior to export so we use just random (but naturally looking)
        /// value here. Progress report will be inaccurate for such files.
        /// </summary>
        public const int GoogleDocsFileSize = 50000;
    }
}
