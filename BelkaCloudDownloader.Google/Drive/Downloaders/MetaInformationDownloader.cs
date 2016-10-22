namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Google.Apis.Drive.v3;
    using global::Google.Apis.Drive.v3.Data;
    using Utils;

    /// <summary>
    /// Downloads meta-information about Google Docs files.
    /// </summary>
    internal sealed class MetaInformationDownloader : AbstractDownloader
    {
        /// <summary>
        /// Google Drive service that is used to download files or meta-information.
        /// </summary>
        private readonly DriveService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaInformationDownloader"/> class.
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to abort downloading.</param>
        /// <param name="service">Google Drive service that is used to download files or meta-information.</param>
        public MetaInformationDownloader(CancellationToken cancellationToken, DriveService service)
            : base(cancellationToken)
        {
            this.service = service;
        }

        /// <summary>
        /// Downloads meta-information about user account and files from Google Drive.
        /// </summary>
        /// <returns>Meta-information container.</returns>
        public async Task<MetaInformation> DownloadMetaInformation()
        {
            try
            {
                var filesInfoDownloader = new FilesInfoDownloader(this.CancellationToken);
                this.AddAsCurrentOperation(filesInfoDownloader);

                var getAboutTask = new TaskWrapper<DriveService, About>(new GetAboutTask());
                this.AddTask(getAboutTask);

                var files = (await filesInfoDownloader.GetFilesInfo(this.service)).ToList();

                var metaInformation = new MetaInformation(
                    await getAboutTask.Run(this.service),
                    files);

                this.Done();

                return metaInformation;
            }
            catch (Exception e)
            {
                if (this.CancellationToken.IsCancellationRequested)
                {
                    this.Log.LogMessage("Operation was cancelled.");
                    throw;
                }

                this.Log.LogError("Exception when downloading metainformation", e);
                return null;
            }
        }
    }
}
