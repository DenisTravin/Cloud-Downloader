namespace BelkaCloudDownloader.Utils
{
    using System;

    /// <summary>
    /// Event arguments for EstimationChanged signal. Reports a number of work units to do.
    /// </summary>
    public sealed class EstimationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EstimationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="task">Task for which estimation has changed.</param>
        /// <param name="estimation">Current estimation (measured in abstract work units).</param>
        public EstimationChangedEventArgs(AbstractTask task, int estimation)
        {
            this.Estimation = estimation;
            this.Task = task;
        }

        /// <summary>
        /// Gets current estimation of a task (measured in abstract work units).
        /// </summary>
        public int Estimation { get; }

        /// <summary>
        /// Gets task for which estimation is changed.
        /// </summary>
        public AbstractTask Task { get; }
    }
}
