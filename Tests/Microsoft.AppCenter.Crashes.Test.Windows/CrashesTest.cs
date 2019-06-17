// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Crashes.Test.Windows
{
    [TestClass]
    public class CrashesTest
    {

        private ApplicationLifecycleHelper _applicationLifecycleHelper;
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannelUnit> _mockChannel;

        [TestInitialize]
        public void InitializeCrashTest()
        {
            Crashes.Instance = new Crashes();
            ErrorLogHelper.ProcessInformation = Mock.Of<IProcessInformation>();
            ErrorLogHelper.DeviceInformationHelper = Mock.Of<IDeviceInformationHelper>();

            _applicationLifecycleHelper = new ApplicationLifecycleHelper();
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            ApplicationLifecycleHelper.Instance = _applicationLifecycleHelper;
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

            // listener is listening

        }

        [TestMethod]
        public void ApplyEnabledStateCleansUp()
        {
            // disabled state is applied
            Crashes.SetEnabledAsync(false).Wait();
            Crashes.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // listener stops listening
            // error log files are deleted

            // Create the error log.
        }
    }
}
