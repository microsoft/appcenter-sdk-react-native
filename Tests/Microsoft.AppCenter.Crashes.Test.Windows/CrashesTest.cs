// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Fakes;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils.Fakes;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Utils;
using Microsoft.QualityTools.Testing.Fakes;
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
            bool passed = false;
            using (ShimsContext.Create())
            {
                ShimErrorLogHelper.SaveErrorLogFileManagedErrorLog = (managedErrorLog) =>
                {
                    passed = true;
                };

                Crashes.SetEnabledAsync(true).Wait();
                Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

                // Raise an arbitrary event for UnhandledExceptionOccurred handler
                _mockApplicationLifecycleHelper.Raise(eventExpression => eventExpression.UnhandledExceptionOccurred += null,
                    new UnhandledExceptionOccurredEventArgs(new System.Exception("test")));

                _mockChannel.Verify(channel => channel.SetEnabled(true), Times.Once());
                Assert.IsTrue(passed);
            }
        }

        [TestMethod]
        public void ApplyEnabledStateCleansUp()
        {
            bool saveErrorLogFileCalled = false;
            bool removeErrorLogFilesCalled = false;
            using (ShimsContext.Create())
            {
                ShimErrorLogHelper.SaveErrorLogFileManagedErrorLog = (managedErrorLog) =>
                {
                    saveErrorLogFileCalled = true;
                };
                ShimErrorLogHelper.RemoveAllStoredErrorLogFiles = () =>
                {
                    removeErrorLogFilesCalled = true;
                };

                Crashes.SetEnabledAsync(false).Wait();
                Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

                // Raise an arbitrary event for UnhandledExceptionOccurred handler
                _mockApplicationLifecycleHelper.Raise(eventExpression => eventExpression.UnhandledExceptionOccurred += null,
                    new UnhandledExceptionOccurredEventArgs(new System.Exception("test")));

                _mockChannel.Verify(channel => channel.SetEnabled(false), Times.Once());
                Assert.IsFalse(saveErrorLogFileCalled);
                Assert.IsTrue(removeErrorLogFilesCalled);
            }
        }

        [TestMethod]
        public void OnChannelGroupReadySendsPendingCrashes()
        {
            var expectedFileInfo1 = new FileInfo("file");
            var expectedFileInfo2 = new FileInfo("file2");
            var expectedprocessId = 123;
            var expectedLogIds = new List<Guid>();
            var removedLogIds = new List<Guid>();
            using (ShimsContext.Create())
            {
                // Stub get/read/delete error files
                ShimErrorLogHelper.GetErrorLogFiles = () => new List<FileInfo> { expectedFileInfo1, expectedFileInfo2 };
                ShimErrorLogHelper.RemoveStoredErrorLogFileGuid = (Guid guid) => removedLogIds.Add(guid);
                ShimErrorLogHelper.ReadErrorLogFileFileInfo = (FileInfo file) =>
                {
                    var errorLog = new ManagedErrorLog
                    {
                        Id = Guid.NewGuid(),
                        ProcessId = expectedprocessId
                    };
                    expectedLogIds.Add(errorLog.Id);
                    return errorLog;
                };

                Crashes.SetEnabledAsync(true).Wait();
                Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

                // Verify crashes logs have been queued to the channel
                foreach (var expectedLogId in expectedLogIds)
                {
                    _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<ManagedErrorLog>(log => log.Id == expectedLogId && log.ProcessId == expectedprocessId)), Times.Once());
                }
                CollectionAssert.AreEqual(expectedLogIds, removedLogIds);
            }
        }

        [TestMethod]
        public void OnChannelGroupReadyDoesNotSendsPendingCrashesOnDisabled()
        {
            using (ShimsContext.Create())
            {
                var getErrorLogFilesCalled = false;
                // Stub get/read/delete error files
                ShimErrorLogHelper.GetErrorLogFiles = () =>
                {
                    getErrorLogFilesCalled = true;
                    return new List<FileInfo>();
                };
                Crashes.SetEnabledAsync(false).Wait();
                Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
                _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ManagedErrorLog>()), Times.Never());
                Assert.IsFalse(getErrorLogFilesCalled);
            }
        }

        [TestMethod]
        public void OnChannelGroupReadyDoesNotSendPendingCrashesIfNoneExist()
        {
            using (ShimsContext.Create())
            {
                // Stub get/read/delete error files
                ShimErrorLogHelper.GetErrorLogFiles = () =>
                {
                    return new List<FileInfo>();
                };
                Crashes.SetEnabledAsync(true).Wait();
                Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
                _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<ManagedErrorLog>()), Times.Never());
            }
        }

        [TestMethod]
        public void ProcessPendingErrorsExcludesCorruptedFiles()
        {
            var corruptedFileName = "corruptedFile";
            var fileDeletionCount = 0;
            var expectedLogId = Guid.NewGuid();
            var actualSentLogIds = new List<Guid>();
            var removedLogIds = new List<Guid>();
            using (ShimsContext.Create())
            {
                // Stub get/read/delete error files
                var file = new FileInfo("file");
                var corruptedFile = new FileInfo(corruptedFileName);
                _mockChannel.Setup(channel => channel.EnqueueAsync(It.IsAny<ManagedErrorLog>())).Callback<Log>(log => actualSentLogIds.Add(((ManagedErrorLog)log).Id));
                ShimFileInfo.AllInstances.Delete = (FileInfo info) => fileDeletionCount++;
                ShimErrorLogHelper.GetErrorLogFiles = () => new List<FileInfo> { file, corruptedFile };
                ShimErrorLogHelper.RemoveStoredErrorLogFileGuid = (Guid guid) => removedLogIds.Add(guid);
                ShimErrorLogHelper.ReadErrorLogFileFileInfo = (FileInfo info) =>
                {
                    if (info.Name == corruptedFileName)
                    {
                        return null;
                    }
                    return new ManagedErrorLog
                    {
                        Id = expectedLogId
                    };
                };

                Crashes.SetEnabledAsync(true).Wait();
                Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
                Assert.AreEqual(actualSentLogIds.Count, 1);
                Assert.AreEqual(actualSentLogIds[0], expectedLogId);
                Assert.AreEqual(removedLogIds.Count, 1);
                Assert.AreEqual(removedLogIds[0], expectedLogId);
                // Valid error log file couldn't be deleted in this test because the test mocks RemoveStoredErrorLogFileGuid which deletes the file.
                Assert.AreEqual(fileDeletionCount, 1);
            }
        }
    }
}
