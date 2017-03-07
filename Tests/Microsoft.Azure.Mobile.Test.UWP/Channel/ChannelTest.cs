using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using HyperMock;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    [TestClass]
    public class ChannelTest
    {
        private Mobile.Channel.Channel _channel;
        private Mock<IIngestion> _mockIngestion;
        private Mock<IStorage> _mockStorage;
        private readonly string _channelName = "channelName";
        private readonly int _maxLogsPerBatch = 3;
        private readonly TimeSpan _batchTimeSpan = TimeSpan.FromSeconds(1);
        private readonly int _maxParallelBatches = 3;
        private readonly string _appSecret = Guid.NewGuid().ToString();
        private readonly Guid _installId = Guid.NewGuid();
        private readonly Storage.Storage _realStorage;

        public ChannelTest()
        {
            LogSerializer.AddFactory(TestLog.JsonIdentifier, new LogFactory<TestLog>());
            _realStorage = new Storage.Storage();
        }

        [TestInitialize]
        public void InitializeChannelTest()
        {
            _realStorage.DeleteLogsAsync(_channelName).Wait();
            _mockIngestion = Mock.Create<IIngestion>();
            _mockStorage = Mock.Create<IStorage>();
            _channel = new Mobile.Channel.Channel(_channelName, _maxLogsPerBatch, _batchTimeSpan, _maxParallelBatches,
                _appSecret, _installId, _mockIngestion.Object, _mockStorage.Object);
          
        }

        [TestMethod]
        public void TestEnabledByDefault()
        {
            Assert.IsTrue(_channel.IsEnabled);
        }

        [TestMethod]
        public void TestDisable()
        {
            _channel.SetEnabled(false);

            Assert.IsFalse(_channel.IsEnabled);
        }

        [TestMethod]
        public void TestEnable()
        {
            _channel.SetEnabled(false);
            _channel.SetEnabled(true);

            Assert.IsTrue(_channel.IsEnabled);
        }

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

        [TestMethod]
        public void TestWaitTime()
        {
            GetWaitTime(1000);
            var expectedWaitTimes = new List<double> {500, 1000, 2000};
            var actualWaitTimes = expectedWaitTimes.Select(t => GetWaitTime(t)).ToList();
            for (var i = 0; i < expectedWaitTimes.Count; ++i)
            {
                Assert.IsTrue(actualWaitTimes[i] >= expectedWaitTimes[i]);
            }
        }

        [TestMethod, Ignore]
        public void TestWaitTimeWithSoftUpperLimit()
        {
            GetWaitTime(1000);
            var expectedWaitTimes = new List<double> { 500, 3424, 7849 };
            var actualWaitTimes = new List<double>();
            var tol = 100;
            var diffs = new List<double>();
            for (var i = 0; i < expectedWaitTimes.Count; ++i)
            {
                actualWaitTimes.Add(GetWaitTime(expectedWaitTimes[i]));
            }
            for (var i = 0; i < expectedWaitTimes.Count; ++i)
            {
                var diff = actualWaitTimes[i] - expectedWaitTimes[i];
                diffs.Add(diff);
                Assert.IsTrue(diff >= 0);
            }
            var range = diffs.Max() - diffs.Min();
            Assert.IsTrue(range <= tol);
        }

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

        [TestMethod]
        public void TestEnqueueWhileDisabled()
        {
            var log = new TestLog();
            var enqueued = false;
            _channel.EnqueuingLog += (sender, args) =>
            {
                enqueued = true;
            };

            _channel.Enqueue(log);
            Assert.IsFalse(enqueued);
        }

        /*
        [TestMethod]
        public void TestDeviceCache()
        {
            _channel = GetChannelWithTimeSpan(1000);
            var log = new TestLog();
     
            var sem = new SemaphoreSlim(0);
            _channel.SentLog += (sender, args) =>
            {
                sem.Release();
            };
            _channel.Enqueue(new TestLog());
            sem.Wait(7000);
            Assert.IsNotNull(log.Device);
        }
        */
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
            _realStorage.DeleteLogsAsync(_channelName).Wait();
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
                _appSecret, _installId, _mockIngestion.Object, _realStorage);
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