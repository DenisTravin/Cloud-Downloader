namespace BelkaCloudDownloader.Google.Drive
{
    using System.Collections.Generic;
    using global::Google.Apis.Drive.v3.Data;

    /// <summary>
    /// Container with meta information about Google Drive user account and files. Uses Google Drive API classes as data model.
    /// </summary>
    public sealed class MetaInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaInformation"/> class.
        /// </summary>
        /// <param name="about">Information about user account.</param>
        /// <param name="files">Collection with meta-information about files on Google Drive.</param>
        public MetaInformation(About about, ICollection<File> files)
        {
            this.About = about;
            this.Files = files;
        }

        /// <summary>
        /// Gets information about user account.
        /// </summary>
        public About About { get; }

        /// <summary>
        /// Gets collection of files on Google Drive.
        /// </summary>
        public ICollection<File> Files { get; }

        /// <summary>
        /// Gets or sets collection of files actually downloaded and stored in local file system.
        /// It is initialized after <see cref="MetaInformation"/> object is created, since metainformation
        /// is downloaded first, then files.
        /// </summary>
        public ICollection<DownloadedFile> DownloadedFiles { get; set; }
    }
}
