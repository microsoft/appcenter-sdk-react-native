using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// SDK core used to initialize, start and control specific service.
    /// </summary>
    public partial class AppCenter
    {
        // Gets the first instance of an app secret corresponding to the given platform name, or returns the string 
        // as-is if no identifier can be found. Logs a message if no identifiers can be found.
        internal static string GetSecretForPlatform(string secrets, string platformIdentifier)
        {
            if (string.IsNullOrEmpty(secrets))
            {
                throw new AppCenterException("App secrets string is null or empty");
            }

            // If there are no equals signs, then there are no named identifiers, but log a message in case the developer made 
            // a typing error.
            if (!secrets.Contains("="))
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
                return secrets;
            }

            var parseErrorMessage = $"Error parsing key for '{platformIdentifier}'";

            var platformIndicator = platformIdentifier + "=";
            var secretIdx = secrets.IndexOf(platformIndicator, StringComparison.Ordinal);
            if (secretIdx == -1)
            {
                throw new AppCenterException(parseErrorMessage);
            }
            secretIdx += platformIndicator.Length;
            var platformSecret = string.Empty;

            while (secretIdx < secrets.Length)
            {
                var nextChar = secrets[secretIdx++];
                if (nextChar == ';')
                {
                    break;
                }

                platformSecret += nextChar;
            }

            if (platformSecret == string.Empty)
            {
                throw new AppCenterException(parseErrorMessage);
            }

            return platformSecret;
        }

        /// <summary>
        ///     This property controls the amount of logs emitted by the SDK.
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                return PlatformLogLevel;
            }

            set
            {
                PlatformLogLevel = value;
            }
        }

        /// <summary>
        /// Get the current version of AppCenter SDK.
        /// </summary>
        public static string SdkVersion
        {
            get { return WrapperSdk.Version; }
        }

        /// <summary>
        /// Check whether the SDK is enabled or not as a whole.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            return PlatformIsEnabledAsync();
        }

        /// <summary>
        ///     Enable or disable the SDK as a whole. 
        ///     Updating the state propagates the value to all services that have been started.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            return PlatformSetEnabledAsync(enabled);
        }

        /// <summary>
        ///     Get the unique installation identifier for this application installation on this device.
        /// </summary>
        /// <remarks>
        ///     The identifier is lost if clearing application data or uninstalling application.
        /// </remarks>
        public static Task<Guid?> GetInstallIdAsync()
        {
            return PlatformGetInstallIdAsync();
        }

        /// <summary>
        ///     Change the base URL (scheme + authority + port only) used to communicate with the backend.
        /// </summary>
        /// <param name="logUrl">Base URL to use for server communication.</param>
        public static void SetLogUrl(string logUrl)
        {
            PlatformSetLogUrl(logUrl);
        }

        /// <summary>
        /// Check whether SDK has already been configured or not.
        /// </summary>
        public static bool Configured => PlatformConfigured;

        /// <summary>
        ///     Configure the SDK.
        ///     This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        public static void Configure(string appSecret)
        {
            PlatformConfigure(appSecret);
        }

        /// <summary>
        ///     Start services.
        ///     This may be called only once per service per application process lifetime.
        /// </summary>
        /// <param name="services">List of services to use.</param>
        public static void Start(params Type[] services)
        {
            PlatformStart(services);
        }

        /// <summary>
        ///     Initialize the SDK with the list of services to start.
        ///     This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        /// <param name="services">List of services to use.</param>
        public static void Start(string appSecret, params Type[] services)
        {
            PlatformStart(appSecret, services);
        }

        /// <summary>
        /// Set the custom properties.
        /// </summary>
        /// <param name="customProperties">Custom properties object.</param>
        public static void SetCustomProperties(CustomProperties customProperties)
        {
            PlatformSetCustomProperties(customProperties);
        }
    }
}
