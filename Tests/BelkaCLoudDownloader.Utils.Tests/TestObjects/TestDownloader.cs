namespace BelkaCloudDownloader.Utils.Tests
{
    using System.Threading;

    /// <summary>
    /// Downloader used for testing of AbstractDownloader capabilities in progress and estimation reporting.
    /// </summary>
    internal sealed class TestDownloader : AbstractDownloader
    {
        public TestDownloader()
            : base(CancellationToken.None, 10)
        {
        }

        public void AddTestSubdownloaderAsCurrentOperation(TestDownloader internalDownloader)
            => this.AddAsCurrentOperation(internalDownloader);

        public void DoWork() => ++this.Progress;

        public void ChangeEstimation(int estimation) => this.Estimation = estimation;
    }
}
