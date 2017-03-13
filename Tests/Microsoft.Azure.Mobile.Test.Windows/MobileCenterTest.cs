using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test
{
    [TestClass]
    public class MobileCenterTest
    {
        [TestInitialize]
        public void InitializeMobileCenterTest()
        {
            MockMobileCenterService.Reset();
            MobileCenter.Reset();
        }

        /// <summary>
        /// Verify that MobileCenter instance is not initially null
        /// </summary>
        [TestMethod]
        public void GetInstanceNotNull()
        {
            Assert.IsNotNull(MobileCenter.Instance);
        }

        /// <summary>
        /// Verify that changing the log level of mobile center has an effect.
        /// </summary>
        [TestMethod]
        public void SetLogLevels()
        {
            VerifySetLogLevel(LogLevel.Assert);
            VerifySetLogLevel(LogLevel.Debug);
            VerifySetLogLevel(LogLevel.Error);
            VerifySetLogLevel(LogLevel.Info);
            VerifySetLogLevel(LogLevel.Verbose);
            VerifySetLogLevel(LogLevel.None);
        }

        /// <summary>
        /// Verify that Mobile Center is able to get a log level even though it hasn't set one
        /// </summary>
        [TestMethod]
        public void GetLogLevelBeforeConfigure()
        {
            MobileCenterLog.Level = LogLevel.Info;

            Assert.AreEqual(LogLevel.Info, MobileCenter.LogLevel);
        }

        /// <summary>
        /// Verify that configuration does not overwrite a previously set log level
        /// </summary>
        [TestMethod]
        public void SetLogLevelBeforeConfigure()
        {
            MobileCenterLog.Level = LogLevel.Info;
            MobileCenter.LogLevel = LogLevel.Assert;
            MobileCenter.Configure("appsecret");

            Assert.AreEqual(LogLevel.Assert, MobileCenter.LogLevel);
        }

        /// <summary>
        /// Verify that the default log level is warn
        /// </summary>
        [TestMethod]
        public void DefaultLogLevel()
        {
            MobileCenterLog.Level = LogLevel.Info;
            MobileCenter.Configure("appsecret");

            Assert.AreEqual(LogLevel.Warn, MobileCenter.LogLevel);
        }

        /// <summary>
        /// Verify that starting the same service twice (separately) only calls its OnChannelGroupReady
        /// </summary>
        [TestMethod]
        public void StartInstanceTwiceSeparately()
        {
            MobileCenter.Configure("appsecret");
            MobileCenter.Start(typeof(MockMobileCenterService));
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<ChannelGroup>()), Times.Once());
        }

        /// <summary>
        /// Verify that starting the same service twice together only calls its OnChannelGroupReady
        /// </summary>
        [TestMethod]
        public void StartInstanceTwiceTogether()
        {
            MobileCenter.Configure("appsecret");
            MobileCenter.Start(typeof(MockMobileCenterService), typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<ChannelGroup>()), Times.Once());
        }

        [TestMethod]
        public void StartNullServices()
        {
            MobileCenter.Start("appsecret", null);
        }

        /// <summary>
        /// Verify that the value of enabled actually comes from the application settings.
        /// </summary>
        [TestMethod]
        public void GetEnabled()
        {
            var settingsMock = new Mock<IApplicationSettings>();
            MobileCenter.ApplicationSettings = settingsMock.Object;
            settingsMock.SetupSequence(settings => settings.GetValue(MobileCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true).Returns(false);

            Assert.IsTrue(MobileCenter.Enabled);
            Assert.IsFalse(MobileCenter.Enabled);
            settingsMock.Verify(settings => settings.GetValue(MobileCenter.EnabledKey, It.IsAny<bool>()), Times.Exactly(2));
        }

        /// <summary>
        /// Verify that setting Enabled to its existing value does nothing
        /// </summary>
        [TestMethod]
        public void SetEnabledSameValue()
        {
            MobileCenter.Start("appsecret", typeof(MockMobileCenterService));
            var settingsMock = new Mock<IApplicationSettings>();
            MobileCenter.ApplicationSettings = settingsMock.Object;
            MobileCenter.Enabled = MobileCenter.Enabled;

            MockMobileCenterService.Instance.MockInstance.VerifySet(service => service.InstanceEnabled = It.IsAny<bool>(), Times.Never());
            settingsMock.VerifySet(settings => settings[MobileCenter.EnabledKey] = It.IsAny<bool>(), Times.Never());
        }

        /// <summary>
        /// Verify that setting Enabled to its existing value propagates the change
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValue()
        {
            MobileCenter.Start("appsecret", typeof(MockMobileCenterService));
            var settingsMock = new Mock<IApplicationSettings>();
            MobileCenter.ApplicationSettings = settingsMock.Object;
            var setVal = !MobileCenter.Enabled;
            MobileCenter.Enabled = setVal;

            //TODO should this also verify that channel group is enabled/disabled as appropriate?
            MockMobileCenterService.Instance.MockInstance.VerifySet(service => service.InstanceEnabled = setVal, Times.Once());
            settingsMock.VerifySet(settings => settings[MobileCenter.EnabledKey] = setVal, Times.Once());
        }

        /// <summary>
        /// Verify that the "configured" property is accurate
        /// </summary>
        [TestMethod]
        public void GetConfigured()
        {
            var isConfiguredFirst = MobileCenter.Configured;
            MobileCenter.Configure("some string");

            Assert.IsFalse(isConfiguredFirst);
            Assert.IsTrue(MobileCenter.Configured);
        }

        /// <summary>
        /// Verify that install id comes from settings and is not null
        /// </summary>
        [TestMethod]
        public void GetInstallId()
        {
            var settingsMock = new Mock<IApplicationSettings>();
            MobileCenter.ApplicationSettings = settingsMock.Object;
            settingsMock.Setup(settings => settings.GetValue(MobileCenter.InstallIdKey, It.IsAny<Guid>()))
                .Returns(Guid.NewGuid());
            var installId = MobileCenter.InstallId;

            settingsMock.Verify(settings => settings.GetValue(MobileCenter.InstallIdKey, It.IsAny<Guid>()), Times.Once());
            Assert.IsTrue(installId.HasValue);
        }

        /// <summary>
        /// Verify that starting a service without configuring MobileCenter does not call its OnChannelGroupReady method
        /// </summary>
        [TestMethod]
        public void StartServiceWithoutConfigure()
        {
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(service => service.OnChannelGroupReady(It.IsAny<ChannelGroup>()), Times.Never());
        }

        /// <summary>
        /// Verify that starting a service calls its OnChannelGroupReady method
        /// </summary>
        [TestMethod]
        public void StartInstanceWithConfigure()
        {
            MobileCenter.Configure("appsecret");
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(service => service.OnChannelGroupReady(It.IsAny<ChannelGroup>()), Times.Once());
        }

        /// <summary>
        /// Verify that starting faulty services does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartFaultyAndCorrectServices()
        {
            MobileCenter.Start("app secret", typeof(string), typeof(BadMobileCenterService), null, typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(service => service.OnChannelGroupReady(It.IsAny<ChannelGroup>()), Times.Once());
        }

        /// <summary>
        /// Verify that starting a Mobile Center instance with a null app secret does not cause Mobile Centerto be configured
        /// </summary>
        [TestMethod]
        public void ConfigureWithNullAppSecret()
        {
            MobileCenter.Configure(null);
            Assert.IsFalse(MobileCenter.Configured);
        }

        /// <summary>
        /// Verify that starting a Mobile Center instance with an empty app secret does not cause Mobile Center to be configured
        /// </summary>
        [TestMethod]
        public void ConfigureWithEmptyAppSecret()
        {
            MobileCenter.Configure(null);
            Assert.IsFalse(MobileCenter.Configured);
        }

        /// <summary>
        /// Verify that configuring a Mobile Center instance multiple times throws an error
        /// </summary>
        [TestMethod]
        public void ConfigureMobileCenterMultipleTimes()
        {
            MobileCenter.Instance.InstanceConfigure("appsecret");
            Assert.ThrowsException<MobileCenterException>(() => MobileCenter.Instance.InstanceConfigure("appsecret"));
        }

        private static void VerifySetLogLevel(LogLevel level)
        {
            MobileCenter.LogLevel = level;
            Assert.AreEqual(MobileCenter.LogLevel, level);
        }
    }

    public class BadMobileCenterService : IMobileCenterService
    {
        public static IMobileCenterService Instance => null;
        public string ServiceName => nameof(BadMobileCenterService);

        public bool InstanceEnabled { get; set; }

        public void OnChannelGroupReady(ChannelGroup channelGroup)
        {
        }
    }
}
