using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HyperMock;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Test.Storage;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    [TestClass]
    public class ChannelTest
    {
        private Mobile.Channel.Channel _channel;
        private Mock<IIngestion> _mockIngestion;
        private IStorage _mockStorage = new MockStorage();
        private readonly string _channelName = "channelName";
        private readonly int _maxLogsPerBatch = 3;
        private readonly TimeSpan _batchTimeSpan = TimeSpan.FromSeconds(1);
        private readonly int _maxParallelBatches = 3;
        private readonly string _appSecret = Guid.NewGuid().ToString();
        private readonly Guid _installId = Guid.NewGuid();

        public ChannelTest()
        {
            LogSerializer.AddFactory(TestLog.JsonIdentifier, new LogFactory<TestLog>());
        }

        [TestInitialize]
        public void InitializeChannelTest()
        {
            _mockIngestion = Mock.Create<IIngestion>();
            _channel = GetChannelWithTimeSpan(_batchTimeSpan.TotalMilliseconds);
        }
        
        /// <summary>
        /// Verify that channel is enabled by default
        /// </summary>
        [TestMethod]
        public void TestEnabledByDefault()
        {
            Assert.IsTrue(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that disabling channel has the expected effect
        /// </summary>
        [TestMethod]
        public void TestDisable()
        {
            _channel.SetEnabled(false);

            Assert.IsFalse(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that enabling the channel has the expected effect
        /// </summary>
        [TestMethod]
        public void TestEnable()
        {
            _channel.SetEnabled(false);
            _channel.SetEnabled(true);

            Assert.IsTrue(_channel.IsEnabled);
        }
        
        /// <summary>
        /// Verify that enqueuing a log invokes the proper event
        /// </summary>
        [TestMethod]
        public void TestEnqueue()
        {
            var log = new TestLog();
            var enqueued = false;

            _channel.EnqueuingLog += (sender, args) =>
            {
                enqueued = true;
                Assert.AreSame(log, args.Log);
            };

            _channel.Enqueue(log);
            Assert.IsTrue(enqueued);
        }

        /// <summary>
        /// Verify that a log is never sent in *less* time than it is supposed to wait (when the batch is not full)
        /// </summary>
        [TestMethod]
        public void TestWaitTime()
        {
            /* One untested call seems to stabilize times */
            GetWaitTime(1000);
            var expectedWaitTimes = new List<double> {500, 1000, 2000};
            var actualWaitTimes = expectedWaitTimes.Select(t => GetWaitTime(t)).ToList();

            for (var i = 0; i < expectedWaitTimes.Count; ++i)
            {
                Assert.IsTrue(actualWaitTimes[i] >= expectedWaitTimes[i]);
            }
        }

        /// <summary>
        /// This verifies that the times waited are *roughly* what they should be. It is ignored because
        /// it should not cause any automated testing to fail due to various CPU speeds, but is useful
        /// to test locally
        /// </summary>
        [TestMethod, Ignore]
        public void TestWaitTimeWithSoftUpperLimit()
        {
            GetWaitTime(1000);
            var expectedWaitTimes = new List<double> { 500, 3424, 7849 };
            var actualWaitTimes = new List<double>();
            for (var i = 0; i < expectedWaitTimes.Count; ++i)
            {
                actualWaitTimes.Add(GetWaitTime(expectedWaitTimes[i]));
            }
            var errorThresholdMilliseconds = 100;
            var diffs = new List<double>();
            for (var i = 0; i < expectedWaitTimes.Count; ++i)
            {
                var diff = actualWaitTimes[i] - expectedWaitTimes[i];
                diffs.Add(diff);
                Assert.IsTrue(diff >= 0);
            }
            var range = diffs.Max() - diffs.Min();
            Assert.IsTrue(range <= errorThresholdMilliseconds);
        }

        /// <summary>
        /// Verify that when a batch reaches its capacity, it is sent immediately
        /// </summary>
        [TestMethod]
        public void TestMaxLogsPerBatch()
        {
            _channel = GetChannelWithTimeSpan(TimeSpan.FromHours(1).TotalMilliseconds);
            var sem = new SemaphoreSlim(0);
            _channel.SendingLog += (sender, args) => { sem.Release(); };
            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }
            var entered = sem.Wait(5000);
            Assert.IsTrue(entered);
        }
        
        /// <summary>
        /// Verify that when channel is disabled, enqueue event is not triggered
        /// </summary>
        [TestMethod]
        public void TestEnqueueWhileDisabled()
        {
            var log = new TestLog();
            var sem = new SemaphoreSlim(0);
            _channel.EnqueuingLog += (sender, args) =>
            {
                sem.Release();
            };
            _channel.Enqueue(log);
            var entered = sem.Wait(5000);
            Assert.IsFalse(entered);
        }


        private double GetWaitTime(double milliseconds)
        {
            _channel = GetChannelWithTimeSpan(milliseconds);
            var log = new TestLog();

            DateTime startTime;
            var endTime = default(DateTime);
            var sem = new SemaphoreSlim(0, 1);
            _channel.SendingLog += (sender, args) =>
            {
                endTime = DateTime.Now;
                sem.Release();
            };

            startTime = DateTime.Now;
            _channel.Enqueue(log);
            sem.Wait((int)milliseconds*4);
            var ticksDiff = endTime.Ticks - startTime.Ticks;
            return TimeSpan.FromTicks(ticksDiff).TotalMilliseconds; 
        }

        private Mobile.Channel.Channel GetChannelWithTimeSpan(double milliseconds)
        {
            var timeSpan = TimeSpan.FromMilliseconds(milliseconds);
            var mockServiceCall = Mock.Create<IServiceCall>();
            _mockIngestion.Setup(
                ingestion => ingestion.PrepareServiceCall(_appSecret, _installId, Param.IsAny<IList<Log>>()))
                .Returns(mockServiceCall.Object);
            return new Mobile.Channel.Channel(_channelName, _maxLogsPerBatch, timeSpan, _maxParallelBatches,
                _appSecret, _installId, _mockIngestion.Object, _mockStorage);
        }
    }
}

/*
        public void SetEnabled(bool enabled)
        public bool IsEnabled

        #region Events
        public event EnqueuingLogEventHandler EnqueuingLog;
        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;
        #endregion

        public void Enqueue(Log log)




        public void InvalidateDeviceCache()

        public void Clear()
       

        public void Shutdown()
        */