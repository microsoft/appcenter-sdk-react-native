using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.Azure.Mobile.Ingestion.Models.Serialization;

namespace Microsoft.Azure.Mobile
{
    /// <summary>
    /// SDK core used to initialize, start and control specific service.
    /// </summary>
    public partial class MobileCenter
    {
        // Internals for testing
        internal const string EnabledKey = Constants.KeyPrefix + "Enabled";
        internal const string InstallIdKey = Constants.KeyPrefix + "InstallId";
        private const string ConfigurationErrorMessage = "Failed to configure Mobile Center";
        private const string StartErrorMessage = "Failed to start services";
        private const string ChannelName = "core";
        private const string DistributeServiceFullType = "Microsoft.Azure.Mobile.Distribute.Distribute";

        // The lock is static. Instance methods are not necessarily thread safe, but static methods are
        private static readonly object MobileCenterLock = new object();

        private readonly IApplicationSettings _applicationSettings;
        private readonly IChannelGroupFactory _channelGroupFactory;
        private IChannelGroup _channelGroup;
        private IChannelUnit _channel;
        private readonly HashSet<IMobileCenterService> _services = new HashSet<IMobileCenterService>();
        private string _logUrl;
        private bool _instanceConfigured;
        private string _appSecret;
        
        #region static

        // The shared instance of MobileCenter
        private static MobileCenter _instanceField;

        /// <summary>
        /// Gets or sets the shared instance of Mobile Center. Should never return null.
        /// Setter is for testing.
        /// </summary>
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
        /// Controls the amount of logs emitted by the SDK.
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
                }
            }
        }

        /// <summary>
        /// Enable or disable the SDK as a whole. Updating the property propagates the value to all services that have been
        /// started.
        /// </summary>
        /// <remarks>
        /// The default state is <c>true</c> and updating the state is persisted into local application storage.
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
        /// <param name="logUrl">Base URL to use for server communication.</param>
        public static void SetLogUrl(string logUrl)
        {
            lock (MobileCenterLock)
            {
                Instance.SetInstanceLogUrl(logUrl);
            }
        }

        // TODO: Make public when backend is ready.
        /// <summary>
        /// Set the custom properties.
        /// </summary>
        /// <param name="customProperties">Custom properties object.</param>
        internal static void SetCustomProperties(CustomProperties customProperties)
        {
            lock (MobileCenterLock)
            {
                Instance.SetInstanceCustomProperties(customProperties);
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
        /// Configure the SDK.
        /// This may be called only once per application process lifetime.
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
        /// Start services.
        /// This may be called only once per service per application process lifetime.
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
        /// Initialize the SDK with the list of services to start.
        /// This may be called only once per application process lifetime.
        /// </summary>
        /// <param name="appSecret">A unique and secret key used to identify the application.</param>
        /// <param name="services">List of services to use.</param>
        public static void Start(string appSecret, params Type[] services)
        {
            lock (MobileCenterLock)
            {
                Instance.StartInstanceAndConfigure(appSecret, services);
            }
        }

        #endregion

        #region instance

        // Creates a new instance of MobileCenter
        private MobileCenter()
        {
            _applicationSettings = new ApplicationSettings();
            LogSerializer.AddLogType(StartServiceLog.JsonIdentifier, typeof(StartServiceLog));
            LogSerializer.AddLogType(CustomPropertiesLog.JsonIdentifier, typeof(CustomPropertiesLog));
        }

        // This constructor is only for unit testing
        internal MobileCenter(IApplicationSettings applicationSettings, IChannelGroupFactory channelGroupFactory = null)
        {
            _applicationSettings = applicationSettings;
            _channelGroupFactory = channelGroupFactory;
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

                _channelGroup?.SetEnabledAsync(value);
                _applicationSettings[EnabledKey] = value;

                foreach (var service in _services)
                {
                    service.InstanceEnabled = value;
                }
                MobileCenterLog.Info(MobileCenterLog.LogTag, $"Mobile Center has been {enabledTerm}.");
            }
        }

        private void SetInstanceLogUrl(string logUrl)
        {
            _logUrl = logUrl;
            _channelGroup?.SetLogUrl(logUrl);
        }

        private void SetInstanceCustomProperties(CustomProperties customProperties)
        {
            if (customProperties == null || customProperties.Properties.Count == 0)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Custom properties may not be null or empty");
                return;
            }
            var customPropertiesLog = new CustomPropertiesLog();
            customPropertiesLog.Properties = customProperties.Properties;
            _channel.EnqueueAsync(customPropertiesLog);
        }

        // Internal for testing
        internal void InstanceConfigure(string appSecretOrSecrets)
        {
            if (_instanceConfigured)
            {
                throw new MobileCenterException("Multiple attempts to configure Mobile Center");
            }
            _appSecret = GetSecretForPlatform(appSecretOrSecrets, PlatformIdentifier);

            // If a factory has been supplied, use it to construct the channel group - this is designed for testing.
            // Normal scenarios will use new ChannelGroup(string).
            _channelGroup = _channelGroupFactory?.CreateChannelGroup(_appSecret) ?? new ChannelGroup(_appSecret);
            ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred += (sender, e) => _channelGroup.ShutdownAsync();
            _channel = _channelGroup.AddChannel(ChannelName, Constants.DefaultTriggerCount, Constants.DefaultTriggerInterval,
                                                Constants.DefaultTriggerMaxParallelRequests);
            if (_logUrl != null)
            {
                _channelGroup.SetLogUrl(_logUrl);
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

            var startServiceLog = new StartServiceLog();

            foreach (var serviceType in services)
            {
                if (serviceType == null)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Skipping null service. Please check that you did not pass a null argument.");
                    continue;
                }
                try
                {
                    // We don't support distribute in UWP, not even a custom start.
                    if (IsDistributeService(serviceType))
                    {
                        MobileCenterLog.Warn(MobileCenterLog.LogTag, "Distribute service is not yet supported on UWP.");
                    }
                    else
                    {
                        var serviceInstance = serviceType.GetRuntimeProperty("Instance")?.GetValue(null) as IMobileCenterService;
                        if (serviceInstance == null)
                        {
                            throw new MobileCenterException("Service type does not contain static 'Instance' property of type IMobileCenterService");
                        }
                        StartService(serviceInstance);
                        startServiceLog.Services.Add(serviceInstance.ServiceName);
                    }
                }
                catch (MobileCenterException ex)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Failed to start service '{serviceType.Name}'; skipping it.", ex);
                }
            }

            // Enqueue a log indicating which services have been initialized
            if (startServiceLog.Services.Count > 0)
            {
                _channel.EnqueueAsync(startServiceLog);
            }
        }

        private void StartService(IMobileCenterService service)
        {
            if (service == null)
            {
                throw new MobileCenterException("Service instance is null; static 'Instance' property either doesn't exist or returned null");
            }
            if (_services.Contains(service))
            {
                ThrowStartedServiceException(service.GetType().Name);
            }

            service.OnChannelGroupReady(_channelGroup, _appSecret);
            _services.Add(service);
            MobileCenterLog.Info(MobileCenterLog.LogTag, $"'{service.GetType().Name}' service started.");
        }

        public void StartInstanceAndConfigure(string appSecret, params Type[] services)
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

        // We don't support Distribute in UWP.
        private static bool IsDistributeService(Type serviceType)
        {
            return serviceType?.FullName == DistributeServiceFullType;
        }

        private void ThrowStartedServiceException(string serviceName)
        {
            throw new MobileCenterException($"Mobile Center has already started a service of type '{serviceName}'.");
        }
        #endregion
    }
}