using System;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Windows
{    
    [TestClass]
    public class MobileCenterServiceTest
    {
        private TestMobileCenterService _testService;
        private Mock<IApplicationSettings> _mockSettings;
        private Mock<IChannelUnit> _mockChannel;
        private Mock<IChannelGroup> _mockChannelGroup;

        [TestInitialize]
        public void InitializeMobileCenterServiceTest()
        {
            _mockSettings = new Mock<IApplicationSettings>();
            _testService = new TestMobileCenterService(_mockSettings.Object);
            _mockChannel = new Mock<IChannelUnit>();
            _mockChannelGroup = new Mock<IChannelGroup>();

            _mockChannelGroup.Setup(
                channelGroup =>
                    channelGroup.AddChannel(_testService.PublicChannelName, It.IsAny<int>(), It.IsAny<TimeSpan>(),
                        It.IsAny<int>())).Returns(_mockChannel.Object);
        }

        /// <summary>
        /// Verify that the enabled value comes from application settings
        /// </summary>
        [TestMethod]
        public void GetEnabled()
        {
            _mockSettings.SetupSequence(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>())).Returns(true).Returns(false);

            Assert.IsTrue(_testService.InstanceEnabled);
            Assert.IsFalse(_testService.InstanceEnabled);
            _mockSettings.Verify(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()), Times.Exactly(2));

        }

        /// <summary>
        /// Verify that changing the enabled value without the channel group still updates the stored value
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValueNoChannel()
        {
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);
            MobileCenter.SetEnabledAsync(true).Wait();
            _testService.InstanceEnabled = false;

            _mockSettings.VerifySet(settings => settings[_testService.PublicEnabledPreferenceKey] = false, Times.Once());
        }

        /// <summary>
        /// Verify that setting the enabling a service when mobile center is disabled has no effect
        /// </summary>
        [TestMethod]
        public void EnableServiceWhenMobileCenterIsDisabled()
        {
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);
            MobileCenter.SetEnabledAsync(false).Wait();

            _testService.InstanceEnabled = true;

            _mockSettings.VerifySet(settings => settings[_testService.PublicEnabledPreferenceKey] = It.IsAny<bool>(), Times.Never());
        }

        /// <summary>
        /// Verify that setting the service to enabled when it is already enabled has no effect
        /// </summary>
        [TestMethod]
        public void SetEnabledSameValue()
        {
            _mockSettings.Setup(
                    settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);
            MobileCenter.SetEnabledAsync(true).Wait();

            _testService.InstanceEnabled = true;

            _mockSettings.VerifySet(settings => settings[_testService.PublicEnabledPreferenceKey] = It.IsAny<bool>(), Times.Never());
        }

        /// <summary>
        /// Verify that changing the value of a service updates the application storage and the channel
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValue()
        {
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);
            MobileCenter.SetEnabledAsync(true).Wait();
            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _testService.InstanceEnabled = false;

            _mockSettings.VerifySet(settings => settings[_testService.PublicEnabledPreferenceKey] = It.IsAny<bool>(), Times.Exactly(2));
            _mockChannel.Verify(channel => channel.SetEnabledAsync(It.IsAny<bool>()), Times.Exactly(2));
        }

        /// <summary>
        /// Verify that OnChannelGroupReady sets the channel, channel group, and enabled values appropriately
        /// </summary>
        [TestMethod]
        public void OnChannelGroupReady()
        {
            MobileCenter.SetEnabledAsync(true).Wait();
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
            .Returns(true);
            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockChannelGroup.Verify(
                channelGroup =>
                    channelGroup.AddChannel(_testService.PublicChannelName, It.IsAny<int>(), It.IsAny<TimeSpan>(),
                        It.IsAny<int>()), Times.Once());
            _mockSettings.VerifySet(settings => settings[_testService.PublicEnabledPreferenceKey] = true, Times.Once());
            _mockChannel.Verify(channel => channel.SetEnabledAsync(true), Times.Once());
            Assert.AreSame(_mockChannelGroup.Object, _testService.PublicChannelGroup);
        }

        /// <summary>
        /// Verify that even if a service is enabled, OnChannelGroupReady disables everything if Mobile Center is disabled
        /// </summary>
        [TestMethod]
        public void OnChannelGroupReadyMobileCenterIsDisabled()
        {
            MobileCenter.SetEnabledAsync(false).Wait();
            _mockSettings.Setup(
                    settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);
            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockChannelGroup.Verify(
                channelGroup =>
                    channelGroup.AddChannel(_testService.PublicChannelName, It.IsAny<int>(), It.IsAny<TimeSpan>(),
                        It.IsAny<int>()), Times.Once());
            _mockSettings.VerifySet(settings => settings[_testService.PublicEnabledPreferenceKey] = false, Times.Once());
            _mockChannel.Verify(channel => channel.SetEnabledAsync(false), Times.Once());
        }

        /// <summary>
        /// Verify that if the channel is null, IsInactive returns true
        /// </summary>
        [TestMethod]
        public void IsInactiveWhenChannelIsNull()
        {
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
            .Returns(true);

            Assert.IsTrue(_testService.PublicIsInactive);
        }

        /// <summary>
        /// Verify that if the service is disabled, IsInactive returns true
        /// </summary>
        [TestMethod]
        public void IsInactiveWhenDisabled()
        {
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
            .Returns(false);
            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            Assert.IsTrue(_testService.PublicIsInactive);
        }

        /// <summary>
        /// Verify that if the service is enabled and the channel is not null, IsInactive returns false
        /// </summary>
        [TestMethod]
        public void IsInactiveWhenEnabledAndChannelExists()
        {
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
            .Returns(true);
            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            Assert.IsFalse(_testService.PublicIsInactive);
        }
    }
}
