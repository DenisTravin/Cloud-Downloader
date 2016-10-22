namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Google.Apis.Drive.v3;

    using Utils;

    using File = global::Google.Apis.Drive.v3.Data.File;

    /// <summary>
    /// Downloads binary and Google Docs files from Google Drive and stores them into target directory.
    /// </summary>
    internal sealed class FilesDownloader : AbstractDownloader
    {
        /// <summary>
        /// Google Drive service that is used to download files or meta-information.
        /// </summary>
        private readonly DriveService service;

        /// <summary>
        /// Directory where we shall store downloaded files.
        /// </summary>
        private readonly string targetDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesDownloader"/> class.
        /// </summary>
        /// <param name="service">Google Drive service that is used to download files or meta-information.</param>
        /// <param name="targetDir">Directory where we shall store downloaded files.</param>
        /// <param name="cancellationToken">Token that can be used to abort downloading.</param>
        public FilesDownloader(DriveService service, string targetDir, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            this.service = service;
            this.targetDir = targetDir;
        }

        /// <summary>
        /// Downloads files.
        /// </summary>
        /// <param name="files">Meta-information about files to download.</param>
        /// <param name="exportFormats">A dictionary with possible export formats for Google Docs files, shall be
        /// received from Google Drive.</param>
        /// <returns>A list of info objects about downloaded files.</returns>
        public async Task<IList<DownloadedFile>> DownloadFiles(
            IEnumerable<File> files,
            IDictionary<string, IList<string>> exportFormats)
        {
            var filesList = files as IList<File> ?? files.ToList();
            var weights = filesList.ToDictionary(f => f.Id, f => f.Size ?? Constants.GoogleDocsFileSize);

            this.Log.LogMessage($"Files to download: {weights.Count}, overall size: {weights.Values.Sum()} bytes.");

            IDictionary<File, IEnumerable<DirectoryInfo>> directories;
            try
            {
                directories = this.CreateDirectoryStructure(filesList, this.targetDir);
            }
            catch (Exception e)
            {
                this.Log.LogError("Unexpected error while creating directory structure, aborting download.", e);
                this.Error();
                return new List<DownloadedFile>();
            }

            Func<long, int> weight = size => (int)Math.Max(1, size / 1000000);
            this.SetGuessEstimation(weights.Values.Select(weight).Sum());

            var semaphore = new AsyncSemaphore(Utils.Constants.DownloadThreadsCount);

            try
            {
                return await AsyncHelper.ProcessInChunks(
                    filesList,
                    f =>
                    {
                        this.Status = Status.DownloadingFiles;
                        if (exportFormats.ContainsKey(f.MimeType))
                        {
                            var downloadTask = new TaskWrapper<File, DownloadedFile>(new DownloadGoogleDocsFileTask(
                                exportFormats,
                                semaphore,
                                directories,
                                this.service));

                            this.AddTask(downloadTask);
                            return downloadTask.Run(f).ContinueWith(t => t.Result, this.CancellationToken);
                        }

                        if (!string.IsNullOrEmpty(f.WebContentLink))
                        {
                            var binaryFileDownloader = new BinaryFileDownloader(this.CancellationToken);
                            this.AddAsCurrentOperation(binaryFileDownloader);
                            return binaryFileDownloader.DownloadBinaryFile(
                                this.service,
                                f,
                                semaphore,
                                weight(f.Size ?? 0),
                                directories).ContinueWith(t => t.Result, this.CancellationToken);
                        }

                        if (f.MimeType != "application/vnd.google-apps.folder")
                        {
                            this.Log.LogDebugMessage(
                                $"File '{f.Name}' has no web content link, can not be exported"
                                + " and not a folder, skipping.");
                        }

                        return TaskEx.FromResult<DownloadedFile>(null);
                    },
                    Utils.Constants.TaskChunkSize,
                    this.CancellationToken);
            }
            catch (Exception e)
            {
                if (this.CancellationToken.IsCancellationRequested)
                {
                    this.Log.LogMessage("Operation was cancelled.");
                    throw;
                }

                this.Log.LogError("Error while downloading file.", e);
                this.Error();
                return new List<DownloadedFile>();
            }
        }

        /// <summary>
        /// Creates all directories needed to store files.
        /// </summary>
        /// <param name="files">A list with files meta-information.</param>
        /// <param name="targetDirectory">Root directory where to store downloaded files.</param>
        /// <returns>A dictionary that for each file contains a list of directories where it can be stored.</returns>
        /// <exception cref="System.IO.IOException">The directory cannot be created. </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="ArgumentException">File path contains invalid characters such as ", &lt;, &gt;, or |. </exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.
        ///         For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.
        ///         The specified path, file name, or both are too long.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        private IDictionary<File, IEnumerable<DirectoryInfo>> CreateDirectoryStructure(
            IEnumerable<File> files,
            string targetDirectory)
        {
            var filesList = files as IList<File> ?? files.ToList();
            var filesDictionary = filesList.ToDictionary(f => f.Id);
            var result = new Dictionary<File, IEnumerable<DirectoryInfo>>();

            foreach (var file in filesList)
            {
                if ((file.Parents != null) && (file.Parents.Count > 0))
                {
                    var paths = this.GetPaths(file, filesDictionary);
                    var dirInfos =
                            paths.Select(
                                p =>
                                {
                                    try
                                    {
                                        return new FileInfo(targetDirectory + Path.DirectorySeparatorChar + p)
                                                .Directory;
                                    }
                                    catch (PathTooLongException)
                                    {
                                        this.Log.LogDebugMessage($"Path too long for a file '{p}',"
                                                + " saving as-is in root.");

                                        return new FileInfo(targetDirectory + Path.DirectorySeparatorChar).Directory;
                                    }
                                }).ToList();

                    foreach (var dir in dirInfos)
                    {
                        dir.Create();
                    }

                    if (dirInfos.Count == 0)
                    {
                        this.Log.LogDebugMessage($"No directory info for file '{file.Name}', saving as-is in root.");
                        dirInfos.Add(new FileInfo(targetDirectory + Path.DirectorySeparatorChar).Directory);
                    }

                    result.Add(file, dirInfos);
                }
                else
                {
                    result.Add(file, new List<DirectoryInfo> { new DirectoryInfo(targetDirectory) });
                }
            }

            return result;
        }

        /// <summary>
        /// Returns full paths where given file can be stored.
        /// </summary>
        /// <param name="file">A file for which we want to get paths.</param>
        /// <param name="files">A dictionary that maps file id to a file.</param>
        /// <returns>A list of possible paths for a given file.</returns>
        private IEnumerable<string> GetPaths(File file, IDictionary<string, File> files)
        {
            if ((file.Parents == null) || (file.Parents.Count == 0))
            {
                this.Log.LogDebugMessage($"Meta-information does not contain parents for file '{file.Name}',"
                    + " it seems that it belongs to someone other.");
            }
            else
            {
                foreach (var parent in file.Parents)
                {
                    if (files.ContainsKey(parent))
                    {
                        var parentPaths = this.GetPaths(files[parent], files);
                        foreach (var path in parentPaths)
                        {
                            yield return path + Path.DirectorySeparatorChar + FileUtils.SanitizeFileName(file.Name);
                        }
                    }
                    else
                    {
                        yield return FileUtils.SanitizeFileName(file.Name);
                    }
                }
            }
        }
    }
}
