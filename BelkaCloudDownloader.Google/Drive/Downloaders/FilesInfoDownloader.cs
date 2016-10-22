namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Google.Apis.Drive.v3;
    using global::Google.Apis.Drive.v3.Data;
    using Utils;

    /// <summary>
    /// Downloads metainformation about files.
    /// </summary>
    internal sealed class FilesInfoDownloader : AbstractDownloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesInfoDownloader"/> class.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that allows to cancel an operation.</param>
        public FilesInfoDownloader(CancellationToken cancellationToken)
            : base(cancellationToken, 10)
        {
        }

        /// <summary>
        /// Downloads meta-information about files.
        /// </summary>
        /// <param name="service">Google Drive service that is used to download files metainformation.</param>
        /// <returns>A list of meta-information for all files on drive.</returns>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        public async Task<IEnumerable<File>> GetFilesInfo(DriveService service)
        {
            this.Log.LogMessage("Downloading files info...");

            var listRequest = service.Files.List();
            listRequest.PageSize = 1000;
            listRequest.Spaces = "drive";

            var filesList = new List<File>();

            try
            {
                this.Status = Status.ListingFiles;
                var listResult = await GoogleRequestHelper.Execute(listRequest.ExecuteAsync, this.CancellationToken);
                ++this.Progress;

                filesList.AddRange(listResult.Files);

                while (!string.IsNullOrEmpty(listResult.NextPageToken))
                {
                    ++this.Estimation;
                    this.CancellationToken.ThrowIfCancellationRequested();
                    listRequest.PageToken = listResult.NextPageToken;
                    listResult = await GoogleRequestHelper.Execute(listRequest.ExecuteAsync, this.CancellationToken);
                    ++this.Progress;
                    filesList.AddRange(listResult.Files);
                }
            }
            catch (Exception e)
            {
                if (this.CancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                this.Log.LogError("Files listing error.", e);
                this.Error();
                return new List<File>();
            }

            this.Log.LogMessage($"Listed {filesList.Count} files.");

            this.Estimation += filesList.Count;

            var semaphore = new AsyncSemaphore(Utils.Constants.DownloadThreadsCount);

            this.Status = Status.DownloadingMetaInformation;

            var result = await AsyncHelper.ProcessInChunks(
                filesList,
                f => this.DownloadFileMetaInformation(service, f, semaphore),
                Utils.Constants.TaskChunkSize,
                this.CancellationToken);

            this.Done();
            return result;
        }

        /// <summary>
        /// Asynchronously downloads meta-information for one file.
        /// </summary>
        /// <param name="service">Google Drive service that is used to download files metainformation.</param>
        /// <param name="file">File with filled Id field.</param>
        /// <param name="semaphore">Semaphore used to throttle downloading process.</param>
        /// <returns>Asynchronous task that downloads meta-information.</returns>
        private async Task<File> DownloadFileMetaInformation(DriveService service, File file, AsyncSemaphore semaphore)
        {
            var fileInfoRequest = service.Files.Get(file.Id);
            fileInfoRequest.Fields = "*";

            await semaphore.WaitAsync();

            try
            {
                var fileInfo = await GoogleRequestHelper.Execute(
                    fileInfoRequest.ExecuteAsync,
                    this.CancellationToken,
                    GoogleRequestHelper.Cooldown * Utils.Constants.DownloadThreadsCount);

                ++this.Progress;
                this.StatusAccumulator.SuccessItem();
                return fileInfo;
            }
            catch (Exception e)
            {
                if (this.CancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                this.Log.LogError($"Downloading meta-information for file {file.Name} failed.", e);
                this.StatusAccumulator.FailureOccurred();

                throw;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
