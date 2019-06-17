// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AppCenter.Crashes.Ingestion.Models;

namespace Microsoft.AppCenter.Crashes.Test.Windows
{
    [TestClass]
    public class CrashesTest
    {
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannelUnit> _mockChannel;
        private Mock<ErrorLogHelper> _mockErrorLogHelper;
        private Mock<IApplicationLifecycleHelper> _mockApplicationLifecycleHelper;

        [TestInitialize]
        public void InitializeCrashTest()
        {
            Crashes.Instance = new Crashes();
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockErrorLogHelper = new Mock<ErrorLogHelper>();
            _mockApplicationLifecycleHelper = new Mock<IApplicationLifecycleHelper>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            //ApplicationLifecycleHelper.Instance = _applicationLifecycleHelper;
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
            // enabled state is applied
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // Raise an arbitrary event for UnhandledExceptionOccurred handler
            _mockApplicationLifecycleHelper.Raise(eventExpression => eventExpression.UnhandledExceptionOccurred += null,
                new UnhandledExceptionOccurredEventArgs(new System.Exception("test")));

            // Channel is enabled and listener is listening
            _mockChannel.Verify(channel => channel.SetEnabled(true), Times.Once());
            _mockErrorLogHelper.Verify(errorLogHelper => ErrorLogHelper.SaveErrorLogFile(It.IsAny<ManagedErrorLog>()), Times.Once());
        }

        [TestMethod]
        public void ApplyEnabledStateCleansUp()
        {
            // disabled state is applied
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockChannel.Verify(channel => channel.SetEnabled(false), Times.Once());
            // listener stops listening
            //_applicationLifecycleHelper.InvokeUnhandledException();

            // listener is listening
            _mockErrorLogHelper.Verify(asdasd => ErrorLogHelper.SaveErrorLogFile(It.IsAny<ManagedErrorLog>()), Times.Never());
            
            // error log files are deleted
            _mockErrorLogHelper.Verify(asdas => ErrorLogHelper.RemoveStoredErrorLogFile(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public void ApplyEnabledStateDeleteFails()
        {
            // disabled state is applied
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // file deletion throws
            
        }
    }
}
