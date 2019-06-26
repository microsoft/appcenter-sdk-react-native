// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

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
    }
}
