namespace BelkaCloudDownloader.Google.Drive
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security;
    using Utils;
    using File = global::Google.Apis.Drive.v3.Data.File;

    /// <summary>
    /// Static class containing useful file utilities.
    /// </summary>
    internal static class FileDownloadUtils
    {
        /// <summary>
        /// By given file and a dictionary of directories returns file name on a local file system where this file shall be stored
        /// (without extension for Google Docs files and with extension for binary files).
        /// </summary>
        /// <param name="file">A file for which we want full local file name.</param>
        /// <param name="directories">A dictionary that maps file to a list of directories where it can be stored.</param>
        /// <returns>Full file name (with path) of a file in local file system.</returns>
        /// <exception cref="InternalErrorException">Internal error, trying to save a file without directory info.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="System.IO.PathTooLongException">The fully qualified path and file name is 260 or more characters.</exception>
        /// <exception cref="ArgumentException">File path contains one or more of the invalid characters.</exception>
        public static string GetFileName(File file, IDictionary<File, IEnumerable<DirectoryInfo>> directories)
        {
            if (!directories.ContainsKey(file))
            {
                throw new InternalErrorException("Internal error, trying to save a file without directory info.");
            }

            var dir = directories[file].FirstOrDefault();
            if (dir == null)
            {
                throw new InternalErrorException("Internal error, trying to save a file without directory info.");
            }

            var idPart = string.Empty;

            // Check for possible names collision, since Google Drive allows to store files with same names in the same directory.
            if (directories.Any(pair => (pair.Value.FirstOrDefault()?.FullName == dir.FullName)
                && (pair.Key != file)
                && (pair.Key.Name == file.Name)))
            {
                idPart = file.Id;
                idPart = FileUtils.SanitizeFileName(idPart);
                idPart = "." + idPart;
            }

            var fileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}{3}{4}",
                dir.FullName,
                Path.DirectorySeparatorChar,
                Path.GetFileNameWithoutExtension(FileUtils.SanitizeFileName(file.Name)),
                idPart,
                Path.GetExtension(FileUtils.SanitizeFileName(file.Name)));

            return fileName;
        }

        /// <summary>
        /// Correct creation, modification and access times for a file in local file system to match corresponding values from cloud.
        /// </summary>
        /// <param name="file">Meta-information about file from cloud.</param>
        /// <param name="fileName">Local file name.</param>
        /// <returns>File info with correct time settings.</returns>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">Access to <paramref name="fileName" /> is denied. </exception>
        /// <exception cref="System.IO.IOException"> General I/O exception when trying to modify file times.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid; for example, it is on
        ///         an unmapped drive.</exception>
        /// <exception cref="PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The caller attempts to set an invalid creation time.</exception>
        public static FileInfo CorrectFileTimes(File file, string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            fileInfo.CreationTime = file.CreatedTime?.ToLocalTime() ?? fileInfo.CreationTime;
            fileInfo.LastWriteTime = file.ModifiedTime?.ToLocalTime() ?? fileInfo.LastWriteTime;
            fileInfo.LastAccessTime = file.ViewedByMeTime?.ToLocalTime() ?? fileInfo.LastAccessTime;
            return fileInfo;
        }
    }
}
