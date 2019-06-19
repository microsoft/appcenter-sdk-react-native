// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils.Fakes;
using Castle.DynamicProxy.Generators;
using System.Runtime.Remoting.Messaging;
using Microsoft.QualityTools.Testing.Fakes;

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

                // enabled state is applied
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
            // disabled state is applied

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
    }
}
