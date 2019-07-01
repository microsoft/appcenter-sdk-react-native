// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
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
            Mock.Get(mockErrorLogHelper).Verify(instance => instance.InstanceSaveErrorLogFile(It.IsAny<ManagedErrorLog>()));
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
            var mockFile1 = Mock.Of<File>();
            var mockFile2 = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedprocessId = 123;
            var expectedManagedErrorLog1 = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device(),
                ProcessId = expectedprocessId
            };
            var expectedManagedErrorLog2 = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device(),
                ProcessId = expectedprocessId
            };

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile1, mockFile2 });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile1)).Returns(expectedManagedErrorLog1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile2)).Returns(expectedManagedErrorLog2);

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
            var mockFile = Mock.Of<File>();
            var mockCorruptedFile = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device(),
            };

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile, mockCorruptedFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile)).Returns(expectedManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockCorruptedFile)).Returns<ManagedErrorLog>(null);

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify the corrupted file got ignored and deleted.
            _mockChannel.Verify(channel => channel.EnqueueAsync(expectedManagedErrorLog), Times.Once());
            Mock.Get(mockCorruptedFile).Verify(file => file.Delete(), Times.Once());
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
        public void LastSessionErrorReportIsNullBeforeStart()
        {
            Assert.IsNull(Crashes.GetLastSessionCrashReportAsync().Result);
            Assert.IsFalse(Crashes.HasCrashedInLastSessionAsync().Result);
        }

        [TestMethod]
        public void LastSessionCrashReportWhenEnabledAndCrashOnDisk()
        {
            var oldFile = Mock.Of<File>();
            Mock.Get(oldFile).SetupGet(f => f.LastWriteTime).Returns(DateTime.Now.AddDays(-200));
            var recentFile = Mock.Of<File>();
            Mock.Get(recentFile).SetupGet(f => f.LastWriteTime).Returns(DateTime.Now);
            var expectedFiles = new List<File> { oldFile, recentFile };
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = new Microsoft.AppCenter.Ingestion.Models.Device() };
            
            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(expectedFiles);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(recentFile)).Returns(expectedManagedErrorLog);

            // Start crashes service in an enabled state to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNotNull(lastSessionErrorReport);
            Assert.AreEqual(expectedManagedErrorLog.Id.ToString(), lastSessionErrorReport.Id);
            Assert.IsTrue(hasCrashedInLastSession);
        }

        [TestMethod]
        public void LastSessionErrorReportWhenEnabledAndNoCrashOnDisk()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns<List<File>>(null);

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
        public void OnChannelGroupReadyGetsLastSessionErrorReportWhenEnabledAndInvalidCrashOnDisk()
        {
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var mockFile = Mock.Of<File>();
            Mock.Get(mockFile).SetupGet(file => file.LastWriteTime).Returns(DateTime.UtcNow);

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
        public void DisablingCrashesCleansUpLastSessionReport()
        {
            var mockFile = Mock.Of<File>();
            Mock.Get(mockFile).SetupGet(file => file.LastWriteTime).Returns(DateTime.UtcNow);
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = new Microsoft.AppCenter.Ingestion.Models.Device() };

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile)).Returns(expectedManagedErrorLog);

            // Start crashes service in an enabled to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Make sure that the report is not null.
            Assert.IsNotNull(lastSessionErrorReport);

            // Disable crashes and retrieve the error report again.
            Crashes.SetEnabledAsync(false).Wait();
            hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNull(lastSessionErrorReport);
            Assert.IsFalse(hasCrashedInLastSession);
        }

        [TestMethod]
        public void OnChannelGroupReadyDoesNotGetLastSessionErrorReportWhenDisabledAndCrashOnDisk()
        {
            var mockFile = Mock.Of<File>();
            Mock.Get(mockFile).SetupGet(file => file.LastWriteTime).Returns(DateTime.UtcNow);
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = new Microsoft.AppCenter.Ingestion.Models.Device() };

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile)).Returns(expectedManagedErrorLog);

            // Start crashes service in an enabled state to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNull(lastSessionErrorReport);
            Assert.IsFalse(hasCrashedInLastSession);
        }

        [TestMethod]
        [DataRow(typeof(System.IO.IOException))]
        [DataRow(typeof(PlatformNotSupportedException))]
        [DataRow(typeof(ArgumentOutOfRangeException))]
        public void GetLastSessionCrashReportDoesNotThrowOrHangWhenLastWriteTimeThrows(Type exceptionType)
        {
            // Use reflection to create an exception of the given C# type.
            var exception = exceptionType.GetConstructor(Type.EmptyTypes).Invoke(null) as System.Exception;

            // Mock multiple error log files.
            var oldFile = Mock.Of<File>();
            Mock.Get(oldFile).SetupGet(f => f.LastWriteTime).Throws(exception);
            var recentFile = Mock.Of<File>();
            Mock.Get(recentFile).SetupGet(f => f.LastWriteTime).Throws(exception);
            var expectedFiles = new List<File> { oldFile, recentFile };
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(expectedFiles);

            // Start crashes service in an enabled state to initiate the process of getting the error report.
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
            Mock.Get(oldFile).SetupGet(f => f.LastWriteTime).Returns(DateTime.Now.AddDays(-200));
            var recentFile = Mock.Of<File>();
            Mock.Get(recentFile).SetupGet(f => f.LastWriteTime).Returns(DateTime.Now);
            var expectedFiles = new List<File> { oldFile, recentFile };
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog = new ManagedErrorLog { Id = Guid.NewGuid(), AppLaunchTimestamp = DateTime.Now, Timestamp = DateTime.Now, Device = new Microsoft.AppCenter.Ingestion.Models.Device() };

            // Stub get/read error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(expectedFiles);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(recentFile)).Throws(new System.Exception());

            // Start crashes service in an enabled state to initiate the process of getting the error report.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var hasCrashedInLastSession = Crashes.HasCrashedInLastSessionAsync().Result;
            var lastSessionErrorReport = Crashes.GetLastSessionCrashReportAsync().Result;

            // Verify results.
            Assert.IsNull(lastSessionErrorReport);
            Assert.IsFalse(hasCrashedInLastSession);
        }
    }
}
