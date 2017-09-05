using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.Azure.Mobile.Ingestion.Models.Serialization;
using System.Threading.Tasks;

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
        private const string ConfigurationErrorMessage = "Failed to configure Mobile Center.";
        private const string StartErrorMessage = "Failed to start services.";
        private const string ChannelName = "core";
        private const string DistributeServiceFullType = "Microsoft.Azure.Mobile.Distribute.Distribute";

        // The lock is static. Instance methods are not necessarily thread safe, but static methods are
        private static readonly object MobileCenterLock = new object();

        private static IApplicationSettingsFactory _applicationSettingsFactory;
        private static IChannelGroupFactory _channelGroupFactory;

        private readonly IApplicationSettings _applicationSettings;
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

        static LogLevel PlatformLogLevel
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

        // This method must be called *before* instance of MobileCenter has been created
        // for a custom application settings to be used.
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete]
        public static void SetApplicationSettingsFactory(IApplicationSettingsFactory factory)
        {
            lock (MobileCenterLock)
            {
                _applicationSettingsFactory = factory;
            }
        }

        // This method must be called *before* instance of MobileCenter has been created
        // for a custom application settings to be used.
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete]
        public static void SetChannelGroupFactory(IChannelGroupFactory factory)
        {
            lock (MobileCenterLock)
            {
                _channelGroupFactory = factory;
            }
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            // This is not really async for now, signature was introduced for Android
            // It's fine for callers of the current implementation to use Wait().
            lock (MobileCenterLock)
            {
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            lock (MobileCenterLock)
            {
                Instance.InstanceEnabled = enabled;
                return Task.FromResult(default(object));
            }
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            return Task.FromResult((Guid?)Instance._applicationSettings.GetValue(InstallIdKey, Guid.NewGuid()));
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            lock (MobileCenterLock)
            {
                Instance.SetInstanceLogUrl(logUrl);
            }
        }

        internal static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            lock (MobileCenterLock)
            {
                Instance.SetInstanceCustomProperties(customProperties);
            }
        }

        static bool PlatformConfigured
        {
            get
            {
                lock (MobileCenterLock)
                {
                    return Instance._instanceConfigured;
                }
            }
        }

        static void PlatformConfigure(string appSecret)
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

        static void PlatformStart(params Type[] services)
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

        static void PlatformStart(string appSecret, params Type[] services)
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
            lock (MobileCenterLock)
            {
                _applicationSettings = _applicationSettingsFactory?.CreateApplicationSettings() ?? new DefaultApplicationSettings();
                LogSerializer.AddLogType(StartServiceLog.JsonIdentifier, typeof(StartServiceLog));
                LogSerializer.AddLogType(CustomPropertiesLog.JsonIdentifier, typeof(CustomPropertiesLog));
            }
        }

        internal IApplicationSettings ApplicationSettings
        {
            get { return _applicationSettings; }
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
                _applicationSettings.SetValue(EnabledKey, value);

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
            if (!Configured)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Mobile Center hasn't been configured. You need to call MobileCenter.Start with appSecret or MobileCenter.Configure first.");
                return;
            }
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
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Mobile Center may only be configured once.");
                return;
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
                catch (MobileCenterException)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Failed to start service '{serviceType.Name}'; skipping it.");
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
                throw new MobileCenterException("Attempted to start an invalid Mobile Center service.");
            }
            if (_services.Contains(service))
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Mobile Center has already started the service with class name '{service.GetType().Name}'");
                return;
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
                MobileCenterLog.Warn(MobileCenterLog.LogTag, ex.Message);
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