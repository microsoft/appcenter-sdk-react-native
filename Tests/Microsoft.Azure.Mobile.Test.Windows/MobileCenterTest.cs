using System;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Test.Channel;
using Microsoft.Azure.Mobile.Test.Utils;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test
{
    [TestClass]
    public class MobileCenterTest
    {
        private readonly Mock<IApplicationSettings> _settingsMock = new Mock<IApplicationSettings>();
        private readonly Mock<IChannelGroup> _channelGroupMock = new Mock<IChannelGroup>();

        [TestInitialize]
        public void InitializeMobileCenterTest()
        {
            MockMobileCenterService.Reset();
            MobileCenter.Instance = null;

            // Return non-null channels.
            _channelGroupMock.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(new Mock<IChannelUnit>().Object);

            // Set factories.
#pragma warning disable 612
            MobileCenter.SetApplicationSettingsFactory(new MockApplicationSettingsFactory(_settingsMock));
            MobileCenter.SetChannelGroupFactory(new MockChannelGroupFactory(_channelGroupMock));
#pragma warning restore 612
        }

        [TestCleanup]
        public void CleanupMobileCenterTest()
        {
            MobileCenter.Instance = null;
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
        /// Verify sdk version.
        /// </summary>
        [TestMethod]
        public void VerifySdkVersion()
        {
            Assert.AreEqual(WrapperSdk.Version, MobileCenter.SdkVersion);
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
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
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
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that calling start with a null services array does not cause a crash
        /// </summary>
        [TestMethod]
        public void StartNullServices()
        {
            Type[] services = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            MobileCenter.Start("appsecret", services);

            /* Reaching this point means the test passed! */
        }

        /// <summary>
        /// Verify that the value of enabled actually comes from the application settings.
        /// </summary>
        [TestMethod]
        public void GetEnabled()
        {
            _settingsMock.SetupSequence(settings => settings.GetValue(MobileCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true).Returns(false);

            Assert.IsTrue(MobileCenter.IsEnabledAsync().Result);
            Assert.IsFalse(MobileCenter.IsEnabledAsync().Result);
            _settingsMock.Verify(settings => settings.GetValue(MobileCenter.EnabledKey, It.IsAny<bool>()),
                Times.Exactly(2));
        }

        /// <summary>
        /// Verify that setting Enabled to its existing value does nothing
        /// </summary>
        [TestMethod]
        public void SetEnabledSameValue()
        {
            _channelGroupMock.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(new Mock<IChannelUnit>().Object);
            MobileCenter.Start("appsecret", typeof(MockMobileCenterService));
            MobileCenter.SetEnabledAsync(MobileCenter.IsEnabledAsync().Result).Wait();

            MockMobileCenterService.Instance.MockInstance.VerifySet(
                service => service.InstanceEnabled = It.IsAny<bool>(), Times.Never());
            _settingsMock.Verify(settings => settings.SetValue(MobileCenter.EnabledKey, It.IsAny<bool>()), Times.Never());
            _channelGroupMock.Verify(channelGroup => channelGroup.SetEnabled(It.IsAny<bool>()), Times.Never());
        }

        /// <summary>
        /// Verify that setting Enabled to a different value (after configure is called) propagates the change
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValueAfterConfigure()
        {
            _channelGroupMock.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(new Mock<IChannelUnit>().Object);
            MobileCenter.Start("appsecret", typeof(MockMobileCenterService));
            var setVal = !MobileCenter.IsEnabledAsync().Result;
            MobileCenter.SetEnabledAsync(setVal).Wait();

            MockMobileCenterService.Instance.MockInstance.VerifySet(service => service.InstanceEnabled = setVal,
                Times.Once());
            _settingsMock.Verify(settings => settings.SetValue(MobileCenter.EnabledKey, setVal), Times.Once());
            _channelGroupMock.Verify(channelGroup => channelGroup.SetEnabled(setVal), Times.Once());
        }

        /// <summary>
        /// Verify that setting Enabled to a different value (before configure is called) propagates the change when configure is called
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValueBeforeConfigure()
        {
            _settingsMock.Setup(settings => settings.GetValue(MobileCenter.EnabledKey, true)).Returns(true);
            MobileCenter.SetEnabledAsync(false).Wait();
            MobileCenter.Start("appsecret", typeof(MockMobileCenterService));

            _settingsMock.Verify(settings => settings.SetValue(MobileCenter.EnabledKey, false), Times.Once());
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
            var fakeInstallId = Guid.NewGuid();
            _settingsMock.Setup(settings => settings.GetValue(MobileCenter.InstallIdKey, It.IsAny<Guid>())).Returns(fakeInstallId);
            var installId = MobileCenter.GetInstallIdAsync().Result;

            Assert.IsTrue(installId.HasValue);
            Assert.AreEqual(installId.Value, fakeInstallId);
        }

        /// <summary>
        /// Verify that starting a service without configuring MobileCenter does not call its OnChannelGroupReady method
        /// </summary>
        [TestMethod]
        public void StartServiceWithoutConfigure()
        {
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Verify that starting a service calls its OnChannelGroupReady method
        /// </summary>
        [TestMethod]
        public void StartInstanceWithConfigure()
        {
            MobileCenter.Configure("appsecret");
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start a null service does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartNullServiceAndCorrectService()
        {
            MobileCenter.Start("app secret", null, typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start a service with no static Instance property does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartNoInstanceServiceAndCorrectService()
        {
            MobileCenter.Start("app secret", typeof(string), typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start a service whose static Instance property returns null does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartNullInstanceServiceAndCorrectService()
        {
            MobileCenter.Start("app secret", typeof(NullInstanceMobileCenterService), typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start mobile center with a null app secret causes the service passed not to be started
        /// </summary>
        [TestMethod]
        public void StartInstanceNullSecretAndCorrectService()
        {
            string appSecret = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            MobileCenter.Start(appSecret, typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Verify that trying to start a service whose static Instance property returns an object that is not an IMobileCenterService
        /// does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartWrongInstanceTypeServiceAndCorrectService()
        {
            MobileCenter.Start("app secret", typeof(WrongInstanceTypeMobileCenterService),
                typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
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
            MobileCenter.Configure("");
            Assert.IsFalse(MobileCenter.Configured);
        }

        /// <summary>
        /// Verify that configuring a Mobile Center instance multiple times does not throw an error
        /// </summary>
        [TestMethod]
        public void ConfigureMobileCenterMultipleTimes()
        {
            try
            {
                MobileCenter.Instance.InstanceConfigure("appsecret");
            }
            catch (MobileCenterException)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Verify that the channel group's log url is not set by Mobile Center by default
        /// </summary>
        [TestMethod]
        public void LogUrlIsNotSetByDefault()
        {
            MobileCenter.Configure("appsecret");
            _channelGroupMock.Verify(channelGroup => channelGroup.SetLogUrl(It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Verify that the channel group's log url is set by Mobile Center once configured if its log url had been set beforehand
        /// </summary>
        [TestMethod]
        public void SetLogUrlBeforeConfigure()
        {
            var customLogUrl = "www dot log url dot com";
            MobileCenter.SetLogUrl(customLogUrl);
            MobileCenter.Configure("appsecret");

            _channelGroupMock.Verify(channelGroup => channelGroup.SetLogUrl(customLogUrl), Times.Once());
        }

        /// <summary>
        /// Verify that the channel group's log url is updated by Mobile Center if its log url is set after configuration
        /// </summary>
        [TestMethod]
        public void SetLogUrlAfterConfigure()
        {
            MobileCenter.Configure("appsecret");
            var customLogUrl = "www dot log url dot com";
            MobileCenter.SetLogUrl(customLogUrl);

            _channelGroupMock.Verify(channelGroup => channelGroup.SetLogUrl(customLogUrl), Times.Once());
        }

        private static void VerifySetLogLevel(LogLevel level)
        {
            MobileCenter.LogLevel = level;
            Assert.AreEqual(MobileCenter.LogLevel, level);
        }

        /// <summary>
        /// Verify parse when there is no equals (so the given string is assumed to be the app secret)
        /// </summary>
        [TestMethod]
        public void ParseAppSecretNoEquals()
        {
            var appSecret = Guid.NewGuid().ToString();
            var parsedSecret = MobileCenter.GetSecretForPlatform(appSecret, "uwp");
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify parse when there is only one platform (and no terminating semicolon)
        /// </summary>
        [TestMethod]
        public void ParseAppSecretOnePlatform()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"{platformId}={appSecret}";
            var parsedSecret = MobileCenter.GetSecretForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify parse when there is only one platform and a terminating semicolon
        /// </summary>
        [TestMethod]
        public void ParseAppSecretOnePlatformTerminatingSemicolon()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"{platformId}={appSecret};";
            var parsedSecret = MobileCenter.GetSecretForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify parse when the platform is one of two
        /// </summary>
        [TestMethod]
        public void ParseAppSecretFirstOfTwo()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"{platformId}={appSecret}; ios=anotherstring";
            var parsedSecret = MobileCenter.GetSecretForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify parse when the platform is second of two
        /// </summary>
        [TestMethod]
        public void ParseAppSecretSecondOfTwo()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"ios=anotherstring; {platformId}={appSecret}";
            var parsedSecret = MobileCenter.GetSecretForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify parse when the string has extra semicolons
        /// </summary>
        [TestMethod]
        public void ParseAppSecretExtraSemicolons()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"ios=anotherstring;;;;{platformId}={appSecret};;;;";
            var parsedSecret = MobileCenter.GetSecretForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify that when the parse result of an app secret is the empty string, configuration does not occur
        /// </summary>
        [TestMethod]
        public void ConfigureMobileCenterWithEmptyAppSecretEmptyResult()
        {
            var secrets = $"{MobileCenter.PlatformIdentifier}=";
            MobileCenter.Configure(secrets);

            Assert.IsFalse(MobileCenter.Configured);
        }

        /// <summary>
        /// Verify parse when the platform identifier is wrong
        /// </summary>
        [TestMethod]
        public void ParseAppSecretWrongIdentifier()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"ios=anotherstring;{platformId}={appSecret};";
            Assert.ThrowsException<MobileCenterException>(
                () => MobileCenter.GetSecretForPlatform(secrets, platformId + platformId));
        }

        /// <summary>
        /// Verify setting custom properties.
        /// </summary>
        [TestMethod]
        public void SetCustomProperties()
        {
            _settingsMock.Setup(settings => settings.GetValue(MobileCenter.EnabledKey, true)).Returns(true);
            var channelUnitMock = new Mock<IChannelUnit>();
            _channelGroupMock.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(channelUnitMock.Object);

            // Set before Mobile Center is configured. 
            MobileCenter.SetCustomProperties(new CustomProperties());
            channelUnitMock.Verify(channel => channel.EnqueueAsync(It.IsAny<Log>()), Times.Never());

            MobileCenter.Configure("appsecret");

            // Set null.
            MobileCenter.SetCustomProperties(null);
            channelUnitMock.Verify(channel => channel.EnqueueAsync(It.IsAny<Log>()), Times.Never());

            // Set empty.
            var empty = new CustomProperties();
            MobileCenter.SetCustomProperties(empty);
            channelUnitMock.Verify(channel => channel.EnqueueAsync(It.IsAny<Log>()), Times.Never());

            // Set normal.
            var properties = new CustomProperties();
            properties.Set("test", "test");
            MobileCenter.SetCustomProperties(properties);
            channelUnitMock.Verify(channel => channel.EnqueueAsync(It.Is<CustomPropertiesLog>(log =>
                log.Properties == properties.Properties)), Times.Once());
        }
    }

    public class NullInstanceMobileCenterService : IMobileCenterService
    {
        public static IMobileCenterService Instance => null;
        public string ServiceName => nameof(NullInstanceMobileCenterService);

        public bool InstanceEnabled { get; set; }
        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
        }
    }
    public class WrongInstanceTypeMobileCenterService : IMobileCenterService
    {
        public static Guid Instance => Guid.NewGuid();
        public string ServiceName => nameof(WrongInstanceTypeMobileCenterService);

        public bool InstanceEnabled { get; set; }


        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
        }
    }
}
