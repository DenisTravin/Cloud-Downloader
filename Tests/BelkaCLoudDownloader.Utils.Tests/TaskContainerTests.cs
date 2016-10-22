namespace BelkaCloudDownloader.Utils.Tests
{
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="TaskContainer"/> class.
    /// </summary>
    [TestClass]
    public sealed class TaskContainerTests
    {
        /// <summary>
        /// Tests the ability of task container to report correct estimation and estimation changes.
        /// </summary>
        [TestMethod]
        public void TestEstimation()
        {
            var taskContainer = new TaskContainer(CancellationToken.None, null);
            var task1 = new TestTask();
            task1.UpdateEstimation();
            var task2 = new TestTask();
            taskContainer.AddTask(task1);
            taskContainer.AddTask(task2);
            Assert.AreEqual(4, taskContainer.Estimation);
            task2.UpdateEstimation();
            Assert.AreEqual(6, taskContainer.Estimation);
        }

        /// <summary>
        /// Tests guessing estimation.
        /// </summary>
        [TestMethod]
        public void TestGuessEstimation()
        {
            var taskContainer = new TaskContainer(CancellationToken.None, null, 5);
            Assert.AreEqual(5, taskContainer.Estimation);
            var task1 = new TestTask();
            task1.UpdateEstimation();
            var task2 = new TestTask();
            taskContainer.AddTask(task1);
            Assert.AreEqual(5, taskContainer.Estimation);
            taskContainer.AddTask(task2);
            Assert.AreEqual(5, taskContainer.Estimation);
            task2.UpdateEstimation();
            Assert.AreEqual(6, taskContainer.Estimation);
        }

        /// <summary>
        /// Tests re-estimation.
        /// </summary>
        [TestMethod]
        public void TestReestimate()
        {
            var taskContainer = new TaskContainer(CancellationToken.None, null, 5);
            Assert.AreEqual(5, taskContainer.Estimation);
            var task1 = new TestTask();
            task1.UpdateEstimation();
            var task2 = new TestTask();
            taskContainer.AddTask(task1);
            Assert.AreEqual(5, taskContainer.Estimation);
            taskContainer.AddTask(task2);
            Assert.AreEqual(5, taskContainer.Estimation);
            taskContainer.Reestimate();
            Assert.AreEqual(4, taskContainer.Estimation);
        }

        /// <summary>
        /// Tests estimation change events.
        /// </summary>
        [TestMethod]
        public void TestEstimationEvents()
        {
            var currentEstimation = 0;
            var taskContainer = new TaskContainer(CancellationToken.None, null);
            taskContainer.EstimationChanged += (sender, args) => currentEstimation = args.Estimation;

            var task1 = new TestTask();
            task1.UpdateEstimation();
            var task2 = new TestTask();
            taskContainer.AddTask(task1);
            Assert.AreEqual(3, currentEstimation);
            taskContainer.AddTask(task2);
            Assert.AreEqual(4, currentEstimation);
            task2.UpdateEstimation();
            Assert.AreEqual(6, currentEstimation);
        }

        /// <summary>
        /// Tests the ability of task container to report progress based on a progress of underlying tasks.
        /// </summary>
        [TestMethod]
        public void TestProgress()
        {
            var taskContainer = new TaskContainer(CancellationToken.None, null);
            var task1 = new TestTask();
            var task2 = new TestTask();
            taskContainer.AddTask(task1);
            taskContainer.AddTask(task2);

            Assert.AreEqual(0, taskContainer.Progress);
            task1.DoWork();
            Assert.AreEqual(1, taskContainer.Progress);
            task2.DoWork();
            Assert.AreEqual(2, taskContainer.Progress);
            task1.DoWork();
            Assert.AreEqual(3, taskContainer.Progress);
            task2.DoWork();
            Assert.AreEqual(4, taskContainer.Progress);
        }
    }
}
