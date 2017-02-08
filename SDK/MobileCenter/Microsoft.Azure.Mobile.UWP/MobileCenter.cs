#define DEBUG

using Microsoft.Azure.Mobile.UWP;
using Microsoft.Azure.Mobile.UWP.Channel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Microsoft.Azure.Mobile
{
    /// <summary>
    ///     SDK core used to initialize, start and control specific service.
    /// </summary>
    public class MobileCenter
    {
        private const string EnabledKey = "MobileCenterEnabled";
        private ChannelGroup _channelGroup;
        private HashSet<IMobileCenterService> _services = new HashSet<IMobileCenterService>();
        private bool _configured = false;
        private string _serverUrl;
        private static object _mobileCenterLock = new object();

        #region static


        private static MobileCenter _instanceField;

        internal static MobileCenter Instance
        {
            get
            {
                lock (_mobileCenterLock)
                {
                    if (_instanceField == null)
                    {
                        _instanceField = new MobileCenter();
                    }
                    return _instanceField;
                }
            }
            set
            {
                lock (_mobileCenterLock)
                {
                    _instanceField = value; //for testing
                }
            }
        }

        /// <summary>
        ///     This property controls the amount of logs emitted by the SDK.
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                return MobileCenterLog.Level;
            }
            set
            {
                MobileCenterLog.Level = value;
            }
        }

        /// <summary>
        ///     Enable or disable the SDK as a whole. Updating the property propagates the value to all services that have been
        ///     started.
        /// </summary>
        /// <remarks>
        ///     The default state is <c>true</c> and updating the state is persisted into local application storage.
        /// </remarks>
        public static bool Enabled
        {
            get
            {
                lock (_mobileCenterLock)
                {
                    return Instance.InstanceEnabled;
                }
            }
            set
            {
                lock (_mobileCenterLock)
                {
                    Instance.InstanceEnabled = value;
                }
            }
        }

        /// <summary>
        ///     Get the unique installation identifier for this application installation on this device.
        /// </summary>
        /// <remarks>
        ///     The identifier is lost if clearing application data or uninstalling application.
        /// </remarks>
        public static Guid? InstallId { get; }

        /// <summary>
        ///     Change the base URL (scheme + authority + port only) used to communicate with the backend.
        /// </summary>
        /// <param name="serverUrl">Base URL to use for server communication.</param>
        public static void SetServerUrl(string serverUrl)
        {
            lock (_mobileCenterLock)
            {
                Instance.SetInstanceServerUrl(serverUrl);
            }
        }

        /// <summary>
        /// Check whether SDK has already been configured or not.
        /// </summary>
        public static bool Configured
        {
            get
            {
                lock (_mobileCenterLock)
                {
                    return Instance.InstanceConfigured;
                }
            }
        }

        /// <summary>
        ///     Configure the SDK.
        ///     This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        public static void Configure(string appSecret)
        {
            lock (_mobileCenterLock)
            {
                Instance.InstanceConfigure(appSecret);
            }
        }

        /// <summary>
        ///     Start services.
        ///     This may be called only once per service per application process lifetime.
        /// </summary>
        /// <param name="services">List of services to use.</param>
        public static void Start(params Type[] services)
        {
            lock (_mobileCenterLock)
            {
                Instance.StartInstance(services);
            }
        }

        /// <summary>
        ///     Initialize the SDK with the list of services to start.
        ///     This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        /// <param name="services">List of services to use.</param>
        public static void Start(string appSecret, params Type[] services)
        {
            lock (_mobileCenterLock)
            {
                Instance.StartInstance(appSecret, services);
            }
        }
        #endregion

        #region instance

        private bool InstanceEnabled
        {
            get
            {
                object enabled;
                bool found = Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(EnabledKey, out enabled);
                if (!found)
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[EnabledKey] = true;
                    return true;
                }
                return (bool)enabled;
            }
            set
            {
                if (_channelGroup != null)
                {
                    _channelGroup.Enabled = false;
                }
                bool previouslyEnabled = InstanceEnabled;
                bool switchToDisabled = previouslyEnabled && !value;
                bool switchToEnabled = !previouslyEnabled && value;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[EnabledKey] = value;

                /* TODO register/unregister lifecycle callbacks? */

                foreach (var service in _services)
                {
                    service.InstanceEnabled = value;
                }

                if (switchToDisabled)
                {
                    MobileCenterLog.Info(MobileCenterLog.LogTag, "Mobile Center has been disabled.");
                }
                else if (switchToEnabled)
                {
                    MobileCenterLog.Info(MobileCenterLog.LogTag, "Mobile Center has been enabled.");
                }
                else
                {
                    MobileCenterLog.Info(MobileCenterLog.LogTag, "Mobile Center has already been " + (value ? "enabled." : "disabled."));
                }
            }
        }

        private void SetInstanceServerUrl(string serverUrl)
        {
            _serverUrl = serverUrl;
            _channelGroup?.SetServerUrl(serverUrl);
        }

        private bool InstanceConfigured => _configured;

        private bool InstanceConfigure(string appSecret)
        {
            /* TODO Do something with log level? */

            if (_configured)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Mobile Center may only be configured once");
            }
            else if (appSecret == null || appSecret == "")
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "appSecret may not be null or empty");
            }
            else
            {
                _channelGroup = new ChannelGroup(appSecret);
                _channelGroup.Enabled = InstanceEnabled;
                if (_serverUrl != null)
                {
                    _channelGroup.SetServerUrl(_serverUrl);
                }
                _configured = true;
                MobileCenterLog.Assert(MobileCenterLog.LogTag, "Mobile Center SDK configured successfully.");
                return true;
            }
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "Mobile Center SDK configuration failed.");
            return false;
        }

        private void StartInstance(params Type[] services)
        {
            if (services == null)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Cannot start services; services array is null. Failed to start services.");
                return;
            }

            if (!_configured)
            {
                string serviceNames = "";
                foreach (var serviceType in services)
                {
                    serviceNames += "\t" + serviceType.Name + "\n";
                }
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Cannot start services; Mobile Center has not been configured. Failed to start the following services:\n" + serviceNames);
                return;
            }

            foreach (var serviceType in services)
            {
                if (serviceType == null)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Skipping null service. Please check that you did not pass a null argument.");
                    continue;
                }
                try
                {
                    IMobileCenterService serviceInstance = (IMobileCenterService)serviceType.GetProperty("_instance").GetGetMethod().Invoke(null, null);
                    StartService(serviceInstance);
                }
                catch (Exception ex)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, $"Failed to start service '{serviceType.Name}'; skipping it.", ex);
                }

            }
        }
        
        private void StartService(IMobileCenterService service)
        {
            if (_services.Contains(service))
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Mobile Center has already started a service of type '{service.GetType().Name}'.");
                return;
            }
            service.OnChannelGroupReady(_channelGroup);
            MobileCenterLog.Info(MobileCenterLog.LogTag, $"'{service.GetType().Name}' service started.");
        }

        private void StartInstance(string appSecret, params Type[] services)
        {
            InstanceConfigure(appSecret);
            StartInstance(services);
        }
        #endregion
    }
}