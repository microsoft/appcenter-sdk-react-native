// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Test.Channel;
using Microsoft.AppCenter.Test.Utils;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test
{
    [TestClass]
    public class AppCenterTest
    {
        private readonly Mock<IApplicationSettings> _settingsMock = new Mock<IApplicationSettings>();
        private readonly Mock<IChannelGroup> _channelGroupMock = new Mock<IChannelGroup>();
        private readonly Mock<IChannelUnit> _channelMock = new Mock<IChannelUnit>();

        [TestInitialize]
        public void InitializeAppCenterTest()
        {
            MockAppCenterService.Reset();
            AppCenter.Instance = null;

            // Return non-null channels.
            _channelGroupMock.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_channelMock.Object);

            // Set factories.
#pragma warning disable 612
            AppCenter.SetApplicationSettingsFactory(new MockApplicationSettingsFactory(_settingsMock));
            AppCenter.SetChannelGroupFactory(new MockChannelGroupFactory(_channelGroupMock));
#pragma warning restore 612
        }

        [TestCleanup]
        public void CleanupAppCenterTest()
        {
            AppCenter.Instance = null;
        }

        /// <summary>
        /// Verify that AppCenter instance is not initially null
        /// </summary>
        [TestMethod]
        public void GetInstanceNotNull()
        {
            Assert.IsNotNull(AppCenter.Instance);
        }

        /// <summary>
        /// Verify that changing the log level of app center has an effect.
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
        /// Verify that App Center is able to get a log level even though it hasn't set one
        /// </summary>
        [TestMethod]
        public void GetLogLevelBeforeConfigure()
        {
            AppCenterLog.Level = LogLevel.Info;

            Assert.AreEqual(LogLevel.Info, AppCenter.LogLevel);
        }

        /// <summary>
        /// Verify that configuration does not overwrite a previously set log level
        /// </summary>
        [TestMethod]
        public void SetLogLevelBeforeConfigure()
        {
            AppCenterLog.Level = LogLevel.Info;
            AppCenter.LogLevel = LogLevel.Assert;
            AppCenter.Configure("appsecret");

            Assert.AreEqual(LogLevel.Assert, AppCenter.LogLevel);
        }

        /// <summary>
        /// Verify sdk version.
        /// </summary>
        [TestMethod]
        public void VerifySdkVersion()
        {
            Assert.AreEqual(WrapperSdk.Version, AppCenter.SdkVersion);
        }

        /// <summary>
        /// Verify country code setter
        /// </summary>
        [TestMethod]
        public void SetCountryCode()
        {
            // Mock event handler.
            var mockInformationInvalidated = new Mock<EventHandler>();

            // Initialize device information helper.
            DeviceInformationHelper.InformationInvalidated += mockInformationInvalidated.Object;
            var deviceInformationHelper = new DeviceInformationHelper();
            var device = deviceInformationHelper.GetDeviceInformationAsync().RunNotAsync();
            Assert.IsNull(device.CarrierCountry);

            // Valid country code.
            var validCountryCode = "US";
            AppCenter.SetCountryCode(validCountryCode);
            device = deviceInformationHelper.GetDeviceInformationAsync().RunNotAsync();
            Assert.AreEqual(device.CarrierCountry, validCountryCode);
            mockInformationInvalidated.Verify(_ => _(It.IsAny<object>(), It.IsAny<EventArgs>()), Times.Once);

            // Invalid country code.
            var invalidCountryCode = "US1";
            AppCenter.SetCountryCode(invalidCountryCode);
            device = deviceInformationHelper.GetDeviceInformationAsync().RunNotAsync();

            // The code has not been updated and the event has not been called.
            Assert.AreEqual(device.CarrierCountry, validCountryCode);
            mockInformationInvalidated.Verify(_ => _(It.IsAny<object>(), It.IsAny<EventArgs>()), Times.Once);

            // Reset country code.
            AppCenter.SetCountryCode(null);
            device = deviceInformationHelper.GetDeviceInformationAsync().RunNotAsync();
            Assert.IsNull(device.CarrierCountry);
            mockInformationInvalidated.Verify(_ => _(It.IsAny<object>(), It.IsAny<EventArgs>()), Times.Exactly(2));

            // Clean.
            DeviceInformationHelper.InformationInvalidated -= mockInformationInvalidated.Object;
        }

        /// <summary>
        /// Verify that starting the same service twice (separately) only calls its OnChannelGroupReady
        /// </summary>
        [TestMethod]
        public void StartInstanceTwiceSeparately()
        {
            AppCenter.Configure("appsecret");
            AppCenter.Start(typeof(MockAppCenterService));
            AppCenter.Start(typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that starting the same service twice together only calls its OnChannelGroupReady
        /// </summary>
        [TestMethod]
        public void StartInstanceTwiceTogether()
        {
            AppCenter.Configure("appsecret");
            AppCenter.Start(typeof(MockAppCenterService), typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
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
            AppCenter.Start("appsecret", services);

            /* Reaching this point means the test passed! */
        }

        /// <summary>
        /// Verify that the value of enabled actually comes from the application settings.
        /// </summary>
        [TestMethod]
        public void GetEnabled()
        {
            AppCenter.Configure("appsecret");

            _settingsMock.ResetCalls();
            _settingsMock.SetupSequence(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true).Returns(false);

            Assert.IsTrue(AppCenter.IsEnabledAsync().Result);
            Assert.IsFalse(AppCenter.IsEnabledAsync().Result);
            _settingsMock.Verify(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()),
                Times.Exactly(2));
        }

        /// <summary>
        /// Verify that setting Enabled to its existing value does nothing
        /// </summary>
        [TestMethod]
        public void SetEnabledSameValue()
        {
            AppCenter.Start("appsecret", typeof(MockAppCenterService));
            AppCenter.SetEnabledAsync(AppCenter.IsEnabledAsync().Result).RunNotAsync();

            MockAppCenterService.Mock.VerifySet(service => service.InstanceEnabled = It.IsAny<bool>(), Times.Never());
            _settingsMock.Verify(settings => settings.SetValue(AppCenter.EnabledKey, It.IsAny<bool>()), Times.Never());
            _channelGroupMock.Verify(channelGroup => channelGroup.SetEnabled(It.IsAny<bool>()), Times.Never());
        }

        /// <summary>
        /// Verify that setting Enabled to a different value (after configure is called) propagates the change
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValueAfterConfigure()
        {
            AppCenter.Start("appsecret", typeof(MockAppCenterService));
            var setVal = !AppCenter.IsEnabledAsync().Result;
            AppCenter.SetEnabledAsync(setVal).RunNotAsync();

            MockAppCenterService.Mock.VerifySet(service => service.InstanceEnabled = setVal, Times.Once());
            _settingsMock.Verify(settings => settings.SetValue(AppCenter.EnabledKey, setVal), Times.Once());
            _channelGroupMock.Verify(channelGroup => channelGroup.SetEnabled(setVal), Times.Once());
        }

        /// <summary>
        /// Verify that setting Enabled to a different value (before configure is called) propagates the change
        /// </summary>
        [TestMethod]
        public void SetEnabledDifferentValueBeforeConfigure()
        {
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true);
            AppCenter.SetEnabledAsync(false).RunNotAsync();
            AppCenter.Start("appsecret", typeof(MockAppCenterService));

            _settingsMock.Verify(settings => settings.SetValue(AppCenter.EnabledKey, false), Times.Once());
        }

        /// <summary>
        /// Verify that install id comes from settings and is not null
        /// </summary>
        [TestMethod]
        public void GetExistingInstallId()
        {
            AppCenter.Configure("appsecret");

            var fakeInstallId = Guid.NewGuid();
            _settingsMock.ResetCalls();
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.InstallIdKey, default(Guid?))).Returns(fakeInstallId);

            var installId = AppCenter.GetInstallIdAsync().Result;

            Assert.IsTrue(installId.HasValue);
            Assert.AreEqual(installId.Value, fakeInstallId);
            _settingsMock.Verify(settings => settings.GetValue(AppCenter.InstallIdKey, default(Guid?)),
                Times.Once());
        }

        /// <summary>
        /// Verify that install id is generated and saved if not existing.
        /// </summary>
        [TestMethod]
        public void GetNewInstallId()
        {
            AppCenter.Configure("appsecret");

            _settingsMock.ResetCalls();

            var installId = AppCenter.GetInstallIdAsync().Result;

            Assert.IsTrue(installId.HasValue);
            Assert.IsNotNull(installId.Value);
            _settingsMock.Verify(settings => settings.SetValue(AppCenter.InstallIdKey, It.IsNotNull<Guid?>()), Times.Once());
        }

        /// <summary>
        /// Verify starting disabled AppCenter
        /// </summary>
        [TestMethod]
        public void StartDisabled()
        {
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(false);

            AppCenter.Start("appsecret", typeof(MockAppCenterService));

            Assert.IsTrue(AppCenter.Configured);
            _channelMock.Verify(channel => channel.SetEnabled(false), Times.Once());
        }

        /// <summary>
        /// Verify that starting a service without configuring App Center does not call its OnChannelGroupReady method
        /// </summary>
        [TestMethod]
        public void StartServiceWithoutConfigure()
        {
            AppCenter.Start(typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Verify that starting a service calls its OnChannelGroupReady method
        /// </summary>
        [TestMethod]
        public void StartInstanceWithConfigure()
        {
            AppCenter.Configure("appsecret");
            AppCenter.Start(typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start a null service does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartNullServiceAndCorrectService()
        {
            AppCenter.Start("app secret", null, typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start a service with no static Instance property does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartNoInstanceServiceAndCorrectService()
        {
            AppCenter.Start("app secret", typeof(string), typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start a service whose static Instance property returns null does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartNullInstanceServiceAndCorrectService()
        {
            AppCenter.Start("app secret", typeof(NullInstanceAppCenterService), typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that trying to start app center with a null app secret causes the service passed not to be started
        /// </summary>
        [TestMethod]
        public void StartInstanceNullSecretAndCorrectService()
        {
            string appSecret = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            AppCenter.Start(appSecret, typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Verify that trying to start a service whose static Instance property returns an object that is not an IAppCenterService
        /// does not prevent other services from starting
        /// </summary>
        [TestMethod]
        public void StartWrongInstanceTypeServiceAndCorrectService()
        {
            AppCenter.Start("app secret", typeof(WrongInstanceTypeAppCenterService),
                typeof(MockAppCenterService));
            MockAppCenterService.Mock.Verify(
                service => service.OnChannelGroupReady(It.IsAny<IChannelGroup>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        /// Verify that configuring a App Center instance
        /// </summary>
        [TestMethod]
        public void Configure()
        {
            Assert.IsFalse(AppCenter.Configured);

            AppCenter.Configure("some string");

            Assert.IsTrue(AppCenter.Configured);
            _channelMock.Verify(channel => channel.SetEnabled(It.IsAny<bool>()));
        }

        /// <summary>
        /// Verify that starting a App Center instance with a null app secret does not cause App Center to be configured
        /// </summary>
        [TestMethod]
        public void ConfigureWithNullAppSecret()
        {
            AppCenter.Configure(null);
            Assert.IsFalse(AppCenter.Configured);
        }

        /// <summary>
        /// Verify that starting a App Center instance with an empty app secret does not cause App Center to be configured
        /// </summary>
        [TestMethod]
        public void ConfigureWithEmptyAppSecret()
        {
            AppCenter.Configure("");
            Assert.IsFalse(AppCenter.Configured);
        }

        /// <summary>
        /// Verify that configuring a App Center instance multiple times does not throw an error
        /// </summary>
        [TestMethod]
        public void ConfigureAppCenterMultipleTimes()
        {
            try
            {
                AppCenter.Instance.InstanceConfigure("appsecret");
            }
            catch (AppCenterException)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Verify that a service will be disabled if App Center is disabled
        /// </summary>
        [TestMethod]
        public void DisableServiceIfAppCenterIsDisabled()
        {
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(false);
            MockAppCenterService.Mock.SetupGet(settings => settings.InstanceEnabled)
                .Returns(true);

            AppCenter.Start("appsecret", typeof(MockAppCenterService));

            MockAppCenterService.Mock.VerifySet(service => service.InstanceEnabled = false, Times.Once);
        }

        /// <summary>
        /// Verify that the channel group's log url is not set by App Center by default
        /// </summary>
        [TestMethod]
        public void LogUrlIsNotSetByDefault()
        {
            AppCenter.Configure("appsecret");
            _channelGroupMock.Verify(channelGroup => channelGroup.SetLogUrl(It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Verify that the channel group's log url is set by App Center once configured if its log url had been set beforehand
        /// </summary>
        [TestMethod]
        public void SetLogUrlBeforeConfigure()
        {
            var customLogUrl = "www dot log url dot com";
            AppCenter.SetLogUrl(customLogUrl);
            AppCenter.Configure("appsecret");

            _channelGroupMock.Verify(channelGroup => channelGroup.SetLogUrl(customLogUrl), Times.Once());
        }

        /// <summary>
        /// Verify that the channel group's log url is updated by App Center if its log url is set after configuration
        /// </summary>
        [TestMethod]
        public void SetLogUrlAfterConfigure()
        {
            AppCenter.Configure("appsecret");
            var customLogUrl = "www dot log url dot com";
            AppCenter.SetLogUrl(customLogUrl);

            _channelGroupMock.Verify(channelGroup => channelGroup.SetLogUrl(customLogUrl), Times.Once());
        }

        private static void VerifySetLogLevel(LogLevel level)
        {
            AppCenter.LogLevel = level;
            Assert.AreEqual(AppCenter.LogLevel, level);
        }

        /// <summary>
        /// Verify parse when there is no equals (so the given string is assumed to be the app secret)
        /// </summary>
        [TestMethod]
        public void ParseAppSecretNoEquals()
        {
            var appSecret = Guid.NewGuid().ToString();
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(appSecret, "uwp");
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
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
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
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify parse when there is only one platform and both app secret and token
        /// </summary>
        [TestMethod]
        public void ParseAppSecretAndTargetOnePlatform()
        {
            var appSecret = Guid.NewGuid().ToString();
            var targetToken = Guid.NewGuid().ToString();
            var platformId = "ios";
            var secrets = $"{platformId}={appSecret};{platformId}Target={targetToken}";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            var expected = $"appsecret={appSecret};target={targetToken}";
            Assert.AreEqual(expected, parsedSecret);
        }

        /// <summary>
        /// Verify throw exception when finding none of the keys.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AppCenterException))]
        public void ThrowWhenFoundNoneOfTheKeys()
        {
            var invalidePlatformIdentifier = "invalidePlatformIdentifier";
            var appSecret = Guid.NewGuid().ToString();
            var targetToken = Guid.NewGuid().ToString();
            var platformId = "ios";
            var secrets = $"{platformId}={appSecret};{platformId}Target={targetToken}";
            AppCenter.GetSecretAndTargetForPlatform(secrets, invalidePlatformIdentifier);
        }

        /// <summary>
        /// Verify throw exception when both keys are empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AppCenterException))]
        public void ThrowWhenBothKeysAreEmpty()
        {
            var appSecret = string.Empty;
            var targetToken = string.Empty;
            var platformId = "ios";
            var secrets = $"{platformId}={appSecret};{platformId}Target={targetToken}";
            AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
        }

        /// <summary>
        /// Verify parse when there are several platforms of both app secret and token.
        /// </summary>
        [TestMethod]
        public void ParseAppSecretAndTargetMultiplePlatform()
        {
            var appSecret = Guid.NewGuid().ToString();
            var anotherAppSecret = Guid.NewGuid().ToString();
            var targetToken = Guid.NewGuid().ToString();
            var platformId = "android";
            var secrets = $"{platformId}={appSecret};ios={anotherAppSecret};{platformId}Target={targetToken};iosTarget={anotherAppSecret}";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            var expected = $"appsecret={appSecret};target={targetToken}";
            Assert.AreEqual(expected, parsedSecret);
        }

        /// <summary>
        /// Verify parse when there is only token.
        /// </summary>
        [TestMethod]
        public void ParseTargetToken()
        {
            var targetToken = Guid.NewGuid().ToString();
            var platformId = "android";
            var secrets = $"{platformId}Target={targetToken};";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            var expected = $"target={targetToken}";
            Assert.AreEqual(expected, parsedSecret);
        }

        /// <summary>
        /// Verify that the invalid target string is not parsed.
        /// </summary>
        [TestMethod]
        public void NotParseTargetString()
        {
            var targetToken = Guid.NewGuid().ToString();
            var secrets = $"target={targetToken};";
            var platformId = "ios";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            Assert.AreEqual(secrets, parsedSecret);
        }

        /// <summary>
        /// Verify parse when the platform is one of two
        /// </summary>
        [TestMethod]
        public void ParseAppSecretFirstOfTwo()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"{platformId}={appSecret};ios=anotherstring";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
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
            var secrets = $"ios=anotherstring;{platformId}={appSecret}";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
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
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify that when the parse result of an app secret is the empty string, configuration does not occur
        /// </summary>
        [TestMethod]
        public void ConfigureAppCenterWithEmptyAppSecretEmptyResult()
        {
            var secrets = $"{AppCenter.PlatformIdentifier}=";
            AppCenter.Configure(secrets);

            Assert.IsFalse(AppCenter.Configured);
        }

        /// <summary>
        /// Verify empty pairs are ignored.
        /// </summary>
        [TestMethod]
        public void ParseValidSecretSurroundedWithInvalidPairs()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"=;{platformId}={appSecret};=";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
        }

        /// <summary>
        /// Verify last value is used on duplicate key.
        /// </summary>
        [TestMethod]
        public void ParseSecretUsesLastValueOnDuplicateKey()
        {
            var appSecret = Guid.NewGuid().ToString();
            var platformId = "uwp";
            var secrets = $"{platformId}={Guid.NewGuid()};{platformId}={appSecret}";
            var parsedSecret = AppCenter.GetSecretAndTargetForPlatform(secrets, platformId);
            Assert.AreEqual(appSecret, parsedSecret);
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
            Assert.ThrowsException<AppCenterException>(
                () => AppCenter.GetSecretAndTargetForPlatform(secrets, platformId + platformId));
        }

        /// <summary>
        /// Verify setting custom properties.
        /// </summary>
        [TestMethod]
        public void SetCustomProperties()
        {
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true);

            // Set before App Center is configured.
            AppCenter.SetCustomProperties(new CustomProperties());
            _channelMock.Verify(channel => channel.EnqueueAsync(It.IsAny<Log>()), Times.Never());

            AppCenter.Configure("appsecret");

            // Set null.
            AppCenter.SetCustomProperties(null);
            _channelMock.Verify(channel => channel.EnqueueAsync(It.IsAny<Log>()), Times.Never());

            // Set empty.
            var empty = new CustomProperties();
            AppCenter.SetCustomProperties(empty);
            _channelMock.Verify(channel => channel.EnqueueAsync(It.IsAny<Log>()), Times.Never());

            // Set normal.
            var properties = new CustomProperties();
            properties.Set("test", "test");
            AppCenter.SetCustomProperties(properties);
            _channelMock.Verify(channel => channel.EnqueueAsync(It.Is<CustomPropertyLog>(log =>
                log.Properties == properties.Properties)), Times.Once());
        }

        /// <summary>
        /// Verify sending start service log
        /// </summary>
        [TestMethod]
        public void SendStartServices()
        {
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(true);

            AppCenter.Start("appsecret", typeof(MockAppCenterService));

            Task.Delay(100).Wait();

            _channelMock.Verify(channel => channel.EnqueueAsync(It.Is<StartServiceLog>(log =>
                log.Services.Count == 1 &&
                log.Services[0] == MockAppCenterService.Instance.ServiceName)), Times.Once());
        }

        /// <summary>
        /// Verify sending start services log after enable
        /// </summary>
        [TestMethod]
        public void SendStartServicesAfterEnable()
        {
            _settingsMock.Setup(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(false);

            AppCenter.Start("appsecret", typeof(MockAppCenterService));
            Task.Delay(100).Wait();
            _channelMock.Verify(channel => channel.EnqueueAsync(It.IsAny<StartServiceLog>()), Times.Never());

            _settingsMock.SetupSequence(settings => settings.GetValue(AppCenter.EnabledKey, It.IsAny<bool>()))
                .Returns(false).Returns(true);
            AppCenter.SetEnabledAsync(true).RunNotAsync();
            _channelMock.Verify(channel => channel.EnqueueAsync(It.Is<StartServiceLog>(log =>
                log.Services.Count == 1 &&
                log.Services[0] == MockAppCenterService.Instance.ServiceName)), Times.Once());
        }

        [TestMethod]
        public void SetWrapperSdk()
        {
            string wrapperName = $"expectedName {Guid.NewGuid()}";
            string wrapperVersion = $"expectedVersion {Guid.NewGuid()}";
            string releaseLabel = $"expectedLabel {Guid.NewGuid()}";
            string updateDevKey = $"expectedUpdateDevKey {Guid.NewGuid()}";
            string updatePackageHash = $"expectedHash {Guid.NewGuid()}";
            string runtimeVersion = $"expectedRuntimeVersion {Guid.NewGuid()}";
            WrapperSdk wrapperSdk = new WrapperSdk(wrapperName, wrapperVersion, runtimeVersion, releaseLabel, updateDevKey, updatePackageHash);
            DeviceInformationHelper.SetWrapperSdk(wrapperSdk);
            var deviceInformationHelper = new DeviceInformationHelper();
            var device = deviceInformationHelper.GetDeviceInformationAsync().RunNotAsync();
            Assert.AreEqual(wrapperName, device.WrapperSdkName);
            Assert.AreEqual(wrapperVersion, device.WrapperSdkVersion);
            Assert.AreEqual(releaseLabel, device.LiveUpdateReleaseLabel);
            Assert.AreEqual(updateDevKey, device.LiveUpdateDeploymentKey);
            Assert.AreEqual(updatePackageHash, device.LiveUpdatePackageHash);
            Assert.AreEqual(runtimeVersion, device.WrapperRuntimeVersion);
        }
    }

    public class NullInstanceAppCenterService : IAppCenterService
    {
        public static IAppCenterService Instance => null;
        public string ServiceName => nameof(NullInstanceAppCenterService);

        public bool InstanceEnabled { get; set; }
        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
        }
    }
    public class WrongInstanceTypeAppCenterService : IAppCenterService
    {
        public static Guid Instance => Guid.NewGuid();
        public string ServiceName => nameof(WrongInstanceTypeAppCenterService);

        public bool InstanceEnabled { get; set; }


        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
        }
    }
}
