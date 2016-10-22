namespace BelkaCloudDownloader.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Google.Drive;
    using Utils;

    /// <summary>
    /// Protocol for downloading Google Drive information and documents.
    /// </summary>
    internal sealed class GoogleDriveProtocol : Protocol
    {
        public override IList<string> AuthenticationMethods { get; } = new List<string>
        {
            "Google Drive consent screen"
        };

        public override IList<string> Capabilities { get; } = new List<string>
        {
            "Meta-information about files",
            "Files from Google Drive"
        };

        public override Control AuthenticationControl(string authenticationMethod)
        {
            switch (authenticationMethod)
            {
                case "Google Drive consent screen":
                    return new ConsentScreenControl(
                        token => GoogleDriveDownloader.AuthenticateManually(token, this.Log));
            }

            Debug.Assert(false, "Unknown authentication method passed to protocol");
            return null;
        }

        /// <summary>
        /// Starts downloading process.
        /// </summary>
        /// <param name="targetDirectory">A directory where downloaded artifacts shall be stored.</param>
        /// <param name="authenticationControl">A control with filled authentication information.</param>
        /// <param name="selectedCapabilities">A list of capabilities that shall be downloaded.</param>
        /// <param name="cancellationToken">A token that allows to cancel an operation.</param>
        /// <returns>Downloaded data as protocol-specific object that can be analyzed and shown
        ///         via reflection.</returns>
        /// <exception cref="InternalErrorException">Incorrect authentication control for Google Drive protocol.
        /// </exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        public override async Task<object> Execute(
            string targetDirectory,
            Control authenticationControl,
            IList<string> selectedCapabilities,
            CancellationToken cancellationToken)
        {
            var downloader = new GoogleDriveDownloader(cancellationToken);
            this.AddDownloader(downloader);

            var consentScreenControl = authenticationControl as ConsentScreenControl;
            if (consentScreenControl == null)
            {
                throw new InternalErrorException("Incorrect authentication control for Google Drive protocol.");
            }

            var token = consentScreenControl.RefreshToken;
            var settings = new DriveDownloaderSettings
            {
                RefreshToken = token,
                TargetDirectory = targetDirectory + Path.DirectorySeparatorChar + "GoogleDrive",
                DoDownloadFiles = selectedCapabilities.Contains("Files from Google Drive")
            };

            this.Settings.SetProtocolSettings(this.GetType().FullName, settings);

            return await downloader.Download(settings);
        }
    }
}
