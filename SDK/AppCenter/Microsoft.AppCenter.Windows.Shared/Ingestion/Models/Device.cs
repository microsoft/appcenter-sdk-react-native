// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Ingestion.Models
{
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Ingestion;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Device characteristics.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Initializes a new instance of the Device class.
        /// </summary>
        public Device() { }

        /// <summary>
        /// Initializes a new instance of the Device class.
        /// </summary>
        /// <param name="sdkName">Name of the SDK. Consists of the name of the
        /// SDK and the platform, e.g. "mobilecenter.ios", "hockeysdk.android".
        /// </param>
        /// <param name="sdkVersion">Version of the SDK in semver format, e.g.
        /// "1.2.0" or "0.12.3-alpha.1".
        /// </param>
        /// <param name="osName">OS name (example: iOS). The following OS names
        /// are standardized (non-exclusive): Android, iOS, macOS, tvOS,
        /// Windows.
        /// </param>
        /// <param name="osVersion">OS version (example: 9.3.0).
        /// </param>
        /// <param name="locale">Language code (example: en-US).
        /// </param>
        /// <param name="timeZoneOffset">The offset in minutes from UTC for the
        /// device time zone, including daylight savings time.
        /// </param>
        /// <param name="appVersion">Application version name, e.g. 1.1.0
        /// </param>
        /// <param name="appBuild">The app's build number, e.g. 42.
        /// </param>
        /// <param name="wrapperSdkVersion">Version of the wrapper SDK in
        /// semver format. When the SDK is embedding another base SDK (for
        /// example Xamarin.Android wraps Android), the Xamarin specific
        /// version is populated into this field while sdkVersion refers to the
        /// original Android SDK.
        /// </param>
        /// <param name="wrapperSdkName">Name of the wrapper SDK. Consists of
        /// the name of the SDK and the wrapper platform, e.g.
        /// "mobilecenter.xamarin", "hockeysdk.cordova".
        /// </param>
        /// <param name="model">Device model (example: iPad2,3).
        /// </param>
        /// <param name="oemName">Device manufacturer (example: HTC).
        /// </param>
        /// <param name="osBuild">OS build code (example: LMY47X).
        /// </param>
        /// <param name="osApiLevel">API level when applicable like in Android
        /// (example: 15).
        /// </param>
        /// <param name="screenSize">Screen size of the device in pixels
        /// (example: 640x480).
        /// </param>
        /// <param name="carrierName">Carrier name (for mobile devices).
        /// </param>
        /// <param name="carrierCountry">Carrier country code (for mobile
        /// devices).
        /// </param>
        /// <param name="appNamespace">The bundle identifier, package
        /// identifier, or namespace, depending on what the individual
        /// plattforms use,  .e.g com.microsoft.example.
        /// </param>
        /// <param name="liveUpdateReleaseLabel">Label that is used to identify
        /// application code 'version' released via Live Update beacon running
        /// on device
        /// </param>
        /// <param name="liveUpdateDeploymentKey">Identifier of environment
        /// that current application release belongs to, deployment key then
        /// maps to environment like Production, Staging.
        /// </param>
        /// <param name="liveUpdatePackageHash">Hash of all files (ReactNative
        /// or Cordova) deployed to device via LiveUpdate beacon. Helps
        /// identify the Release version on device or need to download updates
        /// in future.
        /// </param>
        /// <param name="wrapperRuntimeVersion">Version of the wrapper
        /// technology framework (Xamarin runtime version or ReactNative or
        /// Cordova etc...). See wrappersdkname to see if this version refers
        /// to Xamarin or ReactNative or other.
        /// </param>
        public Device(string sdkName, string sdkVersion, string osName, string osVersion, string locale, int timeZoneOffset, string appVersion, string appBuild, string wrapperSdkVersion = default(string), string wrapperSdkName = default(string), string model = default(string), string oemName = default(string), string osBuild = default(string), int? osApiLevel = default(int?), string screenSize = default(string), string carrierName = default(string), string carrierCountry = default(string), string appNamespace = default(string), string liveUpdateReleaseLabel = default(string), string liveUpdateDeploymentKey = default(string), string liveUpdatePackageHash = default(string), string wrapperRuntimeVersion = default(string))
        {
            SdkName = sdkName;
            SdkVersion = sdkVersion;
            WrapperSdkVersion = wrapperSdkVersion;
            WrapperSdkName = wrapperSdkName;
            Model = model;
            OemName = oemName;
            OsName = osName;
            OsVersion = osVersion;
            OsBuild = osBuild;
            OsApiLevel = osApiLevel;
            Locale = locale;
            TimeZoneOffset = timeZoneOffset;
            ScreenSize = screenSize;
            AppVersion = appVersion;
            CarrierName = carrierName;
            CarrierCountry = carrierCountry;
            AppBuild = appBuild;
            AppNamespace = appNamespace;
            LiveUpdateReleaseLabel = liveUpdateReleaseLabel;
            LiveUpdateDeploymentKey = liveUpdateDeploymentKey;
            LiveUpdatePackageHash = liveUpdatePackageHash;
            WrapperRuntimeVersion = wrapperRuntimeVersion;
        }

        /// <summary>
        /// Gets or sets name of the SDK. Consists of the name of the SDK and
        /// the platform, e.g. "mobilecenter.ios", "hockeysdk.android".
        ///
        /// </summary>
        [JsonProperty(PropertyName = "sdkName")]
        public string SdkName { get; set; }

        /// <summary>
        /// Gets or sets version of the SDK in semver format, e.g. "1.2.0" or
        /// "0.12.3-alpha.1".
        ///
        /// </summary>
        [JsonProperty(PropertyName = "sdkVersion")]
        public string SdkVersion { get; set; }

        /// <summary>
        /// Gets or sets version of the wrapper SDK in semver format. When the
        /// SDK is embedding another base SDK (for example Xamarin.Android
        /// wraps Android), the Xamarin specific version is populated into this
        /// field while sdkVersion refers to the original Android SDK.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "wrapperSdkVersion")]
        public string WrapperSdkVersion { get; set; }

        /// <summary>
        /// Gets or sets name of the wrapper SDK. Consists of the name of the
        /// SDK and the wrapper platform, e.g. "mobilecenter.xamarin",
        /// "hockeysdk.cordova".
        ///
        /// </summary>
        [JsonProperty(PropertyName = "wrapperSdkName")]
        public string WrapperSdkName { get; set; }

        /// <summary>
        /// Gets or sets device model (example: iPad2,3).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets device manufacturer (example: HTC).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "oemName")]
        public string OemName { get; set; }

        /// <summary>
        /// Gets or sets OS name (example: iOS). The following OS names are
        /// standardized (non-exclusive): Android, iOS, macOS, tvOS, Windows.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "osName")]
        public string OsName { get; set; }

        /// <summary>
        /// Gets or sets OS version (example: 9.3.0).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "osVersion")]
        public string OsVersion { get; set; }

        /// <summary>
        /// Gets or sets OS build code (example: LMY47X).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "osBuild")]
        public string OsBuild { get; set; }

        /// <summary>
        /// Gets or sets API level when applicable like in Android (example:
        /// 15).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "osApiLevel")]
        public int? OsApiLevel { get; set; }

        /// <summary>
        /// Gets or sets language code (example: en-US).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the offset in minutes from UTC for the device time
        /// zone, including daylight savings time.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "timeZoneOffset")]
        public int TimeZoneOffset { get; set; }

        /// <summary>
        /// Gets or sets screen size of the device in pixels (example:
        /// 640x480).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "screenSize")]
        public string ScreenSize { get; set; }

        /// <summary>
        /// Gets or sets application version name, e.g. 1.1.0
        ///
        /// </summary>
        [JsonProperty(PropertyName = "appVersion")]
        public string AppVersion { get; set; }

        /// <summary>
        /// Gets or sets carrier name (for mobile devices).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "carrierName")]
        public string CarrierName { get; set; }

        /// <summary>
        /// Gets or sets carrier country code (for mobile devices).
        ///
        /// </summary>
        [JsonProperty(PropertyName = "carrierCountry")]
        public string CarrierCountry { get; set; }

        /// <summary>
        /// Gets or sets the app's build number, e.g. 42.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "appBuild")]
        public string AppBuild { get; set; }

        /// <summary>
        /// Gets or sets the bundle identifier, package identifier, or
        /// namespace, depending on what the individual plattforms use,  .e.g
        /// com.microsoft.example.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "appNamespace")]
        public string AppNamespace { get; set; }

        /// <summary>
        /// Gets or sets label that is used to identify application code
        /// 'version' released via Live Update beacon running on device
        ///
        /// </summary>
        [JsonProperty(PropertyName = "liveUpdateReleaseLabel")]
        public string LiveUpdateReleaseLabel { get; set; }

        /// <summary>
        /// Gets or sets identifier of environment that current application
        /// release belongs to, deployment key then maps to environment like
        /// Production, Staging.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "liveUpdateDeploymentKey")]
        public string LiveUpdateDeploymentKey { get; set; }

        /// <summary>
        /// Gets or sets hash of all files (ReactNative or Cordova) deployed to
        /// device via LiveUpdate beacon. Helps identify the Release version on
        /// device or need to download updates in future.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "liveUpdatePackageHash")]
        public string LiveUpdatePackageHash { get; set; }

        /// <summary>
        /// Gets or sets version of the wrapper technology framework (Xamarin
        /// runtime version or ReactNative or Cordova etc...). See
        /// wrappersdkname to see if this version refers to Xamarin or
        /// ReactNative or other.
        ///
        /// </summary>
        [JsonProperty(PropertyName = "wrapperRuntimeVersion")]
        public string WrapperRuntimeVersion { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (SdkName == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(SdkName));
            }
            if (SdkVersion == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(SdkVersion));
            }
            if (OsName == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(OsName));
            }
            if (OsVersion == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(OsVersion));
            }
            if (Locale == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(Locale));
            }
            if (AppVersion == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(AppVersion));
            }
            if (AppBuild == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, nameof(AppBuild));
            }
            if (TimeZoneOffset > 840)
            {
                throw new ValidationException(ValidationException.Rule.InclusiveMaximum, nameof(TimeZoneOffset), 840);
            }
            if (TimeZoneOffset < -840)
            {
                throw new ValidationException(ValidationException.Rule.InclusiveMinimum, nameof(TimeZoneOffset), -840);
            }
        }
    }
}

