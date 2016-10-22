namespace BelkaCloudDownloader.Utils
{
    using System;

    /// <summary>
    /// Event arguments for PreliminaryResultsReady signal.
    /// </summary>
    public sealed class PreliminaryResultsReadyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreliminaryResultsReadyEventArgs"/> class.
        /// </summary>
        /// <param name="results">Preliminary results from an operation.</param>
        public PreliminaryResultsReadyEventArgs(object results)
        {
            this.Results = results;
        }

        /// <summary>
        /// Gets preliminary results from an operation.
        /// </summary>
        public object Results { get; }
    }
}
