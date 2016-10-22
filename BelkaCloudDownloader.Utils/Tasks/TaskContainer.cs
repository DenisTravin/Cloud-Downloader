namespace BelkaCloudDownloader.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Represents a set of tasks bundled together and able to report estimation and progress as one task (as sum of
    /// estimations and progresses of underlying tasks). Can not run tasks itself, so tasks shall be registered here
    /// and then run manually (it allows better control over execution flow).
    /// </summary>
    public class TaskContainer : AbstractTask
    {
        /// <summary>
        /// A list of underlying tasks.
        /// </summary>
        private IList<AbstractTask> childTasks = new List<AbstractTask>();

        /// <summary>
        /// Guess estimation of a task (based not on subtasks estimations, but on apriori knowledge).
        /// </summary>
        private int guessEstimation;

        /// <summary>
        /// Flag indicating that all subtasks estimations are actual and we shall not rely on guess estimation anymore.
        /// </summary>
        private bool estimationIsFinal;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskContainer"/> class.
        /// </summary>
        /// <param name="cancellationToken">Token that is used to cancel tasks.</param>
        /// <param name="statusAccumulator">Task status accumulator object to track tasks success/failure status.
        /// </param>
        /// <param name="guessEstimation">Starting apriori estimation. Will be updated automatically if it becomes
        /// clear that there is more work to be done than estimated.</param>
        public TaskContainer(
            CancellationToken cancellationToken,
            TaskStatusAccumulator statusAccumulator,
            int guessEstimation = 0)
        {
            this.CancellationToken = cancellationToken;
            this.StatusAccumulator = statusAccumulator;
            this.guessEstimation = guessEstimation;
            if (guessEstimation != 0)
            {
                this.Estimation = guessEstimation;
            }
        }

        /// <summary>
        /// Registers a task as a child task in this container.
        /// </summary>
        /// <param name="task">A task to register as a subtask.</param>
        public void AddTask(AbstractTask task)
        {
            lock (this)
            {
                this.childTasks.Add(task);
                task.EstimationChanged += this.OnEstimationChanged;
                task.ProgressChanged += this.OnProgressChanged;

                task.CancellationToken = this.CancellationToken;
                task.StatusAccumulator = this.StatusAccumulator;
                task.Log = this.Log;

                this.UpdateEstimation();
            }
        }

        /// <summary>
        /// Recalculates estimation based on current estimations of child tasks. Supposed to be called when all
        /// information about remaining work is known and guess estimation becomes obsolete.
        /// </summary>
        public void Reestimate()
        {
            this.estimationIsFinal = true;
            this.Estimation = this.Estimate();
        }

        /// <summary>
        /// Removes all tasks and resets status.
        /// </summary>
        public virtual void Clear()
        {
            lock (this)
            {
                foreach (var task in this.childTasks)
                {
                    task.EstimationChanged -= this.OnEstimationChanged;
                    task.ProgressChanged -= this.OnProgressChanged;
                }

                this.estimationIsFinal = false;
                this.Progress = 0;
                this.Estimation = this.guessEstimation;
                this.childTasks.Clear();
            }
        }

        /// <summary>
        /// Sets guess estimation for a task container.
        /// </summary>
        /// <param name="estimation">Guess estimation in abstract work units.</param>
        protected void SetGuessEstimation(int estimation)
        {
            this.guessEstimation = estimation;
            if (this.guessEstimation > this.Estimation)
            {
                this.Estimation = this.guessEstimation;
                this.estimationIsFinal = false;
            }
        }

        /// <inheritdoc />
        protected override void UpdateLog(IIoLogger logger)
        {
            lock (this)
            {
                foreach (var task in this.childTasks)
                {
                    task.Log = logger;
                }
            }
        }

        /// <summary>
        /// Handler for EstimationChanged event from one of subtasks.
        /// </summary>
        /// <param name="sender">Task that sent the event.</param>
        /// <param name="args">Event arguments.</param>
        private void OnEstimationChanged(object sender, EstimationChangedEventArgs args) => this.UpdateEstimation();

        /// <summary>
        /// Handler for ProgressChanged event from one of subtasks.
        /// </summary>
        /// <param name="sender">Task that sent the event.</param>
        /// <param name="args">Event arguments.</param>
        private void OnProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            if (args.Info != string.Empty)
            {
                this.Info = args.Info;
            }

            this.Progress = this.GetProgress();
        }

        /// <summary>
        /// Updates current estimation based on estimation of subtasks. If sum of subtasks estimations is less than
        /// guess estimation and Reestimate was not called, guess estimation is used.
        /// </summary>
        private void UpdateEstimation()
        {
            var estimation = this.Estimate();
            if ((estimation > this.guessEstimation) || this.estimationIsFinal)
            {
                this.Estimation = estimation;
            }
        }

        /// <summary>
        /// Returns current sum of estimations of subtasks.
        /// </summary>
        /// <returns>Current sum of estimations of subtasks.</returns>
        private int Estimate() => this.childTasks.Sum(t => t.Estimation);

        /// <summary>
        /// Returns current sum of progresses of subtasks.
        /// </summary>
        /// <returns>Current sum of progresses of subtasks.</returns>
        private int GetProgress() => this.childTasks.Sum(t => t.Progress);
    }
}
