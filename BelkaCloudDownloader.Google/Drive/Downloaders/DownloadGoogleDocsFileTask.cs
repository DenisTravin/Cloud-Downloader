namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using global::Google.Apis.Drive.v3;
    using Utils;
    using File = global::Google.Apis.Drive.v3.Data.File;

    /// <summary>
    /// Downloads Google Docs file.
    /// </summary>
    internal sealed class DownloadGoogleDocsFileTask : ITask<File, DownloadedFile>
    {
        /// <summary>
        /// Dictionary with possible export formats for Google Docs files.
        /// </summary>
        private readonly IDictionary<string, IList<string>> exportFormats;

        /// <summary>
        /// Semaphore used to throttle downloading.
        /// </summary>
        private readonly AsyncSemaphore semaphore;

        /// <summary>
        /// Dictionary that contains possible directories to save a file.
        /// </summary>
        private readonly IDictionary<File, IEnumerable<DirectoryInfo>> directories;

        /// <summary>
        /// Google Drive service used to download a file.
        /// </summary>
        private readonly DriveService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadGoogleDocsFileTask"/> class.
        /// </summary>
        /// <param name="exportFormats">A dictionary with possible export formats for Google Docs files, shall be received from Google Drive.</param>
        /// <param name="semaphore">Semaphore used to throttle downloading.</param>
        /// <param name="directories">Dictionary that contains possible directories to save a file.</param>
        /// <param name="service">Google Drive service used to download a file.</param>
        public DownloadGoogleDocsFileTask(
            IDictionary<string, IList<string>> exportFormats,
            AsyncSemaphore semaphore,
            IDictionary<File, IEnumerable<DirectoryInfo>> directories,
            DriveService service)
        {
            this.directories = directories;
            this.exportFormats = exportFormats;
            this.semaphore = semaphore;
            this.service = service;
        }

        /// <summary>
        /// Downloads Google Docs file.
        /// </summary>
        /// <param name="file">Meta-information about file to download.</param>
        /// <param name="context">Provides context for task execution, like logger object and
        ///     cancellation token.</param>
        /// <returns>An information about downloaded file.</returns>
        public async Task<DownloadedFile> Run(File file, ITaskContext context)
        {
            if (!this.exportFormats.ContainsKey(file.MimeType) || (this.exportFormats[file.MimeType].Count == 0))
            {
                context.Log.LogError($"Unknown export format for file MIME type {file.MimeType}.");
                return null;
            }

            // Just take first available export format for now.
            var exportFormat = this.exportFormats[file.MimeType][0];
            var exportRequest = this.service.Files.Export(file.Id, exportFormat);

            await this.semaphore.WaitAsync();

            context.Log.LogDebugMessage(
                $"Starting to download '{file.Name}' Google Docs file, export format is {exportFormat}.");

            try
            {
                var resultStream = await GoogleRequestHelper.Execute(
                    exportRequest.ExecuteAsStreamAsync,
                    context.CancellationToken,
                    GoogleRequestHelper.Cooldown * Utils.Constants.DownloadThreadsCount);

                var mimeParts = exportFormat.Split('/', '+');
                var extension = mimeParts[mimeParts.Length - 1];
                switch (extension)
                {
                    case "vnd.openxmlformats-officedocument.presentationml.presentation":
                        extension = "pptx";
                        break;
                    case "xml":
                        extension = "svg";
                        break;
                }

                var fileName = FileDownloadUtils.GetFileName(file, this.directories) + "." + extension;
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    await resultStream.CopyToAsync(fileStream, 4096, context.CancellationToken);
                }

                var fileInfo = FileDownloadUtils.CorrectFileTimes(file, fileName);
                context.Log.LogDebugMessage(
                    $"File '{file.Name}' downloading finished, saved as '{fileInfo.FullName}'.");
                return new DownloadedFile(file, fileInfo.FullName);
            }
            catch (Exception e)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    throw;
                }

                context.Fail($"File '{file.Name}' downloading failed.", e);

                throw;
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}
