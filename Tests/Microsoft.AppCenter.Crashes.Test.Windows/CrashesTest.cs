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

        private const int ExpectedProcessId = 123;

        private readonly ManagedErrorLog _expectedManagedErrorLog = new ManagedErrorLog
        {
            Id = Guid.NewGuid(),
            ProcessId = ExpectedProcessId,
            AppLaunchTimestamp = DateTime.Now,
            Timestamp = DateTime.Now,
            Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>()
        };

        private readonly System.Exception _expectedException = new DivideByZeroException();

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
            Crashes.ShouldProcessErrorReport = null;
            Crashes.ShouldAwaitUserConfirmation = null;
            Crashes.GetErrorAttachments = null;
            AppCenter.Instance.ApplicationSettings.Remove(Crashes.PrefKeyAlwaysSend);
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
        public void OnChannelGroupReadySendsAllPendingCrashesIfShouldProcessNotImplemented()
        {
            var mockErrorLogFile1 = Mock.Of<File>();
            var mockErrorLogFile2 = Mock.Of<File>();
            var mockExceptionFile1 = Mock.Of<File>();
            var mockExceptionFile2 = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog1 = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device(),
                ProcessId = ExpectedProcessId
            };
            var expectedManagedErrorLog2 = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device(),
                ProcessId = ExpectedProcessId
            };

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
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog1.Id && log.ProcessId == ExpectedProcessId)), Times.Once());
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog2.Id && log.ProcessId == ExpectedProcessId)), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog1.Id), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog2.Id), Times.Once());
        }

        [TestMethod]
        public void OnChannelGroupReadySendsFilteredPendingCrashes()
        {
            var mockErrorLogFile1 = Mock.Of<File>();
            var mockErrorLogFile2 = Mock.Of<File>();
            var mockExceptionFile1 = Mock.Of<File>();
            var mockExceptionFile2 = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog1 = new ManagedErrorLog { Id = Guid.NewGuid(), ProcessId = ExpectedProcessId, AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };
            var expectedManagedErrorLog2 = new ManagedErrorLog { Id = Guid.NewGuid(), ProcessId = ExpectedProcessId, AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>() };

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile1, mockErrorLogFile2 });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile1)).Returns(expectedManagedErrorLog1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile2)).Returns(expectedManagedErrorLog2);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog1.Id)).Returns(mockExceptionFile1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog2.Id)).Returns(mockExceptionFile2);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile1)).Returns(new System.Exception());
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile2)).Returns(new System.Exception());

            // Implement ShouldProcess to send only one of the 2 crashes.
            Crashes.ShouldProcessErrorReport += report => report.Id == expectedManagedErrorLog2.Id.ToString();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify only the should processed logs have been queued to the channel.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog1.Id && log.ProcessId == ExpectedProcessId)), Times.Never());
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog2.Id && log.ProcessId == ExpectedProcessId)), Times.Once());

            // Either way, all log files are deleted.
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog1.Id), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog2.Id), Times.Once());

            // We remove exception file if filtering out.
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(expectedManagedErrorLog1.Id), Times.Once());

            // We keep the exception file until sent or failed to send when processed. See other tests for sent/failed to send that verify that.
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(expectedManagedErrorLog2.Id), Times.Never());
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void OnChannelGroupReadyDoesNotSendPendingCrashes(bool enabled)
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File>());

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
            var mockErrorLogFile = Mock.Of<File>();
            var corruptedId1 = Guid.NewGuid();
            var mockErrorLogCorruptedFile1 = Mock.Of<File>();
            var mockErrorLogCorruptedFile2 = Mock.Of<File>();
            var mockExceptionFile = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile, mockErrorLogCorruptedFile1, mockErrorLogCorruptedFile2 });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile)).Returns(_expectedManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogCorruptedFile1)).Returns<ManagedErrorLog>(null);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogCorruptedFile2)).Returns<ManagedErrorLog>(null);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(_expectedManagedErrorLog.Id)).Returns(mockExceptionFile);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile)).Returns(new System.Exception());
            Mock.Get(mockErrorLogCorruptedFile1).Setup(file => file.Delete()).Throws(new System.IO.IOException());
            Mock.Get(mockErrorLogCorruptedFile1).Setup(file => file.Name).Returns($"{corruptedId1}.exception");

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify the corrupted file got ignored and deleted, even if deletion fails.
            Mock.Get(mockErrorLogCorruptedFile1).Verify(file => file.Delete(), Times.Once());
            Mock.Get(mockErrorLogCorruptedFile2).Verify(file => file.Delete(), Times.Once());

            // Verify we deleted the exception file if we could match the file name.
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(corruptedId1), Times.Once());

            // The regular process file has just the json file being deleted and exception file being kept.
            _mockChannel.Verify(channel => channel.EnqueueAsync(_expectedManagedErrorLog), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Never());
        }

        [TestMethod]
        public void ProcessPendingErrorWithCorruptedFieldsIsDeleted()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var mockFile = Mock.Of<File>();
            var errorId = Guid.NewGuid();

            // Mock we don't have an exception file.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile)).Returns(new ManagedErrorLog { Id = errorId });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockFile)).Returns<SystemException>(null);

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify crashes logs have not been queued to the channel and we deleted files.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ManagedErrorLog>()), Times.Never());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(errorId), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(errorId), Times.Once());
        }

        [TestMethod]
        public void SubscribeAndUnsubscribeSendingAndSentCallbacks()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

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
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id)), Times.Once());

            // Simulate and verify sending callback is called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(_expectedManagedErrorLog));
            Assert.IsNotNull(actualSendingReport);
            Assert.AreEqual(_expectedManagedErrorLog.Id.ToString(), actualSendingReport.Id);
            Assert.AreEqual(_expectedException, actualSendingReport.Exception);
            Assert.IsNotNull(actualSendingReport.Device);
            Assert.AreEqual(_expectedManagedErrorLog.AppLaunchTimestamp.Value.Ticks, actualSendingReport.AppStartTime.Ticks);
            Assert.AreEqual(_expectedManagedErrorLog.Timestamp.Value.Ticks, actualSendingReport.AppErrorTime.Ticks);
            Assert.IsNull(actualSendingReport.AndroidDetails);
            Assert.IsNull(actualSendingReport.iOSDetails);
            Assert.AreEqual(1, sendingReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Never());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(Mock.Of<Log>()));

            // Simulate and verify sent callback is called.
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(_expectedManagedErrorLog));
            Assert.AreSame(actualSendingReport, actualSentReport);
            Assert.AreEqual(1, sentReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Once());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(Mock.Of<Log>()));

            // Disable crashes.
            Crashes.SetEnabledAsync(false).Wait();

            // Simulate and verify sending callback isn't called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(_expectedManagedErrorLog));
            Assert.AreEqual(1, sendingReportCallCount);

            // Simulate and verify sent callback isn't called.
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(_expectedManagedErrorLog));
            Assert.AreEqual(1, sentReportCallCount);
        }

        [TestMethod]
        public void SubscribeAndUnsubscribeSendingAndFailedToSendCallbacks()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();
            var expectedFailedToSendException = new System.IO.IOException("broken pipe");

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
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id)), Times.Once());

            // Simulate and verify sending callback is called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(_expectedManagedErrorLog));
            Assert.IsNotNull(actualSendingReport);
            Assert.AreEqual(_expectedManagedErrorLog.Id.ToString(), actualSendingReport.Id);
            Assert.AreEqual(_expectedException, actualSendingReport.Exception);
            Assert.IsNotNull(actualSendingReport.Device);
            Assert.AreEqual(_expectedManagedErrorLog.AppLaunchTimestamp.Value.Ticks, actualSendingReport.AppStartTime.Ticks);
            Assert.AreEqual(_expectedManagedErrorLog.Timestamp.Value.Ticks, actualSendingReport.AppErrorTime.Ticks);
            Assert.IsNull(actualSendingReport.AndroidDetails);
            Assert.IsNull(actualSendingReport.iOSDetails);
            Assert.AreEqual(1, sendingReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Never());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(Mock.Of<Log>()));

            // Simulate and verify sent callback is called.
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(_expectedManagedErrorLog, expectedFailedToSendException));
            Assert.AreSame(actualSendingReport, actualFailedToSentReport);
            Assert.AreSame(expectedFailedToSendException, actualFailedToSendException);
            Assert.AreEqual(1, failedToSendReportCallCount);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Once());

            // Check unknown log type does not crash.
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(Mock.Of<Log>(), new System.Exception()));

            // Disable crashes.
            Crashes.SetEnabledAsync(false).Wait();

            // Simulate and verify sending callback isn't called.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(_expectedManagedErrorLog));
            Assert.AreEqual(1, sendingReportCallCount);

            // Simulate and verify sent callback isn't called.
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(_expectedManagedErrorLog, new System.Exception()));
            Assert.AreEqual(1, failedToSendReportCallCount);
        }

        [TestMethod]
        public void EventTriggeredWhenExceptionFileCannotBeFound()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            Mock.Get(ErrorLogHelper.Instance)
                .Setup(instance => instance.InstanceGetStoredExceptionFile(_expectedManagedErrorLog.Id)).Returns(default(File));

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
            ErrorReport failedToSendReport = null;
            var failedToSendReportCallCount = 0;
            Crashes.FailedToSendErrorReport += (_, e) =>
            {
                failedToSendReport = e.Report;
                failedToSendReportCallCount++;
            };

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // None of the events work if the exception file is unreadable.
            _mockChannelGroup.Raise(channel => channel.SendingLog += null, null, new SendingLogEventArgs(_expectedManagedErrorLog));
            _mockChannelGroup.Raise(channel => channel.SentLog += null, null, new SentLogEventArgs(_expectedManagedErrorLog));
            _mockChannelGroup.Raise(channel => channel.FailedToSendLog += null, null, new FailedToSendLogEventArgs(_expectedManagedErrorLog, new System.Exception()));
            Assert.AreEqual(1, sendingReportCallCount);
            Assert.AreEqual(1, sentReportCallCount);
            Assert.AreEqual(1, failedToSendReportCallCount);
            Assert.IsNotNull(actualSendingReport);
            Assert.IsNull(actualSendingReport.Exception);
            Assert.IsNotNull(actualSentReport);
            Assert.IsNull(actualSentReport.Exception);
            Assert.IsNotNull(failedToSendReport);
            Assert.IsNull(failedToSendReport.Exception);
        }

        [TestMethod]
        public void LastSessionErrorReportIsNullBeforeStart()
        {
            Assert.IsNull(Crashes.GetLastSessionCrashReportAsync().Result);
            Assert.IsFalse(Crashes.HasCrashedInLastSessionAsync().Result);
        }

        [TestMethod]
        public void LastSessionCrashReportWhenEnabledAndCrashOnDisk()
        {
            var oldFile = Mock.Of<File>();
            var recentFile = Mock.Of<File>();
            var expectedFiles = new List<File> { oldFile, recentFile };
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedRecentManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = new Microsoft.AppCenter.Ingestion.Models.Device() };
            var expectedOldManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now.AddDays(-200), Timestamp = DateTime.Now.AddDays(-200), Device = new Microsoft.AppCenter.Ingestion.Models.Device() };
            var recentExceptionFile = Mock.Of<File>();
            var oldExceptionFile = Mock.Of<File>();
            var recentExpectedException = new DivideByZeroException();

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(expectedFiles);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(recentFile)).Returns(expectedRecentManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(oldFile)).Returns(expectedOldManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedRecentManagedErrorLog.Id)).Returns(recentExceptionFile);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedOldManagedErrorLog.Id)).Returns(oldExceptionFile);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(recentExceptionFile)).Returns(recentExpectedException);

            // Start crashes service in an enabled state to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNotNull(lastSessionErrorReport);
            Assert.AreSame(recentExpectedException, lastSessionErrorReport.Exception);
            Assert.AreEqual(expectedRecentManagedErrorLog.Id.ToString(), lastSessionErrorReport.Id);
            Assert.IsTrue(hasCrashedInLastSession);
        }

        [TestMethod]
        public void LastSessionErrorReportWhenEnabledAndNoCrashOnDisk()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File>());

            // Start crashes service in an enabled state to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNull(lastSessionErrorReport);
            Assert.IsFalse(hasCrashedInLastSession);
        }

        [TestMethod]
        public void GetLastSessionErrorReportWhenEnabledAndInvalidCrashOnDisk()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var mockFile = Mock.Of<File>();

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(It.IsAny<File>())).Returns<ManagedErrorLog>(null);

            // Start crashes service in an enabled state to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNull(lastSessionErrorReport);
            Assert.IsFalse(hasCrashedInLastSession);
        }

        [TestMethod]
        public void DisablingCrashesCleansUpUnprocessedManagedErrorLogs()
        {
            // Make a pending error log file.
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => true;
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify the unprocessed managed error log is cleaned up after disabling crashes.
            Assert.AreEqual(Crashes.Instance._unprocessedManagedErrorLogs.Count, 1);
            Crashes.SetEnabledAsync(false).Wait();
            Assert.AreEqual(Crashes.Instance._unprocessedManagedErrorLogs.Count, 0);
        }

        [TestMethod]
        public void DisablingCrashesCleansUpLastSessionReport()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start crashes service in an enabled to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Make sure that the report is not null.
            Assert.IsNotNull(lastSessionErrorReport);
            Assert.IsTrue(hasCrashedInLastSession);

            // Disable crashes and retrieve the error report again.
            Crashes.SetEnabledAsync(false).Wait();
            hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNull(lastSessionErrorReport);
            Assert.IsFalse(hasCrashedInLastSession);
        }

        [TestMethod]
        public void DoesNotGetLastSessionErrorReportWhenDisabledAndCrashOnDisk()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start crashes service in a disabled state.
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNull(lastSessionErrorReport);
            Assert.IsFalse(hasCrashedInLastSession);
        }

        [TestMethod]
        public void LastSessionCrashReportWhenMultipleCrashesAndRecentCrashIsInvalid()
        {
            var oldFile = Mock.Of<File>();
            var recentFile = Mock.Of<File>();
            var expectedFiles = new List<File> { oldFile, recentFile };
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedRecentErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now };
            var expectedOldErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now.AddDays(-200), Timestamp = DateTime.Now.AddDays(-200), Device = new Microsoft.AppCenter.Ingestion.Models.Device() };

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(expectedFiles);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(recentFile)).Returns(expectedRecentErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(oldFile)).Returns(expectedOldErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedOldErrorLog.Id)).Returns(Mock.Of<File>());

            // Start crashes service in an enabled state to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNotNull(lastSessionErrorReport);
            Assert.AreEqual(expectedOldErrorLog.Id.ToString(), lastSessionErrorReport.Id);
            Assert.IsTrue(hasCrashedInLastSession);
        }
        
        [TestMethod]
        public void PlatformNotifyUserConfirmationDefault()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify the value has been set.
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
        }

        [TestMethod]
        public void SendCrashReportsOrAwaitUserConfirmationAsyncAlwaysTrue()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            AppCenter.Instance.ApplicationSettings.SetValue(Crashes.PrefKeyAlwaysSend, true);
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify logs have been processed and removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id && log.ProcessId == ExpectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, false);
            Assert.IsTrue(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void SendCrashReportsOrAwaitUserConfirmationAsyncNullCallback()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes. `ShouldAwaitUserConfirmation` is null.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify logs have been processed and removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id && log.ProcessId == ExpectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void SendCrashReportsOrAwaitUserConfirmationAsyncFalseCallback()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => false;
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify the logs have not been processed and have been removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id && log.ProcessId == ExpectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void SendCrashReportsOrAwaitUserConfirmationAsyncTrueCallback()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => true;
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify nothing in HandleUserConfirmationAsync has been called.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id && log.ProcessId == ExpectedProcessId)), Times.Never());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Never());
        }

        [TestMethod]
        public void HandleUserConfirmationAsyncDoNotSend()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();

            // Avoids processing the logs during the ProcessPendingErrorsTask
            Crashes.ShouldAwaitUserConfirmation = () => true;
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();
            Crashes.NotifyUserConfirmation(UserConfirmation.DontSend);

            // Verify the logs have not been processed and have been removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id && log.ProcessId == ExpectedProcessId)), Times.Never());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Once());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void HandleUserConfirmationAsyncSend()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();

            // Avoids processing the logs during the ProcessPendingErrorsTask
            Crashes.ShouldAwaitUserConfirmation = () => true;
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();
            Crashes.NotifyUserConfirmation(UserConfirmation.Send);

            // Verify logs have been processed and removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id && log.ProcessId == ExpectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Once());

            // We need to keep exception file until sent or failed to sent, tested in other tests.
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Never());
        }

        [TestMethod]
        public void HandleUserConfirmationAsyncAlwaysSend()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();

            // Avoids processing the logs during the ProcessPendingErrorsTask
            Crashes.ShouldAwaitUserConfirmation = () => true;
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);

            // Verify logs have been processed and removed, alwaysSendValue is saved.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == _expectedManagedErrorLog.Id && log.ProcessId == ExpectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, false);
            Assert.IsTrue(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(_expectedManagedErrorLog.Id), Times.Once());
           
            // We need to keep exception file until sent or failed to sent, tested in other tests.
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredExceptionFile(_expectedManagedErrorLog.Id), Times.Never());
        }

        [TestMethod]
        public void HandleUserConfirmationAsyncAlwaysSendWhileDisabled()
        {
            ErrorLogHelper.Instance = GenerateMockErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(false).Wait();

            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);

            // The alwaysSend value should never set because we are disabled
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, false);
            Assert.IsFalse(alwaysSendValue);
        }

        /// <summary>
        /// Convenience function to create a mock ErrorLogHelper with a file added.
        /// </summary>
        /// <returns>The mock error log helper.</returns>
        private ErrorLogHelper GenerateMockErrorLogHelperWithPendingFile()
        {
            var mockErrorLogFile = Mock.Of<File>();
            var mockExceptionFile = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read/delete error files.
            Mock.Get(mockErrorLogHelper).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile });
            Mock.Get(mockErrorLogHelper).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile)).Returns(_expectedManagedErrorLog);
            Mock.Get(mockErrorLogHelper).Setup(instance => instance.InstanceGetStoredExceptionFile(_expectedManagedErrorLog.Id)).Returns(mockExceptionFile);
            Mock.Get(mockErrorLogHelper).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile)).Returns(_expectedException);
            return mockErrorLogHelper;
        }
    }
}
