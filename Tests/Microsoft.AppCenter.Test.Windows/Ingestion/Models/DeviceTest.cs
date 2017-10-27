using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.Ingestion.Models
{
    using Device = Microsoft.AppCenter.Ingestion.Models.Device;

    [TestClass]
    public class DeviceTest
    {
        private const string SdkName        = "sdkName";
        private const string SdkVersion     = "sdkVersion";
        private const string Model          = "model";
        private const string OemName        = "oemName";
        private const string OsName         = "osName";
        private const string OsVersion      = "osVersion";
        private const string Locale         = "locale";
        private const string ScreenSize     = "screenSize";
        private const string AppVersion     = "appVersion";
        private const string AppBuild       = "appBuild";
        private const int TimeZoneOffset    = 0;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            const string deafultString = default(string);
            var emptyDevice = new Device();
            var device = new Device(SdkName, SdkVersion, Model, OemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);

            Assert.IsNotNull(emptyDevice);
            Assert.IsNotNull(device);
       
            Assert.AreEqual(SdkName, device.SdkName);
            Assert.AreEqual(SdkVersion, device.SdkVersion);
            Assert.AreEqual(Model, device.Model);
            Assert.AreEqual(OemName, device.OemName);
            Assert.AreEqual(OsName, device.OsName);
            Assert.AreEqual(OsVersion, device.OsVersion);
            Assert.AreEqual(Locale, device.Locale);
            Assert.AreEqual(ScreenSize, device.ScreenSize);
            Assert.AreEqual(AppVersion, device.AppVersion);
            Assert.AreEqual(AppBuild, device.AppBuild);
            Assert.AreEqual(deafultString, device.WrapperSdkVersion);
            Assert.AreEqual(deafultString, device.WrapperSdkName);
            Assert.AreEqual(deafultString, device.OsBuild);
            Assert.AreEqual(deafultString, device.CarrierName);
            Assert.AreEqual(deafultString, device.CarrierCountry);
            Assert.AreEqual(deafultString, device.AppNamespace);
            Assert.AreEqual(deafultString, device.LiveUpdateReleaseLabel);
            Assert.AreEqual(deafultString, device.LiveUpdateDeploymentKey);
            Assert.AreEqual(deafultString, device.LiveUpdatePackageHash);
            
            Assert.AreEqual(TimeZoneOffset, device.TimeZoneOffset);
            Assert.AreEqual(default(int?), device.OsApiLevel);
        }

        /// <summary>
        /// Verify that Validate method does not throw exception if all required fields are present.
        /// </summary>
        [TestMethod]
        public void TestValidateSuccessfullWhenAllFieldsArePresent()
        {
            var device = new Device(SdkName, SdkVersion, Model, OemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);

            device.Validate();
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when SdkName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenSdkNameIsNull()
        {
            const string nullSdkName = null;
            var device = new Device(nullSdkName, SdkVersion, Model, OemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when SdkVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenSdkVersionIsNull()
        {
            const string nullSdkVersion = null;
            var device = new Device(SdkName, nullSdkVersion, Model, OemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when model == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenModelIsNull()
        {
            const string nullModel = null;
            var device = new Device(SdkName, SdkVersion, nullModel, OemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when oemName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOemNameIsNull()
        {
            const string nullOemName = null;
            var device = new Device(SdkName, SdkVersion, Model, nullOemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when osName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOsNameIsNull()
        {
            const string nullOsName = null;
            var device = new Device(SdkName, SdkVersion, Model, OemName, nullOsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when osVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOsVersionIsNull()
        {
            const string nullOsVersion = null;
            var device = new Device(SdkName, SdkVersion, Model, OemName, OsName, nullOsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when locale == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenLocaleIsNull()
        {
            const string nullLocale = null;
            var device = new Device(SdkName, SdkVersion, Model, OemName, OsName, OsVersion, nullLocale, TimeZoneOffset, ScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when screenSize == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenScreenSizeIsNull()
        {
            const string nullScreenSize = null;
            var device = new Device(SdkName, SdkVersion, Model, OemName, OsName, OsVersion, Locale, TimeZoneOffset, nullScreenSize, AppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when appVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenAppVersionIsNull()
        {
            const string nullAppVersion = null;
            var device = new Device(SdkName, SdkVersion, Model, OemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, nullAppVersion, AppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when appBuild == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenAppBuildnIsNull()
        {
            const string nullAppBuild = null;
            var device = new Device(SdkName, SdkVersion, Model, OemName, OsName, OsVersion, Locale, TimeZoneOffset, ScreenSize, AppVersion, nullAppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }
    }
}
