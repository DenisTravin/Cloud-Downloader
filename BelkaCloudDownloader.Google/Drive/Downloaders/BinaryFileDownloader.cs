namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Google.Apis.Download;
    using global::Google.Apis.Drive.v3;
    using Utils;
    using File = global::Google.Apis.Drive.v3.Data.File;

    /// <summary>
    /// Downloads binary (non-"Google Docs") file.
    /// </summary>
    internal sealed class BinaryFileDownloader : AbstractDownloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileDownloader"/> class.
        /// </summary>
        /// <param name="cancellationToken">Token that can be used to abort downloading.</param>
        public BinaryFileDownloader(CancellationToken cancellationToken)
            : base(cancellationToken)
        {
        }

        /// <summary>
        /// Downloads binary (non-"Google Docs") file.
        /// </summary>
        /// <param name="service">Google Drive service used to download a file.</param>
        /// <param name="file">Meta-information about file to download.</param>
        /// <param name="semaphore">Semaphore used to throttle downloading process.</param>
        /// <param name="unitsOfWork">Abstract units of work assigned to download this file. Used in progress reporting.</param>
        /// <param name="directories">Dictionary that contains possible directories to save a file.</param>
        /// <returns>Information about downloaded file, null if downloading failed.</returns>
        /// <exception cref="InternalErrorException">Something is wrong with downloader code itself.</exception>
        public async Task<DownloadedFile> DownloadBinaryFile(
            DriveService service,
            File file,
            AsyncSemaphore semaphore,
            int unitsOfWork,
            IDictionary<File, IEnumerable<DirectoryInfo>> directories)
        {
            this.Info = file.Name;
            this.Estimation = unitsOfWork;

            if (file.Size.HasValue && (file.Size.Value == 0))
            {
                try
                {
                    var fileName = FileDownloadUtils.GetFileName(file, directories);
                    System.IO.File.Create(fileName);
                    this.Done();
                    return new DownloadedFile(file, fileName);
                }
                catch (Exception e)
                {
                    this.Log.LogError($"Saving zero-length file '{file.Name}' error.", e);
                    this.StatusAccumulator.FailureOccurred();
                    throw;
                }
            }

            var request = service.Files.Get(file.Id);
            request.MediaDownloader.ProgressChanged += downloadProgress =>
            {
                switch (downloadProgress.Status)
                {
                    case DownloadStatus.NotStarted:
                        break;
                    case DownloadStatus.Downloading:
                        this.Progress = (int)(downloadProgress.BytesDownloaded * unitsOfWork / (file.Size ?? 1));
                        break;
                    case DownloadStatus.Completed:
                        this.Progress = this.Estimation;
                        break;
                    case DownloadStatus.Failed:
                        this.Progress = this.Estimation;
                        if (!this.CancellationToken.IsCancellationRequested)
                        {
                            this.Status = Status.Error;
                            this.Log.LogError($"Downloading file '{file.Name}' error.", downloadProgress.Exception);
                            this.StatusAccumulator.FailureOccurred();
                        }

                        break;
                    default:
                        throw new InternalErrorException("DownloadStatus enum contains unknown value.");
                }
            };

            await semaphore.WaitAsync();

            this.Log.LogDebugMessage($"Starting to download '{file.Name}' binary file.");

            try
            {
                var fileName = FileDownloadUtils.GetFileName(file, directories);
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await GoogleRequestHelper.Execute(
                        ct => request.DownloadAsync(fileStream, ct),
                        this.CancellationToken,
                        GoogleRequestHelper.Cooldown * Utils.Constants.DownloadThreadsCount);
                }

                var fileInfo = FileDownloadUtils.CorrectFileTimes(file, fileName);
                this.Log.LogDebugMessage($"File '{file.Name}' downloading finished, saved as {fileInfo.FullName}.");
                this.StatusAccumulator.SuccessItem();
                this.Done();
                return new DownloadedFile(file, fileInfo.FullName);
            }
            catch (Exception e)
            {
                if (this.CancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                this.Log.LogError($"Downloading file '{file.Name}' error.", e);
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
