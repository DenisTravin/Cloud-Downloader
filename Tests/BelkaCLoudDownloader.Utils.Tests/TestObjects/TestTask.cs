namespace BelkaCloudDownloader.Utils.Tests
{
    /// <summary>
    /// Custom descendant of <see cref="AbstractTask"/> used to test its capabilities on estimation and
    /// progress reporting.
    /// </summary>
    internal sealed class TestTask : AbstractTask
    {
        /// <summary>
        /// Simulates estimation change.
        /// </summary>
        public void UpdateEstimation() => this.Estimation = 3;

        /// <summary>
        /// Simulates finishing one unit of work.
        /// </summary>
        public void DoWork() => ++this.Progress;
    }
}
