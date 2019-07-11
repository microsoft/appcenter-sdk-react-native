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
    /// <summary>
    /// This class tests the error attachments functionality of the Crashes class.
    /// </summary>
    [TestClass]
    public class CrashesErrorAttachmentTest
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
            Crashes.GetErrorAttachments = null;
        }

        [TestMethod]
        public void ProcessPendingCrashesSendsErrorAttachment()
        {
            var mockFile1 = Mock.Of<File>();
            var mockFile2 = Mock.Of<File>();
            var mockExceptionFile1 = Mock.Of<File>();
            var mockExceptionFile2 = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var lastExpectedManagedErrorLog = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device()
            };
            var olderExpectedManagedErrorLog = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = lastExpectedManagedErrorLog.Timestamp.Value.AddDays(-1),
                AppLaunchTimestamp = lastExpectedManagedErrorLog.Timestamp.Value.AddDays(-1),
                Device = new Microsoft.AppCenter.Ingestion.Models.Device()
            };
            var validErrorAttachment = GetValidErrorAttachmentLog();
            var expectedException = new ArgumentOutOfRangeException();

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile1, mockFile2 });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile1)).Returns(lastExpectedManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile2)).Returns(olderExpectedManagedErrorLog);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(lastExpectedManagedErrorLog.Id)).Returns(mockExceptionFile1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(olderExpectedManagedErrorLog.Id)).Returns(mockExceptionFile2);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadExceptionFile(mockExceptionFile1)).Returns(expectedException);

            // Implement attachments callback.
            System.Exception actualException = null;
            Crashes.GetErrorAttachments = errorReport =>
            {
                if (errorReport.Id == lastExpectedManagedErrorLog.Id.ToString())
                {
                    actualException = errorReport.Exception;
                    return new List<ErrorAttachmentLog> { validErrorAttachment };
                }
                return null;
            };

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify attachment log has been queued to the channel. (And only one attachment log).
            _mockChannel.Verify(channel => channel.EnqueueAsync(validErrorAttachment), Times.Once());
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ErrorAttachmentLog>()), Times.Once());

            // Verify that the attachment has been modified with the right fields.
            Assert.AreEqual(lastExpectedManagedErrorLog.Id, validErrorAttachment.ErrorId);
            Assert.AreNotEqual(Guid.Empty, validErrorAttachment.ErrorId);

            // Verify exception was attached to report in the callback.
            Assert.AreSame(expectedException, actualException);
        }

        [TestMethod]
        public void ProcessPendingCrashesIgnoresNullErrorAttachment()
        {
            var mockFile1 = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog1 = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device()
            };

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile1 });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile1)).Returns(expectedManagedErrorLog1);

            // Implement attachments callback.
            Crashes.GetErrorAttachments = errorReport =>
            {
                if (errorReport.Id == expectedManagedErrorLog1.Id.ToString())
                {
                    return new List<ErrorAttachmentLog> { null };
                }
                return null;
            };

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify nothing has been enqueued.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ErrorAttachmentLog>()), Times.Never());
        }

        [TestMethod]
        public void ProcessPendingCrashesIgnoresInvalidErrorAttachmentWithoutCrashing()
        {
            var mockFile1 = Mock.Of<File>();
            var mockFile2 = Mock.Of<File>();
            var mockExceptionFile1 = Mock.Of<File>();
            var mockExceptionFile2 = Mock.Of<File>();
            var mockErrorLogHelper = Mock.Of<ErrorLogHelper>();
            ErrorLogHelper.Instance = mockErrorLogHelper;
            var expectedManagedErrorLog1 = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device()
            };
            var expectedManagedErrorLog2 = new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                AppLaunchTimestamp = DateTime.Now,
                Device = new Microsoft.AppCenter.Ingestion.Models.Device()
            };

            // Stub get/read/delete error files.
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetErrorLogFiles()).Returns(new List<File> { mockFile1, mockFile2 });
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile1)).Returns(expectedManagedErrorLog1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceReadErrorLogFile(mockFile2)).Returns(expectedManagedErrorLog2);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog1.Id)).Returns(mockExceptionFile1);
            Mock.Get(ErrorLogHelper.Instance).Setup(instance => instance.InstanceGetStoredExceptionFile(expectedManagedErrorLog2.Id)).Returns(mockExceptionFile2);

            // Create two valid and one invalid attachment.
            var invalidErrorAttachment1 = new ErrorAttachmentLog();
            var validErrorAttachmentWithoutDevice = new ErrorAttachmentLog()
            {
                ContentType = "ContentType",
                ErrorId = Guid.NewGuid(),
                Data = new byte[] { 1 },
                Id = Guid.NewGuid()
            };
            var validErrorAttachment1 = GetValidErrorAttachmentLog();
            var validErrorAttachment2 = GetValidErrorAttachmentLog();

            // Implement attachments callback.
            Crashes.GetErrorAttachments = errorReport =>
            {
                if (errorReport.Id == expectedManagedErrorLog1.Id.ToString())
                {
                    return new List<ErrorAttachmentLog> { invalidErrorAttachment1, validErrorAttachmentWithoutDevice, validErrorAttachment1 };
                }
                return new List<ErrorAttachmentLog> { validErrorAttachment2 };
            };

            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Crashes.Instance.ProcessPendingErrorsTask.Wait();

            // Verify all valid attachment logs has been queued to the channel, but not invalid one.
            _mockChannel.Verify(channel => channel.EnqueueAsync(invalidErrorAttachment1), Times.Never());
            _mockChannel.Verify(channel => channel.EnqueueAsync(validErrorAttachmentWithoutDevice), Times.Once());
            _mockChannel.Verify(channel => channel.EnqueueAsync(validErrorAttachment1), Times.Once());
            _mockChannel.Verify(channel => channel.EnqueueAsync(validErrorAttachment2), Times.Once());
        }

        private ErrorAttachmentLog GetValidErrorAttachmentLog()
        {
            var device = new Microsoft.AppCenter.Ingestion.Models.Device("sdkName", "sdkVersion", "osName", "osVersion", "locale", 1,
                "appVersion", "appBuild", null, null, "model", "oemName", "osBuild", null, "screenSize", null, null, "appNamespace", null, null, null, null);
            return new ErrorAttachmentLog()
            {
                ContentType = "contenttype",
                Id = Guid.NewGuid(),
                ErrorId = Guid.NewGuid(),
                Data = new byte[] { 1 },
                Device = device
            };
        }
    }
}
