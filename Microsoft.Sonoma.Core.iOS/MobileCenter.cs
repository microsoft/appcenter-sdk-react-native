using System;
using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;

namespace Microsoft.Azure.Mobile
{
    using iOSSonoma = Microsoft.Azure.Mobile.iOS.Bindings.SNMSonoma;
    using iOSLogLevel = Microsoft.Azure.Mobile.iOS.Bindings.SNMLogLevel;
    using iOSWrapperSdk = Microsoft.Azure.Mobile.iOS.Bindings.SNMWrapperSdk;

    /// <summary>
    /// SDK core used to initialize, start and control specific feature.
    /// </summary>
    public static class MobileCenter
    {
        /// <summary>
        /// This property controls the amount of logs emitted by the SDK.
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                var val = iOSSonoma.LogLevel();
                switch (val)
                {
                    case iOSLogLevel.Verbose:
                        return LogLevel.Verbose;
                    case iOSLogLevel.Debug:
                        return LogLevel.Debug;
                    case iOSLogLevel.Info:
                        return LogLevel.Info;
                    case iOSLogLevel.Warning:
                        return LogLevel.Warn;
                    case iOSLogLevel.Error:
                        return LogLevel.Error;
                    case iOSLogLevel.Assert:
                        return LogLevel.Assert;
                    case iOSLogLevel.None:
                        return LogLevel.None;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(val), val, null);
                }
            }
            set
            {
                iOSLogLevel loglevel;
                switch (value)
                {
                    case LogLevel.Verbose:
                        loglevel = iOSLogLevel.Verbose;
                        break;
                    case LogLevel.Debug:
                        loglevel = iOSLogLevel.Debug;
                        break;
                    case LogLevel.Info:
                        loglevel = iOSLogLevel.Info;
                        break;
                    case LogLevel.Warn:
                        loglevel = iOSLogLevel.Warning;
                        break;
                    case LogLevel.Error:
                        loglevel = iOSLogLevel.Error;
                        break;
                    case LogLevel.Assert:
                        loglevel = iOSLogLevel.Assert;
                        break;
                    case LogLevel.None:
                        loglevel = iOSLogLevel.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
                iOSSonoma.SetLogLevel(loglevel);
            }
        }

        /// <summary>
        /// Change the base URL (scheme + authority + port only) used to communicate with the backend.
        /// </summary>
        /// <param name="serverUrl">Base URL to use for server communication.</param>
        public static void SetServerUrl(string serverUrl)
        {
            iOSSonoma.SetServerUrl(serverUrl);
        }

        /// <summary>
        /// Initialize the SDK.
        /// This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        public static void Initialize(string appSecret)
        {
            SetWrapperSdk();
            iOSSonoma.Start(appSecret);
        }

        /// <summary>
        /// Start features.
        /// This may be called only once per feature per application process lifetime.
        /// </summary>
        /// <param name="features">List of features to use.</param>
        public static void Start(params Type[] features)
        {
            SetWrapperSdk();
            foreach (var feature in GetFeatures(features))
            {
                iOSSonoma.StartFeature(feature);
            }
        }

        /// <summary>
        /// Initialize the SDK with the list of features to start.
        /// This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        /// <param name="features">List of features to use.</param>
        public static void Start(string appSecret, params Type[] features)
        {
            SetWrapperSdk();
            iOSSonoma.Start(appSecret, GetFeatures(features));
        }

        /// <summary>
        /// Enable or disable the SDK as a whole. Updating the property propagates the value to all features that have been started.
        /// </summary>
        /// <remarks>
        /// The default state is <c>true</c> and updating the state is persisted into local application storage.
        /// </remarks>
        public static bool Enabled
        {
            get { return iOSSonoma.IsEnabled(); }
            set { iOSSonoma.SetEnabled(value); }
        }

        /// <summary>
        /// Get the unique installation identifier for this application installation on this device.
        /// </summary>
        /// <remarks>
        /// The identifier is lost if clearing application data or uninstalling application.
        /// </remarks>
        public static Guid InstallId => Guid.Parse(iOSSonoma.InstallId().ToString());

        private static Class[] GetFeatures(IEnumerable<Type> features)
        {
            return features.Select(feature => GetClassForType(GetBindingType(feature))).ToArray();
        }

        private static Class GetClassForType(Type type)
        {
            IntPtr classHandle = Class.GetHandle(type);
            if (classHandle != IntPtr.Zero)
            {
                return new Class(classHandle);
            }
            return null; //TODO what should we do here? throw?
        }

        private static Type GetBindingType(Type type)
        {
            return (Type)type.GetProperty("BindingType").GetValue(null, null);
        }

        private static void SetWrapperSdk()
        {
            iOSWrapperSdk wrapperSdk = new iOSWrapperSdk(WrapperSdk.Version, WrapperSdk.Name, "", "", "");
            iOSSonoma.SetWrapperSdk(wrapperSdk);
        }
    }
}
