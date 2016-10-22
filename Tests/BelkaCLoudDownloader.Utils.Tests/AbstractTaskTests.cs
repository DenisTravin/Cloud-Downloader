namespace BelkaCloudDownloader.Utils.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="AbstractTask"/>.
    /// </summary>
    [TestClass]
    public sealed class AbstractTaskTests
    {
        /// <summary>
        /// Tests estimation reporting of abstract task by using custom implementation of a task that can change estimation and simulate progress.
        /// </summary>
        [TestMethod]
        public void EstimationTest()
        {
            var reportedEstimation = 0;
            var reportedProgress = 0;

            var task = new TestTask();
            task.EstimationChanged += (sender, args) => reportedEstimation = args.Estimation;
            task.ProgressChanged += (sender, args) => reportedProgress = args.Progress;

            Assert.AreEqual(1, task.Estimation);

            task.UpdateEstimation();

            Assert.AreEqual(3, task.Estimation);
            Assert.AreEqual(3, reportedEstimation);

            task.DoWork();
            Assert.AreEqual(1, reportedProgress);
            task.DoWork();
            Assert.AreEqual(2, reportedProgress);
            task.DoWork();
            Assert.AreEqual(3, reportedProgress);
        }
    }
}
