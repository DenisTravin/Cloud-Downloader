namespace BelkaCloudDownloader.Google.Drive
{
    using global::Google.Apis.Drive.v3.Data;

    /// <summary>
    /// Represents a file that was successfully downloaded from Google Drive.
    /// </summary>
    public sealed class DownloadedFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadedFile"/> class.
        /// </summary>
        /// <param name="metaInformation">Metainformation about file as it was returned by Google server.</param>
        /// <param name="path">A path to a downloaded file on a local file system.</param>
        public DownloadedFile(File metaInformation, string path)
        {
            this.MetaInformation = metaInformation;
            this.Path = path;
        }

        /// <summary>
        /// Gets metainformation about file as it was returned by Google server.
        /// </summary>
        public File MetaInformation { get; }

        /// <summary>
        /// Gets a path to a downloaded file on a local file system.
        /// </summary>
        public string Path { get; }
    }
}
