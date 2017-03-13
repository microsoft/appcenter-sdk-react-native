using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Microsoft.Azure.Mobile
{
    /// <summary>
    /// SDK core used to initialize, start and control specific service.
    /// </summary>
    public partial class MobileCenter
    {
        /* Internal for testing */
        internal const string EnabledKey = "MobileCenterEnabled";
        internal const string InstallIdKey = "MobileCenterInstallId";

        /* Internal for testing */
        private readonly IApplicationSettings _applicationSettings;
        private readonly IChannelGroupFactory _channelGroupFactory;
        private IChannelGroup _channelGroup;
        private readonly HashSet<IMobileCenterService> _services = new HashSet<IMobileCenterService>();
        private string _serverUrl;
        private static readonly object MobileCenterLock = new object();
        private static bool _logLevelSet;
        private const string ConfigurationErrorMessage = "Failed to configure Mobile Center";
        private const string StartErrorMessage = "Failed to start services";
        private bool _instanceConfigured;

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

        /* This method is only for testing */
        internal static void Reset()
        {
            lock (MobileCenterLock)
            {
                _instanceField = null;
                _logLevelSet = false;
            }
        }

        /// <summary>
        ///     This property controls the amount of logs emitted by the SDK.
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                lock (MobileCenterLock)
                {
                    return MobileCenterLog.Level;
                }
            }
            set
            {
                lock (MobileCenterLock)
                {
                    MobileCenterLog.Level = value;
                    _logLevelSet = true;
                }
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
        /// Get the unique installation identifier for this application installation on this device.
        /// </summary>
        /// <remarks>
        /// The identifier is lost if clearing application data or uninstalling application.
        /// </remarks>
        public static Guid? InstallId => Instance._applicationSettings.GetValue(InstallIdKey, Guid.NewGuid());

        /// <summary>
        /// Change the base URL (scheme + authority + port only) used to communicate with the backend.
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
                    return Instance._instanceConfigured;
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
                try
                {
                    Instance.InstanceConfigure(appSecret);
                }
                catch (MobileCenterException ex)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, ConfigurationErrorMessage, ex);
                }
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
                try
                {
                    Instance.StartInstance(services);
                }
                catch (MobileCenterException ex)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, StartErrorMessage, ex);
                }
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

        internal MobileCenter()
        {
             _applicationSettings = new ApplicationSettings();
            LogSerializer.AddFactory(StartServiceLog.JsonIdentifier, new LogFactory<StartServiceLog>());
        }

        /* This constructor is only for unit testing */
        internal MobileCenter(IApplicationSettings applicationSettings, IChannelGroupFactory channelGroupFactory = null)
        {
            _applicationSettings = applicationSettings;
            _channelGroupFactory = channelGroupFactory;
        }

        private IChannelGroup CreateChannelGroup(string appSecret)
        {
            return _channelGroupFactory?.CreateChannelGroup(appSecret) ?? new ChannelGroup(appSecret);
        }

        private bool InstanceEnabled
        {
            get
            {
                return _applicationSettings.GetValue(EnabledKey, true);
            }
            set
            {
                var enabledTerm = value ? "enabled" : "disabled";
                if (InstanceEnabled == value)
                {
                    MobileCenterLog.Info(MobileCenterLog.LogTag, $"Mobile Center has already been {enabledTerm}.");
                    return;
                }

                _channelGroup?.SetEnabled(value);
                _applicationSettings[EnabledKey] = value;

                foreach (var service in _services)
                {
                    service.InstanceEnabled = value;
                }
                MobileCenterLog.Info(MobileCenterLog.LogTag, $"Mobile Center has been {enabledTerm}.");
            }
        }

        private void SetInstanceServerUrl(string serverUrl)
        {
            _serverUrl = serverUrl;
            _channelGroup?.SetServerUrl(serverUrl);
        }


        internal void InstanceConfigure(string appSecretString)
        {
            if (!_logLevelSet)
            {
                MobileCenterLog.Level = LogLevel.Warn;
                _logLevelSet = true;
            }
            if (_instanceConfigured)
            {
                throw new MobileCenterException("Multiple attempts to configure Mobile Center");
            }
            var appSecret = GetSecretForPlatform(appSecretString, PlatformIdentifier);
            _channelGroup = CreateChannelGroup(appSecret);
            if (_serverUrl != null)
            {
                _channelGroup.SetServerUrl(_serverUrl);
            }
            _instanceConfigured = true;
            MobileCenterLog.Assert(MobileCenterLog.LogTag, "Mobile Center SDK configured successfully.");
        }

        internal void StartInstance(params Type[] services)
        {
            if (services == null)
            {
                throw new MobileCenterException("Services array is null.");
            }
            if (!_instanceConfigured)
            {
                throw new MobileCenterException("Mobile Center has not been configured.");
            }

            List<string> startedServiceNames = new List<string>();

            foreach (var serviceType in services)
            {
                if (serviceType == null)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Skipping null service. Please check that you did not pass a null argument.");
                    continue;
                }
                try
                {
                    var serviceInstance =
                        (IMobileCenterService) serviceType.GetRuntimeProperty("Instance")?.GetValue(null);

                    if (StartService(serviceInstance))
                        startedServiceNames.Add(serviceInstance.ServiceName);
                }
                catch (MobileCenterException ex)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Failed to start service '{serviceType.Name}'; skipping it.", ex);
                }
            }

            if (startedServiceNames.Count > 0)
            {
                StartServiceLog serviceLog = new StartServiceLog();
                serviceLog.Services = startedServiceNames;
                _channelGroup.Enqueue( serviceLog );
            }
        }

        private bool StartService(IMobileCenterService service)
        {
            if (service == null)
            {
                throw new MobileCenterException("Service instance is null; static 'Instance' property either doesn't exist or returned null");
            }
            if (_services.Contains(service))
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Mobile Center has already started a service of type '{service.GetType().Name}'.");
                return false;
            }

            service.OnChannelGroupReady(_channelGroup);
            _services.Add(service);
            MobileCenterLog.Info(MobileCenterLog.LogTag, $"'{service.GetType().Name}' service started.");
            return true;
        }

        public void StartInstance(string appSecret, params Type[] services)
        {
            try
            {
                InstanceConfigure(appSecret);
                StartInstance(services);
            }
            catch (MobileCenterException ex)
            {
                var message = _instanceConfigured ? StartErrorMessage : ConfigurationErrorMessage;
                MobileCenterLog.Error(MobileCenterLog.LogTag, message, ex);
            }
        }

        #endregion
    }
}