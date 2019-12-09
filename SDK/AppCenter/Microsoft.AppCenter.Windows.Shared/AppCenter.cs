// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
using Microsoft.AppCenter.Windows.Shared.Utils;

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

        // The lock is static. Instance methods are not necessarily thread safe, but static methods are
        private static readonly object AppCenterLock = new object();

        private static IApplicationSettingsFactory _applicationSettingsFactory;
        private static IChannelGroupFactory _channelGroupFactory;

        private readonly IApplicationSettings _applicationSettings;
        private INetworkStateAdapter _networkStateAdapter;
        private IChannelGroup _channelGroup;
        private IChannelUnit _channel;
        private readonly HashSet<IAppCenterService> _services = new HashSet<IAppCenterService>();
        private List<string> _startedServiceNames;
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

        /// <summary>
        /// Sets the two-letter ISO country code to send to the backend.
        /// </summary>
        /// <param name="countryCode">The two-letter ISO country code. See <see href="https://www.iso.org/obp/ui/#search"/> for more information.</param>
        public static void SetCountryCode(string countryCode)
        {
            if (countryCode != null && countryCode.Length != 2)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "App Center accepts only the two-letter ISO country code.");
                return;
            }
            DeviceInformationHelper.SetCountryCode(countryCode);
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
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }

        static void PlatformSetUserId(string userId)
        {
            if (userId != null && !UserIdContext.CheckUserIdValidForAppCenter(userId))
            {
                return;
            }
            UserIdContext.Instance.UserId = userId;
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            lock (AppCenterLock)
            {
                return Instance.SetInstanceEnabled(enabled);
            }
        }

        static Task<Guid?> PlatformGetInstallIdAsync()
        {
            lock (AppCenterLock)
            {
                var value = Instance._applicationSettings.GetValue<Guid?>(InstallIdKey);
                if (value == null)
                {
                    value = Guid.NewGuid();
                    Instance._applicationSettings.SetValue(InstallIdKey, value);
                }
                return Task.FromResult(value);
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

        internal static void PlatformUnsetInstance()
        {
            Instance = null;
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

        /// <summary>
        /// A wrapper SDK can use this method to pass extra information to device properties.
        /// </summary>
        /// <param name="wrapperSdk">Wrapper SDK information.</param>
        public static void SetWrapperSdk(WrapperSdk wrapperSdk)
        {
            DeviceInformationHelper.SetWrapperSdk(wrapperSdk);
        }

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

        private bool InstanceEnabled => _applicationSettings.GetValue(EnabledKey, true);

        // That method isn't async itself but can return async task from the channel for awaiting log enqueue.
        private Task SetInstanceEnabled(bool value)
        {
            var enabledTerm = value ? "enabled" : "disabled";
            if (InstanceEnabled == value)
            {
                AppCenterLog.Info(AppCenterLog.LogTag, $"App Center has already been {enabledTerm}.");
                return Task.FromResult(default(object));
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

            // Send started services.
            if (_startedServiceNames != null && value)
            {
                var startServiceLog = new StartServiceLog { Services = _startedServiceNames };
                _startedServiceNames = null;
                return _channel.EnqueueAsync(startServiceLog);
            }
            return Task.FromResult(default(object));
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
                AppCenterLog.Error(AppCenterLog.LogTag, "Custom properties may not be null or empty.");
                return;
            }
            _channel.EnqueueAsync(new CustomPropertyLog { Properties = customProperties.Properties });
        }

        private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionOccurredEventArgs args)
        {
            // Make sure that all storage operations are complete.
            _channelGroup?.WaitStorageOperationsAsync().GetAwaiter().GetResult();
        }

        // Internal for testing
        internal void InstanceConfigure(string appSecretOrSecrets)
        {
            if (_instanceConfigured)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "App Center may only be configured once.");
                return;
            }
            _appSecret = GetSecretAndTargetForPlatform(appSecretOrSecrets, PlatformIdentifier);

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
            AppCenterLog.Info(AppCenterLog.LogTag, "App Center SDK configured successfully.");
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

            var serviceNames = new List<string>();
            foreach (var serviceType in services)
            {
                if (serviceType == null)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, "Skipping null service. Please check that you did not pass a null argument.");
                    continue;
                }
                try
                {
                    var serviceInstance = serviceType.GetRuntimeProperty("Instance")?.GetValue(null) as IAppCenterService;
                    if (serviceInstance == null)
                    {
                        throw new AppCenterException("Service type does not contain static 'Instance' property of type IAppCenterService. The service is either not an App Center service or it's unsupported on this platform or the SDK is used from a .NET standard library and the nuget was not also added to the UWP/WPF/WinForms project.");
                    }
                    StartService(serviceInstance);
                    serviceNames.Add(serviceInstance.ServiceName);
                }
                catch (AppCenterException e)
                {
                    AppCenterLog.Error(AppCenterLog.LogTag, $"Failed to start service '{serviceType.Name}'; skipping it.", e);
                }
            }

            // Enqueue a log indicating which services have been initialized
            if (serviceNames.Count > 0)
            {
                if (InstanceEnabled)
                {
                    _channel.EnqueueAsync(new StartServiceLog { Services = serviceNames });
                }
                else
                {
                    if (_startedServiceNames == null)
                    {
                        _startedServiceNames = new List<string>();
                    }
                    _startedServiceNames.AddRange(serviceNames);
                }
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

        #endregion
    }
}
