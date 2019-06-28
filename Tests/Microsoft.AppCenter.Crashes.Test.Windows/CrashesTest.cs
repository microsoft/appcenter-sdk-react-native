// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Crashes.Test.Windows
{
    [TestClass]
    public class CrashesTest
    {
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannelUnit> _mockChannel;
        private Mock<IApplicationLifecycleHelper> _mockApplicationLifecycleHelper;

        [TestInitialize]
        public void InitializeCrashTest()
        {
            Crashes.Instance = new Crashes();
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockApplicationLifecycleHelper = new Mock<IApplicationLifecycleHelper>();
            _mockChannelGroup.Setup(group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(),
                    It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            ApplicationLifecycleHelper.Instance = _mockApplicationLifecycleHelper.Object;
        }

        [TestCleanup]
        public void Cleanup()
        {
            // If a mock was set, reset it to null before moving on.
            ErrorLogHelper.Instance = null;
            ApplicationLifecycleHelper.Instance = null;
            Crashes.Instance = null;
        }

        [TestMethod]
        public void InstanceIsNotNull()
        {
            Crashes.Instance = null;
            Assert.IsNotNull(Crashes.Instance);
        }

        [TestMethod]
        public void GetEnabled()
        {
            Crashes.SetEnabledAsync(false).Wait();
            Assert.IsFalse(Crashes.IsEnabledAsync().Result);
            Crashes.SetEnabledAsync(true).Wait();
            Assert.IsTrue(Crashes.IsEnabledAsync().Result);
        }

        [TestMethod]
        public void ApplyEnabledStateStartsListening()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // Raise an arbitrary event for UnhandledExceptionOccurred handler
            _mockApplicationLifecycleHelper.Raise(eventExpression => eventExpression.UnhandledExceptionOccurred += null,
                new UnhandledExceptionOccurredEventArgs(new System.Exception("test")));
            _mockChannel.Verify(channel => channel.SetEnabled(true), Times.Once());
            Mock.Get(mockErrorLogHelper).Verify(instance => instance.InstanceSaveErrorLogFiles(It.IsAny<System.Exception>(), It.IsAny<ManagedErrorLog>()));
        }

        [TestMethod]
        public void ApplyEnabledStateCleansUp()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            _mockChannel.Verify(channel => channel.SetEnabled(false), Times.Once());
            Mock.Get(mockErrorLogHelper).Verify(instance => instance.InstanceRemoveAllStoredErrorLogFiles());
        }

        [TestMethod]
        public void OnChannelGroupReadySendsPendingCrashes()
        {
            var mockErrorLogFile1 = Mock.Of<File>();
            var mockErrorLogFile2 = Mock.Of<File>();
            var mockExceptionFile1 = Mock.Of<File>();
            var mockExceptionFile2 = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedprocessId = 123;
            var expectedManagedErrorLog1 = new ManagedErrorLog { Id = Guid.NewGuid(), ProcessId = expectedprocessId, AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };
            var expectedManagedErrorLog2 = new ManagedErrorLog { Id = Guid.NewGuid(), ProcessId = expectedprocessId, AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile1, mockErrorLogFile2 });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile1)).Returns(expectedManagedErrorLog1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile2)).Returns(expectedManagedErrorLog2);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog1.Id)).Returns(mockExceptionFile1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog2.Id)).Returns(mockExceptionFile2);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile1)).Returns(new System.Exception());
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile2)).Returns(new System.Exception());

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify crashes logs have been queued to the channel.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog1.Id && log.ProcessId == expectedprocessId)), Times.Once());
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog2.Id && log.ProcessId == expectedprocessId)), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog1.Id), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog2.Id), Times.Once());
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void OnChannelGroupReadyDoesNotSendPendingCrashes(bool enabled)
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { });

            Crashes.SetEnabledAsync(enabled).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            if (enabled)
            {
                Crashes.Instance.ProcessPendingErrorsTask.Wait();
            }

            // Verify no crashes logs have been queued to the channel.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ManagedErrorLog>()), Times.Never());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(It.IsAny<Guid>()), Times.Never());
        }

        [TestMethod]
        public void ProcessPendingErrorsExcludesCorruptedFiles()
        {
            // TODO: We need to verify exception files get deleted.
            var mockErrorLogFile = Mock.Of<File>();
            var mockErrorLogCorruptedFile = Mock.Of<File>();
            var mockExceptionFile = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile, mockErrorLogCorruptedFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile)).Returns(expectedManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogCorruptedFile)).Returns<ManagedErrorLog>(null);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog.Id)).Returns(mockExceptionFile);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile)).Returns(new System.Exception());

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify the corrupted file got ignored and deleted.
            _mockChannel.Verify(channel => channel.EnqueueAsync(expectedManagedErrorLog), Times.Once());
            Mock.Get(mockErrorLogCorruptedFile).Verify(file => file.Delete(), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void ProcessPendingErrorsDoesNotCrashOnFileDeletionFailure()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var mockFile = Mock.Of<File>();
            Mock.Get(mockFile).Setup(file => file.Delete()).Throws(new System.IO.FileNotFoundException());

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile)).Returns<ManagedErrorLog>(null);

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify crashes logs have been queued to the channel.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ManagedErrorLog>()), Times.Never());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(It.IsAny<Guid>()), Times.Never());
            Mock.Get(mockFile).Verify(file => file.Delete(), Times.Once());
        }

        [TestMethod]
        public void SubscribeAndUnsubscribeSendingAndSentCallbacks()
        {
            var mockErrorLogFile = Mock.Of<File>();
            var mockExceptionFile = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.UtcNow, Timestamp = DateTime.UtcNow, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };
            var expectedException = new ArgumentException("ttl must be positive");

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile)).Returns(expectedManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog.Id)).Returns(mockExceptionFile);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile)).Returns(expectedException);

            // Subscribe to callbacks.
            ErrorReport actualSendingReport = null;
            var sendingReportCallCount = 0;
            Crashes.SendingErrorReport += (_, e) =>
            {
                actualSendingReport = e.Report;
                sendingReportCallCount++;
            };
            ErrorReport actualSentReport = null;
            var sentReportCallCount = 0;
            Crashes.SentErrorReport += (_, e) =>
            {
                actualSentReport = e.Report;
                sentReportCallCount++;
            };

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify crashes logs have been queued to the channel on start.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id)), Times.Once());

            // Simulate and verify sending callback is called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(expectedManagedErrorLog));
            Assert.IsNotNull(actualSendingReport);
            Assert.AreEqual(expectedManagedErrorLog.Id.ToString(), actualSendingReport.Id);
            Assert.AreEqual(expectedException, actualSendingReport.Exception);
            Assert.IsNotNull(actualSendingReport.Device);
            Assert.AreEqual(expectedManagedErrorLog.AppLaunchTimestamp.Value.Ticks, actualSendingReport.AppStartTime.Ticks);
            Assert.AreEqual(expectedManagedErrorLog.Timestamp.Value.Ticks, actualSendingReport.AppErrorTime.Ticks);
            Assert.IsNull(actualSendingReport.AndroidDetails);
            Assert.IsNull(actualSendingReport.iOSDetails);
            Assert.AreEqual(1, sendingReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(expectedManagedErrorLog.Id), Times.Never());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(Mock.Of<Log>()));

            // Simulate and verify sent callback is called.
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(expectedManagedErrorLog));
            Assert.AreSame(actualSendingReport, actualSentReport);
            Assert.AreEqual(1, sentReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(expectedManagedErrorLog.Id), Times.Once());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(Mock.Of<Log>()));

            // Disable crashes.
            Crashes.SetEnabledAsync(false).Wait();

            // Simulate and verify sending callback isn't called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(expectedManagedErrorLog));
            Assert.AreEqual(1, sendingReportCallCount);

            // Simulate and verify sent callback isn't called.
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(expectedManagedErrorLog));
            Assert.AreEqual(1, sentReportCallCount);
        }

        [TestMethod]
        public void SubscribeAndUnsubscribeSendingAndFailedToSendCallbacks()
        {
            var mockErrorLogFile = Mock.Of<File>();
            var mockExceptionFile = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.UtcNow, Timestamp = DateTime.UtcNow, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };
            var expectedException = new ArgumentException("ttl must be positive");
            var expectedFailedToSendException = new System.IO.IOException("broken pipe");

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile)).Returns(expectedManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog.Id)).Returns(mockExceptionFile);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile)).Returns(expectedException);

            // Subscribe to callbacks.
            ErrorReport actualSendingReport = null;
            var sendingReportCallCount = 0;
            Crashes.SendingErrorReport += (_, e) =>
            {
                actualSendingReport = e.Report;
                sendingReportCallCount++;
            };
            ErrorReport actualFailedToSentReport = null;
            System.Exception actualFailedToSendException = null;
            var failedToSendReportCallCount = 0;
            Crashes.FailedToSendErrorReport += (_, e) =>
            {
                actualFailedToSentReport = e.Report;
                actualFailedToSendException = e.Exception as System.Exception;
                failedToSendReportCallCount++;
            };

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify crashes logs have been queued to the channel on start.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id)), Times.Once());

            // Simulate and verify sending callback is called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(expectedManagedErrorLog));
            Assert.IsNotNull(actualSendingReport);
            Assert.AreEqual(expectedManagedErrorLog.Id.ToString(), actualSendingReport.Id);
            Assert.AreEqual(expectedException, actualSendingReport.Exception);
            Assert.IsNotNull(actualSendingReport.Device);
            Assert.AreEqual(expectedManagedErrorLog.AppLaunchTimestamp.Value.Ticks, actualSendingReport.AppStartTime.Ticks);
            Assert.AreEqual(expectedManagedErrorLog.Timestamp.Value.Ticks, actualSendingReport.AppErrorTime.Ticks);
            Assert.IsNull(actualSendingReport.AndroidDetails);
            Assert.IsNull(actualSendingReport.iOSDetails);
            Assert.AreEqual(1, sendingReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(expectedManagedErrorLog.Id), Times.Never());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(Mock.Of<Log>()));

            // Simulate and verify sent callback is called.
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(expectedManagedErrorLog, expectedFailedToSendException));
            Assert.AreSame(actualSendingReport, actualFailedToSentReport);
            Assert.AreSame(expectedFailedToSendException, actualFailedToSendException);
            Assert.AreEqual(1, failedToSendReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(expectedManagedErrorLog.Id), Times.Once());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(Mock.Of<Log>(), new System.Exception()));

            // Disable crashes.
            Crashes.SetEnabledAsync(false).Wait();

            // Simulate and verify sending callback isn't called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(expectedManagedErrorLog));
            Assert.AreEqual(1, sendingReportCallCount);

            // Simulate and verify sent callback isn't called.
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(expectedManagedErrorLog, new System.Exception()));
            Assert.AreEqual(1, failedToSendReportCallCount);
        }

        [TestMethod]
        public void EventNotTriggeredWhenExceptionFileCannotBeFound()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.UtcNow, Timestamp = DateTime.UtcNow, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };
            Mock.Get(ErrorLogHelper.Instance)
                .Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog.Id)).Returns(default(File));

            // Subscribe to callbacks.
            var sendingReportCallCount = 0;
            Crashes.SendingErrorReport += (_, e) =>
            {
                sendingReportCallCount++;
            };
            var sentReportCallCount = 0;
            Crashes.SentErrorReport += (_, e) =>
            {
                sentReportCallCount++;
            };
            var failedToSendReportCallCount = 0;
            Crashes.FailedToSendErrorReport += (_, e) =>
            {
                failedToSendReportCallCount++;
            };

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // None of the events work if the exception file is unreadable.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(expectedManagedErrorLog));
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(expectedManagedErrorLog));
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(expectedManagedErrorLog, new System.Exception()));
            Assert.AreEqual(0, sendingReportCallCount);
            Assert.AreEqual(0, sentReportCallCount);
            Assert.AreEqual(0, failedToSendReportCallCount);
        }
    }
}
