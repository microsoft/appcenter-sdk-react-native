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
using Xunit;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    public class ChannelTest
    {
        private Mobile.Channel.Channel _channel;
        private MockIngestion _mockIngestion = new MockIngestion();
        private IStorage _storage = new Mobile.Storage.Storage();
        private readonly string _channelName = "channelName";
        private readonly int _maxLogsPerBatch = 3;
        private readonly TimeSpan _batchTimeSpan = TimeSpan.FromSeconds(1);
        private readonly int _maxParallelBatches = 3;
        private readonly string _appSecret = Guid.NewGuid().ToString();
        private readonly Guid _installId = Guid.NewGuid();
        private const int DefaultWaitTime = 5000;

        /* Event semaphores for invokation verification */
        private const int SendingLogSemaphoreIdx = 0;
        private const int SentLogSemaphoreIdx = 1;
        private const int FailedToSendLogSemaphoreIdx = 2;
        private const int EnqueuingLogSemaphoreIdx = 3;
        private readonly List<SemaphoreSlim> _eventSemaphores = new List<SemaphoreSlim> { new SemaphoreSlim(0), new SemaphoreSlim(0), new SemaphoreSlim(0), new SemaphoreSlim(0) };

        public ChannelTest()
        {
            LogSerializer.AddFactory(TestLog.JsonIdentifier, new LogFactory<TestLog>());
            SetChannelWithTimeSpan(_batchTimeSpan);
        }
        
        /// <summary>
        /// Verify that channel is enabled by default
        /// </summary>
        [Fact]
        public void ChannelEnabledByDefault()
        {
            Assert.True(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that disabling channel has the expected effect
        /// </summary>
        [Fact]
        public void DisableChannel()
        {
            _channel.SetEnabled(false);

            Assert.False(_channel.IsEnabled);
        }

        /// <summary>
        /// Verify that enabling the channel has the expected effect
        /// </summary>
        [Fact]
        public void EnableChannel()
        {
            _channel.SetEnabled(false);
            _channel.SetEnabled(true);

            Assert.True(_channel.IsEnabled);
        }
        
        /// <summary>
        /// Verify that enqueuing a log passes the same log reference to enqueue event argument
        /// </summary>
        [Fact]
        public void EnqueuedLogsAreSame()
        {
            var log = new TestLog();
            var sem = new SemaphoreSlim(0);
            _channel.EnqueuingLog += (sender, args) =>
            {
                Assert.Same(log, args.Log);
                sem.Release();
            };

            _channel.Enqueue(log);
            Assert.True(sem.Wait(DefaultWaitTime));
        }

        /// <summary>
        /// Verify that a log is never sent in *less* time than it is supposed to wait (when the batch is not full)
        /// </summary>
        [Fact(Skip="too unstable")]
        public void WaitTime()
        {
            var expectedWaitTimes = new List<double> {500, 1000, 2000};
            var actualWaitTimes = expectedWaitTimes.Select(t => GetWaitTime(t)).ToList();

            for (var i = 0; i < expectedWaitTimes.Count; ++i)
            {
                Assert.True(actualWaitTimes[i] >= expectedWaitTimes[i]);
            }
        }

        /// <summary>
        /// This verifies that the times waited are *roughly* what they should be. It is ignored because
        /// it should not cause any automated testing to fail due to various CPU speeds, but is useful
        /// to test locally
        /// </summary>
        [Fact(Skip="too unpredictable")]
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
                Assert.True(diff >= 0);
            }
            var range = diffs.Max() - diffs.Min();
            Assert.True(range <= errorThresholdMilliseconds);
        }

        /// <summary>
        /// Verify that when a batch reaches its capacity, it is sent immediately
        /// </summary>
        [Fact]
        public void MaxLogsPerBatch()
        {

            SetChannelWithTimeSpan(TimeSpan.FromHours(1));
            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }
            Assert.True(SendingLogOccurred(1));
        }
        
        /// <summary>
        /// Verify that when channel is disabled, sent event is not triggered
        /// </summary>
        [Fact]
        public void EnqueueWhileDisabled()
        {
            SetChannelWithTimeSpan(_batchTimeSpan);

            _channel.SetEnabled(false);
            var log = new TestLog();
            _channel.Enqueue(log);
            Assert.False(SentLogOccurred(1));
        }

        [Fact]
        public void ChannelInvokesSendingLogEvent()
        {
            SetChannelWithTimeSpan(_batchTimeSpan);

            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.True(SendingLogOccurred(_maxLogsPerBatch));
        }

        [Fact]
        public void ChannelInvokesSentLogEvent()
        {
            SetChannelWithTimeSpan(_batchTimeSpan);

            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.True(SentLogOccurred(_maxLogsPerBatch));
        }

        [Fact]
        public void ChannelInvokesFailedToSendLogEvent()
        {
            SetChannelWithTimeSpan(_batchTimeSpan);

            MakeIngestionCallsFail();

            for (var i = 0; i < _maxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.True(FailedToSendLogOccurred(_maxLogsPerBatch));
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
            _storage.DeleteLogsAsync(_channelName).Wait();
            _channel = new Mobile.Channel.Channel(_channelName, _maxLogsPerBatch, timeSpan, _maxParallelBatches,
                _appSecret, _installId, _mockIngestion, _storage);
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
