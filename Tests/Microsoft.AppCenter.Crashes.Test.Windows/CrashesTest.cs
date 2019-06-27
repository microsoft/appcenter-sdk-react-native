// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
            Crashes.ShouldProcessErrorReport += (ErrorReport report) => true;
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
            Crashes.ShouldProcessErrorReport += (ErrorReport report) => true;
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
            Crashes.ShouldProcessErrorReport += (ErrorReport report) => true;
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
            Crashes.ShouldProcessErrorReport += (ErrorReport report) => true;
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify crashes logs have been queued to the channel.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ManagedErrorLog>()), Times.Never());
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(It.IsAny<Guid>()), Times.Never());
            Mock.Get(mockFile).Verify(file => file.Delete(), Times.Once());
        }
    }
}
