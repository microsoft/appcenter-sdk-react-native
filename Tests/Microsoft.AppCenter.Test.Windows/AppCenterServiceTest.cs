using System;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Test.Utils;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows
{    
    [TestClass]
    public class AppCenterServiceTest
    {
        private TestAppCenterService _testService;
        private Mock<IApplicationSettings> _mockSettings;
        private Mock<IChannelUnit> _mockChannel;
        private Mock<IChannelGroup> _mockChannelGroup;

        [TestInitialize]
        public void InitializeAppCenterServiceTest()
        {
            _testService = new TestAppCenterService();
            _mockSettings = new Mock<IApplicationSettings>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannelGroup.Setup(
                channelGroup =>
                    channelGroup.AddChannel(_testService.PublicChannelName, It.IsAny<int>(), It.IsAny<TimeSpan>(),
                        It.IsAny<int>())).Returns(_mockChannel.Object);

            AppCenter.Instance = null;
#pragma warning disable 612
            AppCenter.SetApplicationSettingsFactory(new MockApplicationSettingsFactory(_mockSettings));
#pragma warning restore 612
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
            _mockSettings.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true);
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);
            _testService.InstanceEnabled = false;

            _mockSettings.Verify(settings => settings.SetValue(_testService.PublicEnabledPreferenceKey, false), Times.Once());
        }

        /// <summary>
        /// Verify that setting the enabling a service when app center is disabled has no effect
        /// </summary>
        [TestMethod]
        public void EnableServiceWhenAppCenterIsDisabled()
        {
            _mockSettings.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(false);
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);

            _testService.InstanceEnabled = true;

            _mockSettings.Verify(settings => settings.SetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()), Times.Never());
        }

        /// <summary>
        /// Verify that setting the service to enabled when it is already enabled has no effect
        /// </summary>
        [TestMethod]
        public void SetEnabledSameValue()
        {
            _mockSettings.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true);
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);

            _testService.InstanceEnabled = true;

            _mockSettings.Verify(settings => settings.SetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()), Times.Never());
        }

        /// <summary>
        /// Verify that changing the value of a service updates the application storage and the channel
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValue()
        {
            _mockSettings.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true);
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);

            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            _testService.InstanceEnabled = false;

            _mockSettings.Verify(settings => settings.SetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()), Times.Exactly(2));
            _mockChannel.Verify(channel => channel.SetEnabled(It.IsAny<bool>()), Times.Exactly(2));
        }

        /// <summary>
        /// Verify that OnChannelGroupReady sets the channel, channel group, and enabled values appropriately
        /// </summary>
        [TestMethod]
        public void OnChannelGroupReady()
        {
            _mockSettings.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true);
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);

            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockChannelGroup.Verify(
                channelGroup =>
                    channelGroup.AddChannel(_testService.PublicChannelName, It.IsAny<int>(), It.IsAny<TimeSpan>(),
                        It.IsAny<int>()), Times.Once());
            _mockSettings.Verify(settings => settings.SetValue(_testService.PublicEnabledPreferenceKey, true), Times.Once());
            _mockChannel.Verify(channel => channel.SetEnabled(true), Times.Once());
            Assert.AreSame(_mockChannelGroup.Object, _testService.PublicChannelGroup);
        }

        /// <summary>
        /// Verify that even if a service is enabled, OnChannelGroupReady disables everything if App Center is disabled
        /// </summary>
        [TestMethod]
        public void OnChannelGroupReadyAppCenterIsDisabled()
        {
            _mockSettings.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(false);
            _mockSettings.Setup(settings => settings.GetValue(_testService.PublicEnabledPreferenceKey, It.IsAny<bool>()))
                .Returns(true);

            _testService.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockChannelGroup.Verify(
                channelGroup =>
                    channelGroup.AddChannel(_testService.PublicChannelName, It.IsAny<int>(), It.IsAny<TimeSpan>(),
                        It.IsAny<int>()), Times.Once());
            _mockSettings.Verify(settings => settings.SetValue(_testService.PublicEnabledPreferenceKey, false), Times.Once());
            _mockChannel.Verify(channel => channel.SetEnabled(false), Times.Once());
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
