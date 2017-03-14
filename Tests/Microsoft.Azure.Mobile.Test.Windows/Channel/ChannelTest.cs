using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Test.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    [TestClass]
    public class ChannelTest
    {
        private Mobile.Channel.Channel _channel;
        private readonly MockIngestion _mockIngestion = new MockIngestion();
        private readonly IStorage _storage = new Mobile.Storage.Storage();
        private const string ChannelName = "channelName";
        private const int _maxLogsPerBatch = 3;
        private readonly TimeSpan _batchTimeSpan = TimeSpan.FromSeconds(1);
        private const int MaxParallelBatches = 3;
        private readonly string _appSecret = Guid.NewGuid().ToString();
        private const int DefaultWaitTime = 5000;

        /* Event semaphores for invokation verification */
        private const int SendingLogSemaphoreIdx = 0;
        private const int SentLogSemaphoreIdx = 1;
        private const int FailedToSendLogSemaphoreIdx = 2;
        private const int EnqueuingLogSemaphoreIdx = 3;
        private readonly List<SemaphoreSlim> _eventSemaphores = new List<SemaphoreSlim> { new SemaphoreSlim(0), new SemaphoreSlim(0), new SemaphoreSlim(0), new SemaphoreSlim(0) };

        public ChannelTest()
        {
            LogSerializer.AddLogType(TestLog.JsonIdentifier, typeof(TestLog));
        }

        [TestInitialize]
        public void InitializeChannelTest()
        {
            SetChannelWithTimeSpan(_batchTimeSpan);
        }

        /// <summary>
        /// Verify that channel is enabled by default
        /// </summary>
        [TestMethod]
        public void ChannelEnabledByDefault()
        {
            Assert.IsTrue(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that disabling channel has the expected effect
        /// </summary>
        [TestMethod]
        public void DisableChannel()
        {
            _channel.SetEnabled(false);

            Assert.IsFalse(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that enabling the channel has the expected effect
        /// </summary>
        [TestMethod]
        public void EnableChannel()
        {
            _channel.SetEnabled(false);
            _channel.SetEnabled(true);

            Assert.IsTrue(_channel.IsEnabled);
        }
        
        /// <summary>
        /// Verify that enqueuing a log passes the same log reference to enqueue event argument
        /// </summary>
        [TestMethod]
        public void EnqueuedLogsAreSame()
        {
            var log = new TestLog();
            var sem = new SemaphoreSlim(0);
            _channel.EnqueuingLog += (sender, args) =>
            {
                Assert.AreSame(log, args.Log);
                sem.Release();
            };

            _channel.Enqueue(log);
            Assert.IsTrue(sem.Wait(DefaultWaitTime));
        }

        /// <summary>
        /// Verify that a log is never sent in *less* time than it is supposed to wait (when the batch is not full)
        /// </summary>
        /// <remarks>Ignore due to instability</remarks>
        //[TestMethod, Ignore]
        public void WaitTime()
        {
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
        /// <remarks>Ignore due to instability</remarks>
        //[TestMethod, Ignore]
        public void WaitTimeWithSoftUpperLimit()
        {
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
        public void EnqueueMaxLogs()
        {
            SetChannelWithTimeSpan(TimeSpan.FromHours(1));
            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }
            Assert.IsTrue(SendingLogOccurred(1));
        }
        
        /// <summary>
        /// Verify that when channel is disabled, sent event is not triggered
        /// </summary>
        [TestMethod]
        public void EnqueueWhileDisabled()
        {
            _channel.SetEnabled(false);
            var log = new TestLog();
            _channel.Enqueue(log);
            Assert.IsFalse(SentLogOccurred(1));
        }

        [TestMethod]
        public void ChannelInvokesSendingLogEvent()
        {
            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.IsTrue(SendingLogOccurred(_maxLogsPerBatch));
        }

        [TestMethod]
        public void ChannelInvokesSentLogEvent()
        {
            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.IsTrue(SentLogOccurred(_maxLogsPerBatch));
        }

        [TestMethod]
        public void ChannelInvokesFailedToSendLogEvent()
        {
            MakeIngestionCallsFail();

            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.IsTrue(FailedToSendLogOccurred(_maxLogsPerBatch));
        }

        private double GetWaitTime(double milliseconds)
        {
            SetChannelWithTimeSpan(TimeSpan.FromMilliseconds(milliseconds));
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
            sem.Wait();
            var ticksDiff = endTime.Ticks - startTime.Ticks;
            return TimeSpan.FromTicks(ticksDiff).TotalMilliseconds; 
        }

        private void SetChannelWithTimeSpan(TimeSpan timeSpan)
        {
            _storage.DeleteLogsAsync(ChannelName).Wait();
            _channel = new Mobile.Channel.Channel(ChannelName, _maxLogsPerBatch, timeSpan, MaxParallelBatches,
                _appSecret, _mockIngestion, _storage);
            MakeIngestionCallsSucceed();
            SetupEventCallbacks();
        }

        private void MakeIngestionCallsFail()
        {
            _mockIngestion.CallShouldSucceed = false;
        }

        private void MakeIngestionCallsSucceed()
        {
            _mockIngestion.CallShouldSucceed = true;
        }

        private void SetupEventCallbacks()
        {
            foreach (var sem in _eventSemaphores)
            {
                if (sem.CurrentCount != 0)
                {
                    sem.Release(sem.CurrentCount);
                }
            }
            _channel.SendingLog += (sender, args) => { _eventSemaphores[SendingLogSemaphoreIdx].Release(); };
            _channel.SentLog += (sender, args) => { _eventSemaphores[SentLogSemaphoreIdx].Release(); };
            _channel.FailedToSendLog += (sender, args) => { _eventSemaphores[FailedToSendLogSemaphoreIdx].Release(); };
            _channel.EnqueuingLog += (sender, args) => { _eventSemaphores[EnqueuingLogSemaphoreIdx].Release(); };       
        }

        private bool FailedToSendLogOccurred(int numTimes, int waitTime = DefaultWaitTime)
        {
            return EventWithSemaphoreOccurred(_eventSemaphores[FailedToSendLogSemaphoreIdx], numTimes, waitTime);
        }

        private bool EnqueuingLogOccurred(int numTimes, int waitTime = DefaultWaitTime)
        {
            return EventWithSemaphoreOccurred(_eventSemaphores[EnqueuingLogSemaphoreIdx], numTimes, waitTime);
        }

        private bool SentLogOccurred(int numTimes, int waitTime = DefaultWaitTime)
        {
            return EventWithSemaphoreOccurred(_eventSemaphores[SentLogSemaphoreIdx], numTimes, waitTime);
        }

        private bool SendingLogOccurred(int numTimes, int waitTime = DefaultWaitTime)
        {
            return EventWithSemaphoreOccurred(_eventSemaphores[SendingLogSemaphoreIdx], numTimes, waitTime);
        }

        private static bool EventWithSemaphoreOccurred(SemaphoreSlim semaphore, int numTimes, int waitTime)
        {
            var enteredAll = true;
            for (var i = 0; i < numTimes; ++i)
            {
                enteredAll &= semaphore.Wait(waitTime);
            }
            return enteredAll;
        }
    }
}
