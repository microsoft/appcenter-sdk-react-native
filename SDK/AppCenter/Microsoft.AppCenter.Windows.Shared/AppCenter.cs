using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter
{
    /// <summary>
    /// SDK core used to initialize, start and control specific service.
    /// </summary>
    public partial class AppCenter
    {
        // Internals for testing
        internal const string EnabledKey = Constants.KeyPrefix + "Enabled";
        internal const string InstallIdKey = Constants.KeyPrefix + "InstallId";
        private const string ConfigurationErrorMessage = "Failed to configure App Center.";
        private const string StartErrorMessage = "Failed to start services.";
        private const string NotConfiguredMessage = "App Center hasn't been configured. " +
                                                    "You need to call AppCenter.Start with appSecret or AppCenter.Configure first.";
        private const string ChannelName = "core";
        private const string DistributeServiceFullType = "Microsoft.AppCenter.Distribute.Distribute";

        // The lock is static. Instance methods are not necessarily thread safe, but static methods are
        private static readonly object AppCenterLock = new object();

        private static IApplicationSettingsFactory _applicationSettingsFactory;
        private static IChannelGroupFactory _channelGroupFactory;

        private readonly IApplicationSettings _applicationSettings;
        private INetworkStateAdapter _networkStateAdapter;
        private IChannelGroup _channelGroup;
        private IChannelUnit _channel;
        private readonly HashSet<IAppCenterService> _services = new HashSet<IAppCenterService>();
        private string _logUrl;
        private bool _instanceConfigured;
        private string _appSecret;

        #region static

        // The shared instance of AppCenter
        private static AppCenter _instanceField;

        /// <summary>
        /// Gets or sets the shared instance of App Center. Should never return null.
        /// Setter is for testing.
        /// </summary>
        internal static AppCenter Instance
        {
            get
            {
                lock (AppCenterLock)
                {
                    return _instanceField ?? (_instanceField = new AppCenter());
                }
            }
            set
            {
                lock (AppCenterLock)
                {
                    _instanceField = value;
                }
            }
        }

        static LogLevel PlatformLogLevel
        {
            get
            {
                lock (AppCenterLock)
                {
                    return AppCenterLog.Level;
                }
            }
            set
            {
                lock (AppCenterLock)
                {
                    AppCenterLog.Level = value;
                }
            }
        }

        // This method must be called *before* instance of AppCenter has been created
        // for a custom application settings to be used.
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete]
        public static void SetApplicationSettingsFactory(IApplicationSettingsFactory factory)
        {
            lock (AppCenterLock)
            {
                _applicationSettingsFactory = factory;
            }
        }

        // This method must be called *before* instance of AppCenter has been created
        // for a custom application settings to be used.
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete]
        public static void SetChannelGroupFactory(IChannelGroupFactory factory)
        {
            lock (AppCenterLock)
            {
                _channelGroupFactory = factory;
            }
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            // This is not really async for now, signature was introduced for Android
            // It's fine for callers of the current implementation to use Wait().
            lock (AppCenterLock)
            {
                var taskSource = new TaskCompletionSource<bool>();
                if (!Configured)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, NotConfiguredMessage);
                    taskSource.SetException(new AppCenterException(NotConfiguredMessage));
                }
                else
                {
                    taskSource.SetResult(Instance.InstanceEnabled);
                }
                return taskSource.Task;
            }
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            lock (AppCenterLock)
            {
                var taskSource = new TaskCompletionSource<object>();
                if (!Configured)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, NotConfiguredMessage);
                    taskSource.SetException(new AppCenterException(NotConfiguredMessage));
                }
                else
                {
                    Instance.InstanceEnabled = enabled;
                    taskSource.SetResult(null);
                }
                return taskSource.Task;
            }
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            lock (AppCenterLock)
            {
                var taskSource = new TaskCompletionSource<Guid?>();
                if (!Configured)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, NotConfiguredMessage);
                    taskSource.SetException(new AppCenterException(NotConfiguredMessage));
                }
                else
                {
                    var installId = (Guid?) Instance._applicationSettings.GetValue(InstallIdKey, Guid.NewGuid());
                    taskSource.SetResult(installId);
                }
                return taskSource.Task;
            }
        }

        static void PlatformSetLogUrl(string logUrl)
        {
            lock (AppCenterLock)
            {
                Instance.SetInstanceLogUrl(logUrl);
            }
        }

        internal static void PlatformSetCustomProperties(CustomProperties customProperties)
        {
            lock (AppCenterLock)
            {
                Instance.SetInstanceCustomProperties(customProperties);
            }
        }

        static bool PlatformConfigured
        {
            get
            {
                lock (AppCenterLock)
                {
                    return Instance._instanceConfigured;
                }
            }
        }

        static void PlatformConfigure(string appSecret)
        {
            lock (AppCenterLock)
            {
                try
                {
                    Instance.InstanceConfigure(appSecret);
                }
                catch (AppCenterException ex)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, ConfigurationErrorMessage, ex);
                }
            }
        }

        static void PlatformStart(params Type[] services)
        {
            lock (AppCenterLock)
            {
                try
                {
                    Instance.StartInstance(services);
                }
                catch (AppCenterException ex)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, StartErrorMessage, ex);
                }
            }
        }

        static void PlatformStart(string appSecret, params Type[] services)
        {
            lock (AppCenterLock)
            {
                try
                {
                    Instance.InstanceConfigure(appSecret);
                }
                catch (AppCenterException ex)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, ConfigurationErrorMessage, ex);
                }
                try
                {
                    Instance.StartInstance(services);
                }
                catch (AppCenterException ex)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, StartErrorMessage, ex);
                }
            }
        }
 
        // Atomically checks if the CorrelationId equals "testValue" and updates the value if true.
        // Returns "true" if value was changed. If not, the current value is assigned to setValue.
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete]
        public static bool TestAndSetCorrelationId(Guid testValue, ref Guid setValue)
        {
            lock (AppCenterLock)
            {
                if (testValue == Instance.InstanceCorrelationId)
                {
                    // Can't use the property setter here because that would cause the
                    // event to trigger within the lock, which is not allowed.
                    // (And calling the setter outside the lock would not be atomic).
                    Instance.InstanceCorrelationId = setValue;
                    CorrelationIdChanged?.Invoke(null, setValue);
                    return true;
                }
                setValue = Instance.InstanceCorrelationId;
            }
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete]
        // Note: Do not access the CorrelationId property in this event handler!
        // Doing so on a different thread can cause deadlocks.
        public static event EventHandler<Guid> CorrelationIdChanged;

        #endregion

        #region instance

        // Creates a new instance of AppCenter
        private AppCenter()
        {
            lock (AppCenterLock)
            {
                _applicationSettings = _applicationSettingsFactory?.CreateApplicationSettings() ?? new DefaultApplicationSettings();
                LogSerializer.AddLogType(StartServiceLog.JsonIdentifier, typeof(StartServiceLog));
                LogSerializer.AddLogType(CustomPropertyLog.JsonIdentifier, typeof(CustomPropertyLog));
                ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred += OnUnhandledExceptionOccurred;
            }
        }

        internal IApplicationSettings ApplicationSettings => _applicationSettings;
        internal INetworkStateAdapter NetworkStateAdapter => _networkStateAdapter;

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
                    AppCenterLog.Info(AppCenterLog.LogTag, $"App Center has already been {enabledTerm}.");
                    return;
                }

                // Update channels state.
                _channelGroup?.SetEnabled(value);

                // Store state in the application settings.
                _applicationSettings.SetValue(EnabledKey, value);

                // Apply change to services.
                foreach (var service in _services)
                {
                    service.InstanceEnabled = value;
                }
                AppCenterLog.Info(AppCenterLog.LogTag, $"App Center has been {enabledTerm}.");
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
                AppCenterLog.Error(AppCenterLog.LogTag, NotConfiguredMessage);
                return;
            }
            if (customProperties == null || customProperties.Properties.Count == 0)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Custom properties may not be null or empty");
                return;
            }
            var customPropertiesLog = new CustomPropertyLog();
            customPropertiesLog.Properties = customProperties.Properties;
            _channel.EnqueueAsync(customPropertiesLog);
        }

        private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionOccurredEventArgs args)
        {
            lock (AppCenterLock)
            {
                if (_channelGroup != null)
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag, "Shutting down channel group due to unhandled exception.");
                    _channelGroup.ShutdownAsync();
                    _channelGroup = null;
                }
            }
        }

        // Internal for testing
        internal void InstanceConfigure(string appSecretOrSecrets)
        {
            if (_instanceConfigured)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "App Center may only be configured once.");
                return;
            }
            _appSecret = GetSecretForPlatform(appSecretOrSecrets, PlatformIdentifier);

            // If a factory has been supplied, use it to construct the channel group - this is useful for wrapper SDKs and testing.
            _networkStateAdapter = new NetworkStateAdapter();
            _channelGroup = _channelGroupFactory?.CreateChannelGroup(_appSecret, _networkStateAdapter) ?? new ChannelGroup(_appSecret, null, _networkStateAdapter);
            _channel = _channelGroup.AddChannel(ChannelName, Constants.DefaultTriggerCount, Constants.DefaultTriggerInterval,
                                                Constants.DefaultTriggerMaxParallelRequests);
            _channel.SetEnabled(InstanceEnabled);
            if (_logUrl != null)
            {
                _channelGroup.SetLogUrl(_logUrl);
            }
            _instanceConfigured = true;
            AppCenterLog.Assert(AppCenterLog.LogTag, "App Center SDK configured successfully.");
        }

        internal void StartInstance(params Type[] services)
        {
            if (services == null)
            {
                throw new AppCenterException("Services array is null.");
            }
            if (!_instanceConfigured)
            {
                throw new AppCenterException("App Center has not been configured.");
            }

            var startServiceLog = new StartServiceLog();
            foreach (var serviceType in services)
            {
                if (serviceType == null)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, "Skipping null service. Please check that you did not pass a null argument.");
                    continue;
                }
                try
                {
                    // We don't support distribute in UWP, not even a custom start.
                    if (IsDistributeService(serviceType))
                    {
                        AppCenterLog.Warn(AppCenterLog.LogTag, "Distribute service is not yet supported on UWP.");
                    }
                    else
                    {
                        var serviceInstance = serviceType.GetRuntimeProperty("Instance")?.GetValue(null) as IAppCenterService;
                        if (serviceInstance == null)
                        {
                            throw new AppCenterException("Service type does not contain static 'Instance' property of type IAppCenterService");
                        }
                        StartService(serviceInstance);
                        startServiceLog.Services.Add(serviceInstance.ServiceName);
                    }
                }
                catch (AppCenterException e)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, $"Failed to start service '{serviceType.Name}'; skipping it.", e);
                }
            }

            // Enqueue a log indicating which services have been initialized
            if (startServiceLog.Services.Count > 0)
            {
                _channel.EnqueueAsync(startServiceLog);
            }
        }

        private void StartService(IAppCenterService service)
        {
            if (service == null)
            {
                throw new AppCenterException("Attempted to start an invalid App Center service.");
            }
            if (_channelGroup == null)
            {
                throw new AppCenterException("Attempted to start a service after App Center has been shut down.");
            }
            if (_services.Contains(service))
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, $"App Center has already started the service with class name '{service.GetType().Name}'");
                return;
            }
            if (!InstanceEnabled && service.InstanceEnabled)
            {
                service.InstanceEnabled = false;
            }
            service.OnChannelGroupReady(_channelGroup, _appSecret);
            _services.Add(service);
            AppCenterLog.Info(AppCenterLog.LogTag, $"'{service.GetType().Name}' service started.");
        }

        internal Guid InstanceCorrelationId = Guid.Empty;

        // We don't support Distribute in UWP.
        private static bool IsDistributeService(Type serviceType)
        {
            return serviceType?.FullName == DistributeServiceFullType;
        }

        #endregion
    }
}