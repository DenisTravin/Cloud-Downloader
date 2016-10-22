namespace BelkaCloudDownloader.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Abstract downloader, something that can get some information from a cloud, with logger, possibility to cancel
    /// an operation and able to report progress and current status, possibly into other thread. It is basically a
    /// task container that has a notion of global Status and distinquishes progress and estimation by overall and
    /// current operation.
    /// </summary>
    public abstract class AbstractDownloader : TaskContainer
    {
        /// <summary>
        /// Current global status of a download.
        /// </summary>
        private Status status;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDownloader"/> class.
        /// </summary>
        /// <param name="cancellationToken">Token by which downloading progress can be cancelled.</param>
        /// <param name="guessEstimation">Starting estimation of complexity of an operation (expressed in abstract
        /// work units). Will be automatically corrected when more information about tasks estimation becomes
        /// available.</param>
        protected AbstractDownloader(CancellationToken cancellationToken, int guessEstimation = 0)
            : base(cancellationToken, new TaskStatusAccumulator(), guessEstimation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDownloader"/> class. Supposed to be used as
        /// sub-downloader, because it has no cancellation token and no status accumulator.
        /// </summary>
        protected AbstractDownloader()
            : base(CancellationToken.None, null)
        {
        }

        /// <summary>
        /// Operation status as a string message.
        /// </summary>
        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Progress for current operation (for example, progress of downloading a file or getting meta-information).
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> CurrentOperationProgressChanged;

        /// <summary>
        /// Sent when estimation of current operation is changed.
        /// </summary>
        public event EventHandler<EstimationChangedEventArgs> CurrentOperationEstimationChanged;

        /// <summary>
        /// Sent when something is downloaded and ready to be shown to an user.
        /// </summary>
        public event EventHandler<PreliminaryResultsReadyEventArgs> PreliminaryResultsReady;

        /// <summary>
        /// Gets or sets global status of a downloader.
        /// </summary>
        public Status Status
        {
            get
            {
                return this.status;
            }

            protected set
            {
                if (value == this.status)
                {
                    return;
                }

                this.status = value;
                this.EmitStatusChanged(this.status);
            }
        }

        /// <summary>
        /// Gets a list of sub-downloaders for this downloader.
        /// </summary>
        protected IList<AbstractDownloader> SubDownloaders { get; } = new List<AbstractDownloader>();

        /// <summary>
        /// Registers another downloader as sub-downloader of this downloader, capable of doing part of the job,
        /// reporting progress, status and current operation progress.
        /// </summary>
        /// <param name="subDownloader">Downloader to register as sub-downloader.</param>
        public void AddSubDownloader(AbstractDownloader subDownloader)
        {
            lock (this)
            {
                this.AddTask(subDownloader);
                this.SubDownloaders.Add(subDownloader);
                subDownloader.StatusAccumulator = this.StatusAccumulator;
                subDownloader.StatusChanged += this.OnStatusChanged;
                subDownloader.PreliminaryResultsReady += this.PreliminaryResultsReady;
                subDownloader.CurrentOperationEstimationChanged += this.OnCurrentOperationEstimationChanged;
                subDownloader.CurrentOperationProgressChanged += this.OnCurrentOperationProgressChanged;
            }
        }

        public override void Clear()
        {
            lock (this)
            {
                foreach (var downloader in this.SubDownloaders)
                {
                    downloader.StatusChanged -= this.OnStatusChanged;
                    downloader.PreliminaryResultsReady -= this.PreliminaryResultsReady;
                    downloader.CurrentOperationEstimationChanged -= this.OnCurrentOperationEstimationChanged;
                    downloader.CurrentOperationProgressChanged -= this.OnCurrentOperationProgressChanged;
                }

                this.SubDownloaders.Clear();
            }

            base.Clear();
        }

        /// <summary>
        /// Marks downloading process as "Done", notifying everyone.
        /// </summary>
        protected void Done()
        {
            this.Progress = this.Estimation;
            this.Status = Status.Done;
        }

        /// <summary>
        /// Marks downloading process state as unrecoverable error, notifying everyone.
        /// </summary>
        protected void Error()
        {
            this.Progress = this.Estimation;
            this.Status = Status.Error;
            this.StatusAccumulator.FailureOccurred();
        }

        /// <summary>
        /// Adds sub-downloader as a current suboperation, so its progress will be returned as current operation
        /// progress.
        /// </summary>
        /// <param name="subDownloader">Downloader to register as sub-downloader.</param>
        protected void AddAsCurrentOperation(AbstractDownloader subDownloader)
        {
            this.AddSubDownloader(subDownloader);

            subDownloader.ProgressChanged +=
                (sender, args) => this.SetCurrentOperationProgress(subDownloader, subDownloader.Info, args.Progress);
            subDownloader.EstimationChanged +=
                (sender, args) => this.SetCurrentOperationEstimation(subDownloader, args.Estimation);

            this.SetCurrentOperationEstimation(subDownloader, subDownloader.Estimation);
            this.SetCurrentOperationProgress(subDownloader, subDownloader.Info, subDownloader.Progress);
        }

        /// <summary>
        /// Helper that posts MetaInformationReady event into correct thread.
        /// </summary>
        /// <param name="results">Meta-information that can be displayed in GUI without need to wait when actual files
        ///     are downloaded.</param>
        protected void EmitPreliminaryResultsReady(object results) =>
            this.Invoke(
                () => this.PreliminaryResultsReady?.Invoke(this, new PreliminaryResultsReadyEventArgs(results)));

        /// <summary>
        /// Handler for StatusChanged event from one of subdownloaders.
        /// </summary>
        /// <param name="sender">Downloader that sent the event.</param>
        /// <param name="args">Event arguments.</param>
        private void OnStatusChanged(object sender, StatusChangedEventArgs args)
        {
            if (args.Status != Status.Done)
            {
                this.Status = args.Status;
            }
            else
            {
                lock (this)
                {
                    if (this.SubDownloaders.All(d => d.Status == Status.Done))
                    {
                        this.Status = Status.Done;
                    }
                }
            }
        }

        /// <summary>
        /// Handler for CurrentOperationEstimationChanged event from one of subdownloaders.
        /// </summary>
        /// <param name="sender">Downloader that sent the event.</param>
        /// <param name="args">Event arguments.</param>
        private void OnCurrentOperationEstimationChanged(object sender, EstimationChangedEventArgs args)
            => this.EmitCurrentOperationEstimationChanged(args.Task, args.Estimation);

        /// <summary>
        /// Handler for CurrentOperationProgressChanged event from one of subdownloaders.
        /// </summary>
        /// <param name="sender">Downloader that sent the event.</param>
        /// <param name="args">Event arguments.</param>
        private void OnCurrentOperationProgressChanged(object sender, ProgressChangedEventArgs args)
            => this.EmitCurrentOperationProgressChanged(args.Task, args.Info, args.Progress);

        /// <summary>
        /// Helper that posts CurrentOperationProgressChanged event into correct thread.
        /// </summary>
        /// <param name="task">Operation which progress was changed.</param>
        /// <param name="info">Human-readable information about operation to be displayed as a status in GUI.</param>
        /// <param name="progress">Operation progress value (measured in abstract work units).</param>
        private void EmitCurrentOperationProgressChanged(AbstractTask task, string info, int progress)
            => this.Invoke(() => this.CurrentOperationProgressChanged?.Invoke(
                this, new ProgressChangedEventArgs(task, info, progress)));

        /// <summary>
        /// Helper that posts CurrentOperationEstimationChanged event into correct thread.
        /// </summary>
        /// <param name="task">Operation which estimation was changed.</param>
        /// <param name="estimation">Operation estimation value (measured in abstract work units).</param>
        private void EmitCurrentOperationEstimationChanged(AbstractTask task, int estimation)
            => this.Invoke(() => this.CurrentOperationEstimationChanged?.Invoke(
                this, new EstimationChangedEventArgs(task, estimation)));

        /// <summary>
        /// Sets the estimation of a current sub-task and notifies everyone.
        /// </summary>
        /// <param name="operation">Sub-task for which it is needed to set estimation.</param>
        /// <param name="estimation">Estimation (in abstract work units) of a sub-task.</param>
        private void SetCurrentOperationEstimation(AbstractTask operation, int estimation)
            => this.EmitCurrentOperationEstimationChanged(operation, estimation);

        /// <summary>
        /// Sets the progress of a current sub-task and notifies everyone.
        /// </summary>
        /// <param name="operation">Sub-task for which it is needed to set estimation.</param>
        /// <param name="info">Human-readable information about operation to be displayed as a status in GUI.</param>
        /// <param name="progress">Current progress (in abstract work units) of a sub-task.</param>
        private void SetCurrentOperationProgress(AbstractTask operation, string info, int progress)
            => this.EmitCurrentOperationProgressChanged(operation, info, progress);

        /// <summary>
        /// Helper that posts StatusChanged event into correct thread.
        /// </summary>
        /// <param name="statusCode">TaskStatus code.</param>
        private void EmitStatusChanged(Status statusCode) =>
            this.Invoke(() => this.StatusChanged?.Invoke(this, new StatusChangedEventArgs(statusCode)));
    }
}
