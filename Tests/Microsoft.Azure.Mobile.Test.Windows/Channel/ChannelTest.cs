using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    [TestClass]
    public class ChannelTest
    {
        private Mobile.Channel.Channel _channel;
        private readonly MockIngestion _mockIngestion = new MockIngestion();
        private readonly IStorage _storage = new Mobile.Storage.Storage();

        private const string ChannelName = "channelName";
        private const int MaxLogsPerBatch = 3;
        private readonly TimeSpan _batchTimeSpan = TimeSpan.FromSeconds(1);
        private const int MaxParallelBatches = 3;
        private readonly string _appSecret = Guid.NewGuid().ToString();
        private const int DefaultWaitTime = 5000;

        // Event semaphores for invokation verification
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
        /// Verify that when a batch reaches its capacity, it is sent immediately
        /// </summary>
        [TestMethod]
        public void EnqueueMaxLogs()
        {
            SetChannelWithTimeSpan(TimeSpan.FromHours(1));
            for (var i = 0; i < MaxLogsPerBatch; ++i)
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

//        [TestMethod]
        public void ChannelInvokesSendingLogEvent()
        {
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.IsTrue(SendingLogOccurred(MaxLogsPerBatch));
        }

//        [TestMethod]
        public void ChannelInvokesSentLogEvent()
        {
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.IsTrue(SentLogOccurred(MaxLogsPerBatch));
        }

//        [TestMethod]
        public void ChannelInvokesFailedToSendLogEvent()
        {
            MakeIngestionCallsFail();
            for (var i = 0; i < MaxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.IsTrue(FailedToSendLogOccurred(MaxLogsPerBatch));
        }

        /// <summary>
        /// Validate that links are same on an error and a log
        /// </summary>
        [TestMethod]
        public void FailedToSendLogEventArgsAreSame()
        {
            var ex = new Exception();
            var log = new TestLog();
            var failedEventLogArgs = new FailedToSendLogEventArgs(log, ex);
            Assert.AreSame(log, failedEventLogArgs.Log);
            Assert.AreSame(ex, failedEventLogArgs.Exception);
        }

        /// <summary>
        /// Validate that channel will send log after enabling
        /// </summary>
 //       [TestMethod]
        public void ChannelInvokesSendingLogEventAfterEnabling()
        {
            _channel.Shutdown();
            for (int i = 0; i < MaxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            // Wait while log is saving; changing enabled value of channel too soon
            // will invalidate its currently running operations
            Task.Delay(1000).Wait();

            _channel.SetEnabled(true);

            Assert.IsTrue(SendingLogOccurred(MaxLogsPerBatch));
        }

        /// <summary>
        /// Validate that FailedToSendLog calls when channel is disabled
        /// </summary>
//      [TestMethod]
        public void ChannelInvokesFailedToSendLogEventAfterDisabling()
        {
            _channel.SetEnabled(false);
            for (int i = 0; i < MaxLogsPerBatch; ++i)
            {
                _channel.Enqueue(new TestLog());
            }

            Assert.IsTrue(SendingLogOccurred(MaxLogsPerBatch));
            Assert.IsTrue(FailedToSendLogOccurred(MaxLogsPerBatch));
        }

        /// <summary>
        /// Validate that all logs removed
        /// </summary>
        [TestMethod]
        public void ClearLogs()
        {
            _channel.Shutdown();
            _channel.Enqueue(new TestLog());

            Task.Delay(DefaultWaitTime).Wait();

            _channel.Clear();
            _channel.SetEnabled(true);

            Assert.IsFalse(SendingLogOccurred(1));
        }

        /// <summary>
        /// Validate that channel's mutex is disposed
        /// </summary>
        [TestMethod]
        public void DisposeChannelTest()
        {
            _channel.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => _channel.SetEnabled(true));
        }

        /// <summary>
        /// Validate that StorageException is processing without exception
        /// </summary>
        [TestMethod]
        public void ThrowStorageExceptionInDeleteLogsTime()
        {
            var storage = new Mock<IStorage>();
            storage.Setup(s => s.DeleteLogsAsync(It.IsAny<string>(), It.IsAny<string>())).Throws<StorageException>();
            storage.Setup(s => s.GetLogsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<Log>>())).Returns(Task.FromResult(""));

            Mobile.Channel.Channel channel = new Mobile.Channel.Channel("name", 1, _batchTimeSpan, 1, _appSecret, _mockIngestion, storage.Object);

            //Shutdown channel and store some log
            channel.Shutdown();
            channel.Enqueue(new TestLog());

            //Wait while log is saving
            Task.Delay(1000).Wait();

            channel.SetEnabled(true);

            // Not throw any exception
        }

        private void SetChannelWithTimeSpan(TimeSpan timeSpan)
        {
            _storage.DeleteLogsAsync(ChannelName).Wait();
            _channel = new Mobile.Channel.Channel(ChannelName, MaxLogsPerBatch, timeSpan, MaxParallelBatches,
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
            for (var i = 0; i < numTimes; ++i)
            {
                if (!semaphore.Wait(waitTime))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
