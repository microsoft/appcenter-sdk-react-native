// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
            var device = new Device
            {
                SdkName = SdkName,
                SdkVersion = SdkVersion,
                OsName = OsName,
                OsVersion = OsVersion,
                Locale = Locale,
                TimeZoneOffset = TimeZoneOffset,
                AppVersion = AppVersion,
                AppBuild = AppBuild,
                Model = Model,
                ScreenSize = ScreenSize,
                OemName = OemName
            };
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
            var device = new Device
            {
                SdkName = SdkName,
                SdkVersion = SdkVersion,
                OsName = OsName,
                OsVersion = OsVersion,
                Locale = Locale,
                TimeZoneOffset = TimeZoneOffset,
                AppVersion = AppVersion,
                AppBuild = AppBuild,
                Model = Model,
                ScreenSize = ScreenSize,
                OemName = OemName
            };
            device.Validate();
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when SdkName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenSdkNameIsNull()
        {
            var device = new Device(null, SdkVersion, OsName, OsVersion, Locale, TimeZoneOffset, AppVersion, AppBuild, null, null, Model);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when SdkVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenSdkVersionIsNull()
        {
            var device = new Device
            {
                SdkName = SdkName,
                SdkVersion = null,
                OsName = OsName,
                OsVersion = OsVersion,
                Locale = Locale,
                TimeZoneOffset = TimeZoneOffset,
                AppVersion = AppVersion,
                AppBuild = AppBuild,
                Model = Model,
                ScreenSize = ScreenSize,
                OemName = OemName
            };
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when osName == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOsNameIsNull()
        {
            var device = new Device(SdkName, SdkVersion, null, OsVersion, Locale, TimeZoneOffset, AppVersion, AppBuild, null, null, Model, OemName);
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when osVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenOsVersionIsNull()
        {
            var device = new Device
            {
                SdkName = SdkName,
                SdkVersion = SdkVersion,
                OsName = OsName,
                OsVersion = null,
                Locale = Locale,
                TimeZoneOffset = TimeZoneOffset,
                AppVersion = AppVersion,
                AppBuild = AppBuild,
                Model = Model,
                ScreenSize = ScreenSize,
                OemName = OemName
            };
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when locale == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenLocaleIsNull()
        {
            var device = new Device
            {
                SdkName = SdkName,
                SdkVersion = SdkVersion,
                OsName = OsName,
                OsVersion = OsVersion,
                Locale = null,
                TimeZoneOffset = TimeZoneOffset,
                AppVersion = AppVersion,
                AppBuild = AppBuild,
                Model = Model,
                ScreenSize = ScreenSize,
                OemName = OemName
            };
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when appVersion == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenAppVersionIsNull()
        {
            var device = new Device
            {
                SdkName = SdkName,
                SdkVersion = SdkVersion,
                OsName = OsName,
                OsVersion = OsVersion,
                Locale = Locale,
                TimeZoneOffset = TimeZoneOffset,
                AppVersion = null,
                AppBuild = AppBuild,
                Model = Model,
                ScreenSize = ScreenSize,
                OemName = OemName
            };
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }

        /// <summary>
        /// Verify that Validate method throws ValidationException when appBuild == null.
        /// </summary>
        [TestMethod]
        public void TestValidateThrowsExceptionWhenAppBuildnIsNull()
        {
            var device = new Device
            {
                SdkName = SdkName,
                SdkVersion = SdkVersion,
                OsName = OsName,
                OsVersion = OsVersion,
                Locale = Locale,
                TimeZoneOffset = TimeZoneOffset,
                AppVersion = AppVersion,
                AppBuild = null,
                Model = Model,
                ScreenSize = ScreenSize,
                OemName = OemName
            };
            Assert.ThrowsException<ValidationException>(() => device.Validate());
        }
    }
}
