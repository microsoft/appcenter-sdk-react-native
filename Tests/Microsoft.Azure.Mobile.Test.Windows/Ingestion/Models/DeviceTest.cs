using Microsoft.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Models
{
    using Device = Microsoft.Azure.Mobile.Ingestion.Models.Device;

    [TestClass]
    public class DeviceTest
    {
        private const string sdkName        = "sdkName";
        private const string sdkVersion     = "sdkVersion";
        private const string model          = "model";
        private const string oemName        = "oemName";
        private const string osName         = "osName";
        private const string osVersion      = "osVersion";
        private const string locale         = "locale";
        private const string screenSize     = "screenSize";
        private const string appVersion     = "appVersion";
        private const string appBuild       = "appBuild";
        private const int timeZoneOffset    = 0;

        /// <summary>
        /// Verify that instance is constructed properly.
        /// </summary>
        [TestMethod]
        public void TestInstanceConstruction()
        {
            int? deafultInt = default(int?);
            const string deafultString = default(string);
            Device emptyDevice = new Device();
            Device device = new Device(sdkName, sdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);

            Assert.IsNotNull(emptyDevice);
            Assert.IsNotNull(device);
       
            Assert.AreEqual<string>(sdkName, device.SdkName);
            Assert.AreEqual<string>(sdkVersion, device.SdkVersion);
            Assert.AreEqual<string>(model, device.Model);
            Assert.AreEqual<string>(oemName, device.OemName);
            Assert.AreEqual<string>(osName, device.OsName);
            Assert.AreEqual<string>(osVersion, device.OsVersion);
            Assert.AreEqual<string>(locale, device.Locale);
            Assert.AreEqual<string>(screenSize, device.ScreenSize);
            Assert.AreEqual<string>(appVersion, device.AppVersion);
            Assert.AreEqual<string>(appBuild, device.AppBuild);
            Assert.AreEqual<string>(deafultString, device.WrapperSdkVersion);
            Assert.AreEqual<string>(deafultString, device.WrapperSdkName);
            Assert.AreEqual<string>(deafultString, device.OsBuild);
            Assert.AreEqual<string>(deafultString, device.CarrierName);
            Assert.AreEqual<string>(deafultString, device.CarrierCountry);
            Assert.AreEqual<string>(deafultString, device.AppNamespace);
            Assert.AreEqual<string>(deafultString, device.LiveUpdateReleaseLabel);
            Assert.AreEqual<string>(deafultString, device.LiveUpdateDeploymentKey);
            Assert.AreEqual<string>(deafultString, device.LiveUpdatePackageHash);
            
            Assert.AreEqual<int>(timeZoneOffset, device.TimeZoneOffset);
            Assert.AreEqual<int?>(deafultInt, device.OsApiLevel);
        }

        /// <summary>
        /// Verify that Validate method does not throw exception if all required fields are present.
        /// </summary>
        [TestMethod]
        public void TestValidateSuccessfullWhenAllFieldsArePresent()
        {
            Device device = new Device(sdkName, sdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
            device.Validate();
            Assert.IsTrue(true);
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when SdkName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenSdkNameIsNull()
        {
            const string nullSdkName = null;
            Device device = new Device(nullSdkName, sdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when SdkVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenSdkVersionIsNull()
        {
            const string nullSdkVersion = null;
            Device device = new Device(sdkName, nullSdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when model == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenModelIsNull()
        {
            const string nullModel = null;
            Device device = new Device(sdkName, sdkVersion, nullModel, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when oemName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOemNameIsNull()
        {
            const string nullOemName = null;
            Device device = new Device(sdkName, sdkVersion, model, nullOemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when osName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOsNameIsNull()
        {
            const string nullOsName = null;
            Device device = new Device(sdkName, sdkVersion, model, oemName, nullOsName, osVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when osVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOsVersionIsNull()
        {
            const string nullOsVersion = null;
            Device device = new Device(sdkName, sdkVersion, model, oemName, osName, nullOsVersion, locale, timeZoneOffset, screenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when locale == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenLocaleIsNull()
        {
            const string nullLocale = null;
            Device device = new Device(sdkName, sdkVersion, model, oemName, osName, osVersion, nullLocale, timeZoneOffset, screenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when screenSize == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenScreenSizeIsNull()
        {
            const string nullScreenSize = null;
            Device device = new Device(sdkName, sdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, nullScreenSize, appVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when appVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenAppVersionIsNull()
        {
            const string nullAppVersion = null;
            Device device = new Device(sdkName, sdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, nullAppVersion, appBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when appBuild == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenAppBuildnIsNull()
        {
            const string nullAppBuild = null;
            Device device = new Device(sdkName, sdkVersion, model, oemName, osName, osVersion, locale, timeZoneOffset, screenSize, appVersion, nullAppBuild);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }
    }
}
