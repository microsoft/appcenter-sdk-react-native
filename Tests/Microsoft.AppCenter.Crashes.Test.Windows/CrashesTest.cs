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

        private const int expectedProcessId = 123;

        private ManagedErrorLog expectedManagedErrorLog = new ManagedErrorLog
        {
            Id = Guid.NewGuid(),
            ProcessId = expectedProcessId,
            AppLaunchTimestamp = DateTime.Now,
            Timestamp = DateTime.Now,
            Device = Mock.Of<Microsoft.AppCenter.Ingestion.Models.Device>()
        };

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
        public void PlatformNotifyUserConfirmationDefault()
        {
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

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
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify logs have been processed and removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id && log.ProcessId == expectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, false);
            Assert.IsTrue(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void SendCrashReportsOrAwaitUserConfirmationAsyncNullCallback()
        {
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

            // Start Crashes. `ShouldAwaitUserConfirmation` is null.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify logs have been processed and removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id && log.ProcessId == expectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void SendCrashReportsOrAwaitUserConfirmationAsyncFalseCallback()
        {
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => { return false; };
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify the logs have not been processed and have been removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id && log.ProcessId == expectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void SendCrashReportsOrAwaitUserConfirmationAsyncTrueCallback()
        {
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => { return true; };
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify nothing in HandleUserConfirmationAsync has been called.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id && log.ProcessId == expectedProcessId)), Times.Never());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Never());
        }

        [TestMethod]
        public void HandleUserConfirmationAsyncDontSend()
        {
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => { return true; }; // Avoids processing the logs during the ProcessPendingErrorsTask
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();
            Crashes.NotifyUserConfirmation(UserConfirmation.DontSend);

            // Verify the logs have not been processed and have been removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id && log.ProcessId == expectedProcessId)), Times.Never());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void HandleUserConfirmationAsyncSend()
        {
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => { return true; }; // Avoids processing the logs during the ProcessPendingErrorsTask
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();
            Crashes.NotifyUserConfirmation(UserConfirmation.Send);

            // Verify logs have been processed and removed.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id && log.ProcessId == expectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, true);
            Assert.IsFalse(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Once());
        }

        [TestMethod]
        public void HandleUserConfirmationAsyncAlwaysSend()
        {
            ErrorLogHelper.Instance = GenerateErrorLogHelperWithPendingFile();

            // Start Crashes.
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.ShouldAwaitUserConfirmation = () => { return true; }; // Avoids processing the logs during the ProcessPendingErrorsTask
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();
            Crashes.NotifyUserConfirmation(UserConfirmation.AlwaysSend);

            // Verify logs have been processed and removed, alwaysSendValue is saved.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedManagedErrorLog.Id && log.ProcessId == expectedProcessId)), Times.Once());
            var alwaysSendValue = AppCenter.Instance.ApplicationSettings.GetValue(Crashes.PrefKeyAlwaysSend, false);
            Assert.IsTrue(alwaysSendValue);
            Mock.Get(ErrorLogHelper.Instance).Verify(instance => instance.InstanceRemoveStoredErrorLogFile(expectedManagedErrorLog.Id), Times.Once());
        }

        /// <summary>
        /// Convenience function to create a mock ErrorLogHelper with a file added.
        /// </summary>
        /// <returns></returns>
        private ErrorLogHelper GenerateErrorLogHelperWithPendingFile()
        {
            var mockErrorLogFile = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;

            // Stub get/read/delete error files.
            Mock.Get(mockErrorLogHelper).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockErrorLogFile });
            Mock.Get(mockErrorLogHelper).Setup(instance => instance.InstanceReadErrorLogFile(mockErrorLogFile)).Returns(expectedManagedErrorLog);
            return mockErrorLogHelper;
        }
    }
}
