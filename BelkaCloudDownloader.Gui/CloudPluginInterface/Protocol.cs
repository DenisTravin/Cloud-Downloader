namespace BelkaCloudDownloader.Gui
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Utils;

    /// <summary>
    /// Base class for all protocols. Protocol is one method of authentication and pieces
    /// of data that can be downloaded by that method of authentication. For example, Google requires different
    /// permissions for Drive and Plus, so we actually use two sets of credentials for those applications,
    /// so it is two protocols.
    /// </summary>
    internal abstract class Protocol : AbstractDownloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        protected Protocol()
            : base(CancellationToken.None)
        {
        }

        /// <summary>
        /// Gets a list of authentication method names accepted by this protocol. For example, Google Drive
        /// can use Refresh Token or consent screen as authentication methods.
        /// </summary>
        public abstract IList<string> AuthenticationMethods { get; }

        /// <summary>
        /// Gets a list of pieces of cloud data (known here as capabilities) that this protocol can download.
        /// </summary>
        public abstract IList<string> Capabilities { get; }

        /// <summary>
        /// Gets or sets global settings object.
        /// </summary>
        public SettingsWrapper Settings { get; set; }

        /// <summary>
        /// Returns control through which an user shall provide authentication credentials.
        /// </summary>
        /// <param name="authenticationMethod">Name of the selected authentication method.</param>
        /// <returns>Control through which an user shall provide authentication credentials.</returns>
        public abstract Control AuthenticationControl(string authenticationMethod);

        /// <summary>
        /// Starts downloading process.
        /// </summary>
        /// <param name="targetDirectory">A directory where downloaded artifacts shall be stored.</param>
        /// <param name="authenticationControl">A control with filled authentication information.</param>
        /// <param name="selectedCapabilities">A list of capabilities that shall be downloaded.</param>
        /// <param name="cancellationToken">A token that allows to cancel an operation.</param>
        /// <returns>Downloaded data as protocol-specific object that can be analyzed and shown
        /// via reflection.</returns>
        public abstract Task<object> Execute(
            string targetDirectory,
            Control authenticationControl,
            IList<string> selectedCapabilities,
            CancellationToken cancellationToken);

        public override void Clear()
        {
            foreach (var subDownloader in this.SubDownloaders)
            {
                subDownloader.StatusChanged -= this.OnStatusChanged;
            }

            base.Clear();
        }

        /// <summary>
        /// Adds a downloader to do work for this protocol. Clears previously added downloaders.
        /// </summary>
        /// <param name="downloader">Downloader to add.</param>
        protected void AddDownloader(AbstractDownloader downloader)
        {
            this.Clear();
            this.AddSubDownloader(downloader);
            downloader.StatusChanged += this.OnStatusChanged;
        }

        /// <summary>
        /// Handler for StatusChanged event from one of subdownloaders.
        /// </summary>
        /// <param name="sender">Downloader that sent an event.</param>
        /// <param name="args">Event arguments.</param>
        private void OnStatusChanged(object sender, StatusChangedEventArgs args)
        {
            if (args.Status == Status.Done)
            {
                this.Status = Status.Done;
            }
        }
    }
}
