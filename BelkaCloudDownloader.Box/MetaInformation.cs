namespace BelkaCloudDownloader.BoxDrive
{
    using Box.V2.Models;
    using System.Collections.Generic;

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
        public MetaInformation(List<BoxItem> files)
        {
            this.Files = files;
        }

        /// <summary>
        /// Gets collection of files on Google Drive.
        /// </summary>
        public List<BoxItem> Files { get; }


        /// <summary>
        /// Gets or sets collection of files actually downloaded and stored in local file system.
        /// It is initialized after <see cref="MetaInformation"/> object is created, since metainformation
        /// is downloaded first, then files.
        /// </summary>
        public ICollection<DownloadedFile> DownloadedFiles { get; set; }
    }
}