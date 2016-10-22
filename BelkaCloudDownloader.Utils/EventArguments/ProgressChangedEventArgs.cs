namespace BelkaCloudDownloader.Utils
{
    using System;

    /// <summary>
    /// Event arguments for ProgressChanged signal. Reports a number of work units done.
    /// </summary>
    public sealed class ProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="task">Task which progress has changed.</param>
        /// <param name="info">Current operation user-readable description.</param>
        /// <param name="progress">Current operation progress.</param>
        public ProgressChangedEventArgs(AbstractTask task, string info, int progress)
        {
            this.Task = task;
            this.Progress = progress;
            this.Info = info;
        }

        /// <summary>
        /// Gets identifier or information about operation being executed, to allow to report progress of multiple
        /// simultaneous operations, like downloading multiple files in parallel.
        /// </summary>
        public AbstractTask Task { get; }

        /// <summary>
        /// Gets current operation user-readable description.
        /// </summary>
        public string Info { get; }

        /// <summary>
        /// Gets current operation progress.
        /// </summary>
        public int Progress { get; }
    }
}
