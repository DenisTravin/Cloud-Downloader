namespace BelkaCloudDownloader.Utils
{
    using System;
    using System.Threading;

    /// <summary>
    /// Abstract task that can be performed during downloading. Has estimation (expressed in abstract work units),
    /// can update estimation, report progress, report status, can be cancelled. Can be used in multithreaded
    /// context, but is not thread-safe (no simultaneous calls from multiple threads are allowed).
    /// What it actually does is implemented in descendants.
    /// </summary>
    public abstract class AbstractTask
    {
        /// <summary>
        /// Synchronization context of an object that created this task.
        /// </summary>
        private readonly SynchronizationContext syncContext = SynchronizationContext.Current;

        /// <summary>
        /// Current estimation of a task, in abstract work units.
        /// </summary>
        private int estimation = 1;

        /// <summary>
        /// Current progress of a task, in abstract work units.
        /// </summary>
        private int progress;

        /// <summary>
        /// Logger object used to output debug info.
        /// </summary>
        private IIoLogger log = DownloaderGlobalLog.Log;

        /// <summary>
        /// Reports current progress change for a task.
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Reports changes in an estimation of a task.
        /// </summary>
        public event EventHandler<EstimationChangedEventArgs> EstimationChanged;

        /// <summary>
        /// Gets current status of a task --- success, failure or if there were errors but some information
        /// was downloaded anyway.
        /// </summary>
        public TaskStatus TaskStatus => this.StatusAccumulator.Status;

        /// <summary>
        /// Gets or sets human-readable string about activity this task performs now. Will be sent with the progress
        /// update if set.
        /// </summary>
        public string Info { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets or sets current estimation of a task (measured in abstract work units).
        /// </summary>
        public int Estimation
        {
            get
            {
                return this.estimation;
            }

            protected set
            {
                if (value == this.estimation)
                {
                    return;
                }

                this.estimation = value;
                this.EmitEstimationChanged();
            }
        }

        /// <summary>
        /// Gets or sets current progress of a task (measured in abstract work units).
        /// </summary>
        public int Progress
        {
            get
            {
                return this.progress;
            }

            protected set
            {
                if ((value == this.progress) && (value != 0))
                {
                    return;
                }

                if (value > this.Estimation)
                {
                    this.Estimation = value;
                }

                this.progress = value;
                this.EmitProgressChanged();
            }
        }

        /// <summary>
        /// Gets or sets token that can be used to abort downloading.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets task status accumulator, to report all successfully downloaded pieces of information or
        /// report errors.
        /// </summary>
        public TaskStatusAccumulator StatusAccumulator { get; set; }

        /// <summary>
        /// Gets or sets downloader activity logger.
        /// </summary>
        public IIoLogger Log
        {
            protected get
            {
                return this.log;
            }

            set
            {
                this.log = value;
                this.UpdateLog(value);
            }
        }

        /// <summary>
        /// Invokes given delegate in a thread where downloader was created.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        protected void Invoke(Action action)
        {
            if (this.syncContext != null)
            {
                this.syncContext.Post(s => action(), null);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Helper method that reports failure and logs error with exception.
        /// </summary>
        /// <param name="errorMessage">Message to add to a log.</param>
        /// <param name="exception">Exception to log along the message.</param>
        protected void FailureOccured(string errorMessage, Exception exception = null)
        {
            if (exception != null)
            {
                this.Log.LogError(errorMessage, exception);
            }
            else
            {
                this.Log.LogError(errorMessage);
            }

            this.StatusAccumulator.FailureOccurred();
        }

        /// <summary>
        /// Virtual method that is called when Log property changes and shall be overriden to propagate log changing.
        /// </summary>
        /// <param name="logger">New logger object.</param>
        protected virtual void UpdateLog(IIoLogger logger)
        {
        }

        /// <summary>
        /// Helper that posts ProgressChanged event into correct thread.
        /// </summary>
        private void EmitProgressChanged()
            => this.Invoke(() => this.ProgressChanged?.Invoke(
                this,
                new ProgressChangedEventArgs(this, this.Info, this.progress)));

        /// <summary>
        /// Helper that posts EstimationChanged event into correct thread.
        /// </summary>
        private void EmitEstimationChanged()
            => this.Invoke(() => this.EstimationChanged?.Invoke(
                this,
                new EstimationChangedEventArgs(this, this.estimation)));
    }
}
