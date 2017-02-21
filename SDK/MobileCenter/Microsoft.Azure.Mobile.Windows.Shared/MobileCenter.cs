using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Microsoft.Azure.Mobile
{
    /// <summary>
    /// SDK core used to initialize, start and control specific service.
    /// </summary>
    public partial class MobileCenter
    {
        private const string EnabledKey = "MobileCenterEnabled";
        private ChannelGroup _channelGroup;
        private readonly HashSet<IMobileCenterService> _services = new HashSet<IMobileCenterService>();
        private bool _configured = false;
        private string _serverUrl;
        private readonly static object MobileCenterLock = new object();
        private readonly static IApplicationSettings ApplicationSettings = new ApplicationSettings();

        #region static

        private static MobileCenter _instanceField;

        internal static MobileCenter Instance
        {
            get
            {
                lock (MobileCenterLock)
                {
                    return _instanceField ?? (_instanceField = new MobileCenter());
                }
            }
            set
            {
                lock (MobileCenterLock)
                {
                    _instanceField = value;
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
                lock (MobileCenterLock)
                {
                    return Instance.InstanceEnabled;
                }
            }
            set
            {
                lock (MobileCenterLock)
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
        public static Guid? InstallId => IdHelper.InstallId;

        /// <summary>
        ///     Change the base URL (scheme + authority + port only) used to communicate with the backend.
        /// </summary>
        /// <param name="serverUrl">Base URL to use for server communication.</param>
        public static void SetServerUrl(string serverUrl)
        {
            lock (MobileCenterLock)
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
                lock (MobileCenterLock)
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
            lock (MobileCenterLock)
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
            lock (MobileCenterLock)
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
            lock (MobileCenterLock)
            {
                Instance.StartInstance(appSecret, services);
            }
        }
        #endregion

        #region instance

        //TODO need a way of giving application settings

        private bool InstanceEnabled
        {
            get
            {
                return ApplicationSettings.GetValue(EnabledKey, true);
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
                ApplicationSettings[EnabledKey] = value;

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
            else if (string.IsNullOrEmpty(appSecret))
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "appSecret may not be null or empty");
            }
            else
            {
                _channelGroup = new ChannelGroup(appSecret) {Enabled = InstanceEnabled};
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
                    IMobileCenterService serviceInstance = (IMobileCenterService)serviceType.GetRuntimeProperty("Instance").GetValue(null);
                    StartService(serviceInstance);
                }
                catch (Exception ex) //TODO make this more specific
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
            _services.Add(service);
            service.OnChannelGroupReady(_channelGroup);
            MobileCenterLog.Info(MobileCenterLog.LogTag, $"'{service.GetType().Name}' service started.");
        }

        public void StartInstance(string appSecret, params Type[] services)
        {
            string parsedSecret;
            try
            {
                parsedSecret = GetSecretForPlatform(appSecret, PlatformIdentifier);
            }
            catch (ArgumentException ex)
            {
                MobileCenterLog.Assert(MobileCenterLog.LogTag, ex.Message);
                return;
            }
            if (InstanceConfigure(parsedSecret))
            {
                StartInstance(services);
            }
        }
        #endregion
    }
}