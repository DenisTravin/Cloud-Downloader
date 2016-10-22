namespace BelkaCloudDownloader.Gui
{
    using System.Threading;
    using Utils;

    /// <summary>
    /// Downloader that is used as a container for selected protocols. Used for correct progress and status reporting.
    /// </summary>
    internal sealed class Downloader : AbstractDownloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Downloader"/> class.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that allows to abort download.</param>
        public Downloader(CancellationToken cancellationToken)
            : base(cancellationToken)
        {
        }
    }
}
