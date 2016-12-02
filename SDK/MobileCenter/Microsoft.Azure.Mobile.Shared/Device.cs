namespace Microsoft.Azure.Mobile
{
    /// <summary>
    /// Device class to help retrieve device information.
    /// </summary>
    public partial class Device
    {
        /// <summary>
        /// Gets the name of the SDK.
        /// </summary>
        /// <value>Name of the SDK. Consists of the name of the SDK and the platform, e.g. "mobilecenter.ios", "mobilecenter.android"</value>
        public string SdkName { get; }

        /// <summary>
        /// Gets the SDK version.
        /// </summary>
        /// <value>Version of the SDK in semver format, e.g. "1.2.0" or "0.12.3-alpha.1".</value>
        public string SdkVersion { get; }

        /// <summary>
        /// Gets the device model.
        /// </summary>
        /// <value>Device model (example: iPad2,3).</value>
        public string Model { get; }

        /// <summary>
        /// Gets the name of the manufacturer.
        /// </summary>
        /// <value> Device manufacturer (example: HTC).</value>
        public string OemName { get; }

        /// <summary>
        /// Gets the name of the OS.
        /// </summary>
        /// <value>OS name (example: iOS).</value>
        public string OsName { get; }

        /// <summary>
        /// Gets the OS version.
        /// </summary>
        /// <value>OS version (example: 9.3.0).</value>
        public string OsVersion { get; }

        /// <summary>
        /// Gets the OS build
        /// </summary>
        /// <value>OS build code (example: LMY47X).</value>
        public string OsBuild { get; }

        /// <summary>
        /// Gets the OS API level.
        /// </summary>
        /// <value>API level when applicable like in Android (example: 15).</value>
        public int OsApiLevel { get; }

        /// <summary>
        /// Gets the locale.
        /// </summary>
        /// <value>Language code (example: en_US).</value>
        public string Locale { get; }

        /// <summary>
        /// Gets the time zone offset.
        /// </summary>
        /// <value>The offset in minutes from UTC for the device time zone, including daylight savings time.</value>
        public int TimeZoneOffset { get; }

        /// <summary>
        /// Gets the size of the screen.
        /// </summary>
        /// <value>Screen size of the device in pixels (example: 640x480).</value>
        public string ScreenSize { get; }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        /// <value>Application version name, e.g. 1.1.0</value>
        public string AppVersion { get; }

        /// <summary>
        /// Gets the name of the carrier.
        /// </summary>
        /// <value>Carrier name (for mobile devices).</value>
        public string CarrierName { get; }

        /// <summary>
        /// Gets the carrier country.
        /// </summary>
        /// <value>Carrier country code (for mobile devices).</value>
        public string CarrierCountry { get; }

        /// <summary>
        /// Gets the app build.
        /// </summary>
        /// <value>The app's build number, e.g. 42.</value>
        public string AppBuild { get; }

        /// <summary>
        /// Gets the app namespace.
        /// </summary>
        /// <value>The bundle identifier, package identifier, or namespace, depending on what the individual platforms
        /// use, e.g.com.microsoft.example.</value>
        public string AppNamespace { get; }
    }
}
