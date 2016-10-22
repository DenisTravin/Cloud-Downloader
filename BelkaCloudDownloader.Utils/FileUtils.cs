namespace BelkaCloudDownloader.Utils
{
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Contains useful methods for working with file system.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// By given file name makes file name correct with respect to underlying local file system.
        /// </summary>
        /// <param name="fileName">File name to normalize.</param>
        /// <returns>Normalized file name.</returns>
        public static string SanitizeFileName(string fileName)
            => Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '_'));

        /// <summary>
        /// By given folder path makes it correct with respect to underlying local file system.
        /// </summary>
        /// <param name="path">Path to normalize.</param>
        /// <returns>Normalized file name.</returns>
        public static string SanitizePath(string path)
            => Path.GetInvalidFileNameChars().Where(c => (c != '\\') && (c != '/')).Aggregate(path, (current, c) => current.Replace(c, '_'));
    }
}
