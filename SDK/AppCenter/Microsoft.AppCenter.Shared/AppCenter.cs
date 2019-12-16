// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// SDK core used to initialize, start and control specific service.
    /// </summary>
    public partial class AppCenter
    {
        const string SecretDelimiter = ";";
        const string PlatformKeyValueDelimiter = "=";
        const string TargetKeyName = "target";
        const string TargetKeyNameUpper = "Target";
        const string AppSecretKeyName = "appsecret";
        const string SecretsPattern = @"([^;=]+)=([^;=]+);?";

#if NETSTANDARD
        static readonly Regex _secretsRegex = new Regex(SecretsPattern);
#else
        static readonly Regex _secretsRegex = new Regex(SecretsPattern, RegexOptions.Compiled);
#endif

        // Gets the first instance of an app sceret and/or target token corresponding to the given platform name, or returns the string 
        // as-is if no identifier can be found. Logs a message if no identifiers can be found.
        internal static string GetSecretAndTargetForPlatform(string secrets, string platformIdentifier)
        {
            var platformTargetIdentifier = platformIdentifier + TargetKeyNameUpper;
            if (string.IsNullOrEmpty(secrets))
            {
                throw new AppCenterException("App secrets string is null or empty");
            }

            // If there are no equals signs, then there are no named identifiers, but log a message in case the developer made 
            // a typing error.
            if (!secrets.Contains(PlatformKeyValueDelimiter))
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "No named identifier found in appSecret; using as-is");
                return secrets;
            }

            // Iterate over matching patterns.
            var secretsDictionary = new Dictionary<string, string>();
            var matches = _secretsRegex.Matches(secrets);
            foreach (Match match in matches)
            {
                secretsDictionary[match.Groups[1].Value] = match.Groups[2].Value;
            }

            // Extract the secrets for the current platform.
            if (secretsDictionary.ContainsKey(TargetKeyName))
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "Found 'target=' identifier in the secret; using as-is.");
                return secrets;
            }
            if (secretsDictionary.ContainsKey(AppSecretKeyName))
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, "Found 'appSecret=' identifier in the secret; using as-is.");
                return secrets;
            }
            var platformSecret = string.Empty;
            var platformTargetToken = string.Empty;
            if (secretsDictionary.ContainsKey(platformIdentifier))
            {
                secretsDictionary.TryGetValue(platformIdentifier, out platformSecret);
            }
            if (secretsDictionary.ContainsKey(platformTargetIdentifier))
            {
                secretsDictionary.TryGetValue(platformTargetIdentifier, out platformTargetToken);
            }
            if (string.IsNullOrEmpty(platformSecret) && string.IsNullOrEmpty(platformTargetToken))
            {
                throw new AppCenterException($"Error parsing key for '{platformIdentifier}'");
            }

            // Format the string as "appSecret={};target={}" or "target={}" if needed.
            if (!string.IsNullOrEmpty(platformTargetToken))
            {
                // If there is an app secret
                if (!string.IsNullOrEmpty(platformSecret))
                {
                    platformSecret = AppSecretKeyName + PlatformKeyValueDelimiter + platformSecret + SecretDelimiter;
                }
                platformSecret += TargetKeyName + PlatformKeyValueDelimiter + platformTargetToken;
            }
            return platformSecret;
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

        internal static void UnsetInstance()
        {
            PlatformUnsetInstance();
        }
    }
}
