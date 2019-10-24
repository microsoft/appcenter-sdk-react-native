// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// SDK core used to initialize, start and control specific service.
    /// </summary>
    public partial class AppCenter
    {
        const char Delimiter = ';';
        const string PlatformIndicator = "=";
        const string TargetPostfix = "Target";
        const string SecretPostfix = "appSecret";

        // Gets the first instance of an app sceret and/or target token corresponding to the given platform name, or returns the string 
        // as-is if no identifier can be found. Logs a message if no identifiers can be found.
        internal static string GetSecretAndTargetForPlatform(string secrets, string platformIdentifier)
        {
            var platformTargetIdentifier = platformIdentifier + TargetPostfix;
            if (string.IsNullOrEmpty(secrets))
            {
                throw new AppCenterException("App secrets string is null or empty");
            }

            // If there are no equals signs, then there are no named identifiers, but log a message in case the developer made 
            // a typing error.
            if (!secrets.Contains(PlatformIndicator))
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
                return secrets;
            }

            var parseErrorMessage = $"Error parsing key for '{platformIdentifier}'";

            var platformIndicator = platformIdentifier + PlatformIndicator;
            var platformTargetIdicator = platformTargetIdentifier + PlatformIndicator;
            var secretIdx = secrets.IndexOf(platformIndicator, StringComparison.Ordinal);
            var targetTokenIdx = secrets.IndexOf(platformTargetIdicator, StringComparison.Ordinal);
            var targetIdx = secrets.IndexOf(TargetPostfix.ToLower(), StringComparison.Ordinal);
            if (secretIdx == -1 && targetTokenIdx == -1 && targetIdx == -1)
            {
                throw new AppCenterException(parseErrorMessage);
            }
            if (targetIdx >= 0 && secretIdx == -1 && targetTokenIdx == -1) 
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "Found named identifier 'target' in the secret. Returning as-is.");
                return secrets;
            }
            if (secretIdx >= 0)
            {
                secretIdx += platformIndicator.Length;
            }
            if (targetTokenIdx >= 0)
            {
                targetTokenIdx += platformTargetIdicator.Length;
            }
            var platformSecret = FindTheKey(secretIdx, secrets);
            var platformTargetToken = FindTheKey(targetTokenIdx, secrets);
            if (string.IsNullOrEmpty(platformSecret) && string.IsNullOrEmpty(platformTargetToken))
            {
                throw new AppCenterException(parseErrorMessage);
            }

            // Format the string as "appSecret={};target={}" or "target={}" if needed.
            if (platformTargetToken.Length > 0)
            {
                //If there is an app secret
                if (platformSecret.Length > 0)
                {
                    platformSecret = SecretPostfix + PlatformIndicator + platformSecret + Delimiter;
                }
                platformSecret += TargetPostfix.ToLower() + PlatformIndicator + platformTargetToken;
            }
            return platformSecret;
        }

        private static string FindTheKey(int keyIdx, string secrets) 
        {
            var key = string.Empty;
            if (keyIdx >= 0)
            {
                while (keyIdx < secrets.Length)
                {
                    var nextChar = secrets[keyIdx++];
                    if (nextChar == Delimiter)
                    {
                        break;
                    }
                    key += nextChar;
                }
            }
            return key;
        }

        /// <summary>
        ///     This property controls the amount of logs emitted by the SDK.
        /// </summary>
        public static LogLevel LogLevel
        {
            get => PlatformLogLevel;
            set => PlatformLogLevel = value;
        }

        /// <summary>
        ///     Set the custom user id.
        /// </summary>
        /// <param name="userId">Custom string to identify user. 256 characters or less.</param>
        public static void SetUserId(string userId)
        {
            PlatformSetUserId(userId);
        }

        /// <summary>
        /// Get the current version of AppCenter SDK.
        /// </summary>
        public static string SdkVersion => WrapperSdk.Version;

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
