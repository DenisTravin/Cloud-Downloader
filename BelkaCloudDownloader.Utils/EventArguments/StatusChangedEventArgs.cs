namespace BelkaCloudDownloader.Utils
{
    using System;

    /// <summary>
    /// Event arguments for StatusChanged signal.
    /// </summary>
    public sealed class StatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="status">Current status of the operation.</param>
        public StatusChangedEventArgs(Status status)
        {
            this.Status = status;
        }

        /// <summary>
        /// Gets current status of the operation.
        /// </summary>
        public Status Status { get; }
    }
}
