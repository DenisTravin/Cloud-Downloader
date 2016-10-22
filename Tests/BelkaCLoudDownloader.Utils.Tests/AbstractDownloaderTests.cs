namespace BelkaCloudDownloader.Utils.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="AbstractDownloader"/> class.
    /// </summary>
    [TestClass]
    public sealed class AbstractDownloaderTests
    {
        /// <summary>
        /// Tests estimation and progress propagation from subdownloader.
        /// </summary>
        [TestMethod]
        public void SubDownloaderTest()
        {
            var externalDownloader = new TestDownloader();

            var currentOperationEstimation = 0;
            var currentOperationProgress = 0;

            externalDownloader.CurrentOperationEstimationChanged +=
                (sender, args) => currentOperationEstimation = args.Estimation;

            externalDownloader.CurrentOperationProgressChanged +=
                (sender, args) => currentOperationProgress = args.Progress;

            Assert.AreEqual(10, externalDownloader.Estimation);
            Assert.AreEqual(0, externalDownloader.Progress);
            Assert.AreEqual(0, currentOperationEstimation);
            Assert.AreEqual(0, currentOperationProgress);

            var internalDownloader = new TestDownloader();

            Assert.AreEqual(10, internalDownloader.Estimation);
            Assert.AreEqual(0, internalDownloader.Progress);
            Assert.AreEqual(0, currentOperationEstimation);
            Assert.AreEqual(0, currentOperationProgress);

            externalDownloader.AddTestSubdownloaderAsCurrentOperation(internalDownloader);

            Assert.AreEqual(10, externalDownloader.Estimation);
            Assert.AreEqual(0, externalDownloader.Progress);
            Assert.AreEqual(10, currentOperationEstimation);
            Assert.AreEqual(0, currentOperationProgress);

            internalDownloader.DoWork();

            Assert.AreEqual(10, externalDownloader.Estimation);
            Assert.AreEqual(1, externalDownloader.Progress);
            Assert.AreEqual(10, currentOperationEstimation);
            Assert.AreEqual(1, currentOperationProgress);

            externalDownloader.DoWork();

            Assert.AreEqual(10, externalDownloader.Estimation);
            Assert.AreEqual(2, externalDownloader.Progress);
            Assert.AreEqual(10, currentOperationEstimation);
            Assert.AreEqual(1, currentOperationProgress);

            internalDownloader.ChangeEstimation(5);

            Assert.AreEqual(10, externalDownloader.Estimation);
            Assert.AreEqual(2, externalDownloader.Progress);
            Assert.AreEqual(5, currentOperationEstimation);
            Assert.AreEqual(1, currentOperationProgress);
        }

        /// <summary>
        /// Tests estimation and progress propagation when there is two subdownloaders.
        /// </summary>
        [TestMethod]
        public void TwoSubDownloadersTest()
        {
            var externalDownloader = new TestDownloader();

            var currentOperationEstimation = 0;
            var currentOperationProgress = 0;

            externalDownloader.CurrentOperationEstimationChanged += (sender, args) => currentOperationEstimation = args.Estimation;
            externalDownloader.CurrentOperationProgressChanged += (sender, args) => currentOperationProgress = args.Progress;

            var internalDownloader1 = new TestDownloader();
            var internalDownloader2 = new TestDownloader();

            externalDownloader.AddTestSubdownloaderAsCurrentOperation(internalDownloader1);
            externalDownloader.AddTestSubdownloaderAsCurrentOperation(internalDownloader2);

            Assert.AreEqual(20, externalDownloader.Estimation);
            Assert.AreEqual(0, externalDownloader.Progress);
            Assert.AreEqual(10, currentOperationEstimation);
            Assert.AreEqual(0, currentOperationProgress);

            internalDownloader1.DoWork();

            Assert.AreEqual(20, externalDownloader.Estimation);
            Assert.AreEqual(1, externalDownloader.Progress);
            Assert.AreEqual(10, currentOperationEstimation);
            Assert.AreEqual(1, currentOperationProgress);

            internalDownloader1.DoWork();

            Assert.AreEqual(20, externalDownloader.Estimation);
            Assert.AreEqual(2, externalDownloader.Progress);
            Assert.AreEqual(10, currentOperationEstimation);
            Assert.AreEqual(2, currentOperationProgress);

            internalDownloader2.DoWork();

            Assert.AreEqual(20, externalDownloader.Estimation);
            Assert.AreEqual(3, externalDownloader.Progress);
            Assert.AreEqual(10, currentOperationEstimation);
            Assert.AreEqual(1, currentOperationProgress);

            internalDownloader2.ChangeEstimation(5);

            Assert.AreEqual(15, externalDownloader.Estimation);
            Assert.AreEqual(3, externalDownloader.Progress);
            Assert.AreEqual(5, currentOperationEstimation);
            Assert.AreEqual(1, currentOperationProgress);
        }
    }
}
