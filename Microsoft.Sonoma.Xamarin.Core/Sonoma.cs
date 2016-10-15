using System;

namespace Microsoft.Sonoma.Xamarin.Core
{
    /// <summary>
    /// SDK core used to initialize, start and control specific feature.
    /// </summary>
    public static class Sonoma
    {
        /// <summary>
        /// This property controls the amount of logs emitted by the SDK.
        /// </summary>
        public static LogLevel LogLevel
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Change the base URL (scheme + authority + port only) used to communicate with the backend.
        /// </summary>
        /// <param name="serverUrl">Base URL to use for server communication.</param>
        public static void SetServerUrl(string serverUrl)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialize the SDK.
        /// This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        public static void Initialize(string appSecret)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Start features.
        /// This may be called only once per feature per application process lifetime.
        /// </summary>
        /// <param name="features">List of features to use.</param>
        public static void Start(params Type[] features)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialize the SDK with the list of features to start.
        /// This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        /// <param name="features">List of features to use.</param>
        public static void Start(string appSecret, params Type[] features)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enable or disable the SDK as a whole. Updating the property propagates the value to all features that have been started.
        /// </summary>
        /// <remarks>
        /// The default state is <c>true</c> and updating the state is persisted into local application storage.
        /// </remarks>
        public static bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get the unique installation identifier for this application installation on this device.
        /// </summary>
        /// <remarks>
        /// The identifier is lost if clearing application data or uninstalling application.
        /// </remarks>
        public static Guid InstallId
        {
            get { throw new NotImplementedException(); }
        }
    }
}