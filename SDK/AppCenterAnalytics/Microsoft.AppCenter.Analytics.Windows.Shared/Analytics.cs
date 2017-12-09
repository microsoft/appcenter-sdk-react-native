using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Analytics.Channel;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Analytics
{
    public class Analytics : AppCenterService
    {
        #region static

        private const int MaxEventProperties = 5;
        private const int MaxEventNameLength = 256;
        private const int MaxEventPropertyKeyLength = 64;
        private const int MaxEventPropertyValueLength = 64;

        private static readonly object AnalyticsLock = new object();

        private static Analytics _instanceField;

        public static Analytics Instance
        {
            get
            {
                lock (AnalyticsLock)
                {
                    return _instanceField ?? (_instanceField = new Analytics());
                }
            }
            set
            {
                lock (AnalyticsLock)
                {
                    _instanceField = value; //for testing
                }
            }
        }

        /// <summary>
        /// Check whether the Analytics service is enabled or not.
        /// </summary>
        /// <returns>A task with result being true if enabled, false if disabled.</returns>
        public static Task<bool> IsEnabledAsync()
        {
            lock (AnalyticsLock)
            {
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }

        /// <summary>
        /// Enable or disable the Analytics service.
        /// </summary>
        /// <returns>A task to monitor the operation.</returns>
        public static Task SetEnabledAsync(bool enabled)
        {
            lock (AnalyticsLock)
            {
                Instance.InstanceEnabled = enabled;
                return Task.FromResult(default(object));
            }
        }

        /// <summary>
        ///     Track a custom event with name and optional properties.
        /// </summary>
        /// <remarks>
        ///     The name parameter can not be null or empty.Maximum allowed length = 256.
        ///     The properties parameter maximum item count = 5.
        ///     The properties keys/names can not be null or empty, maximum allowed key length = 64.
        ///     The properties values can not be null, maximum allowed value length = 64.
        /// </remarks>
        /// <param name="name">An event name.</param>
        /// <param name="properties">Optional properties.</param>
        public static void TrackEvent(string name, IDictionary<string, string> properties = null)
        {
            lock (AnalyticsLock)
            {
                Instance.InstanceTrackEvent(name, properties);
            }
        }

        #endregion

        #region instance

        // Internal for testing purposes
        internal ISessionTracker SessionTracker;
        private readonly ISessionTrackerFactory _sessionTrackerFactory;

        internal Analytics()
        {
            LogSerializer.AddLogType(PageLog.JsonIdentifier, typeof(PageLog));
            LogSerializer.AddLogType(EventLog.JsonIdentifier, typeof(EventLog));
            LogSerializer.AddLogType(StartSessionLog.JsonIdentifier, typeof(StartSessionLog));
        }

        internal Analytics(ISessionTrackerFactory sessionTrackerFactory) : this()
        {
            _sessionTrackerFactory = sessionTrackerFactory;
        }

        public override bool InstanceEnabled
        {
            get
            {
                return base.InstanceEnabled;
            }

            set
            {
                lock (_serviceLock)
                {
                    var prevValue = InstanceEnabled;
                    base.InstanceEnabled = value;
                    if (value != prevValue)
                    {
                        ApplyEnabledState(value);
                    }
                }
            }
        }

        protected override string ChannelName => "analytics";

        public override string ServiceName => "Analytics";

        private void InstanceTrackEvent(string name, IDictionary<string, string> properties = null)
        {
            lock (_serviceLock)
            {
                if (IsInactive)
                {
                    return;
                }
                const string type = "Event";
                if (ValidateName(ref name, type))
                {
                    properties = ValidateProperties(properties, name, type);
                    var log = new EventLog(null, null, Guid.NewGuid(), name, null, properties);
                    Channel.EnqueueAsync(log);
                }
            }
        }

        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            lock (_serviceLock)
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(InstanceEnabled);
            }
        }

        private void ApplyEnabledState(bool enabled)
        {
            lock (_serviceLock)
            {
                if (enabled && ChannelGroup != null && SessionTracker == null)
                {
                    SessionTracker = CreateSessionTracker(ChannelGroup, Channel, ApplicationSettings);
                    if (!ApplicationLifecycleHelper.Instance.IsSuspended)
                    {
                        SessionTracker.Resume();
                    }
                    SubscribeToApplicationLifecycleEvents();
                }
                else if (!enabled)
                {
                    UnsubscribeFromApplicationLifecycleEvents();
                    SessionTracker = null;
                }
            }
        }

        private ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannelUnit channel, IApplicationSettings applicationSettings)
        {
            return _sessionTrackerFactory?.CreateSessionTracker(channelGroup, channel, applicationSettings) ?? new SessionTracker(channelGroup, channel, applicationSettings);
        }

        /// <summary>
        /// Validates name.
        /// </summary>
        /// <param name="name">Log name to validate.</param>
        /// <param name="logType">Log type.</param>
        /// <returns><c>true</c> if validation succeeds, otherwise <с>false</с>.</returns>
        private bool ValidateName(ref string name, string logType)
        {
            if (string.IsNullOrEmpty(name))
            {
                AppCenterLog.Error(LogTag, $"{logType} name cannot be null or empty.");
                return false;
            }
            if (name.Length > MaxEventNameLength)
            {
                AppCenterLog.Warn(LogTag,
                    $"{logType} '{name}' : name length cannot be longer than {MaxEventNameLength} characters. Name will be truncated.");
                name = name.Substring(0, MaxEventNameLength);
                return true;
            }
            return true;
        }

        private void SubscribeToApplicationLifecycleEvents()
        {
            ApplicationLifecycleHelper.Instance.ApplicationResuming += ApplicationResumingEventHandler;
            ApplicationLifecycleHelper.Instance.ApplicationSuspended += ApplicationSuspendedEventHandler;
        }
        private void UnsubscribeFromApplicationLifecycleEvents()
        {
            ApplicationLifecycleHelper.Instance.ApplicationResuming -= ApplicationResumingEventHandler;
            ApplicationLifecycleHelper.Instance.ApplicationSuspended -= ApplicationSuspendedEventHandler;
        }

        private void ApplicationResumingEventHandler(object sender, EventArgs e)
        {
            SessionTracker?.Resume();
        }

        private void ApplicationSuspendedEventHandler(object sender, EventArgs e)
        {
            SessionTracker?.Pause();
        }

        /// <summary>
        /// Validates properties.
        /// </summary>
        /// <param name="properties">Properties collection to validate.</param>
        /// <param name="logName">Log name.</param>
        /// <param name="logType">Log type.</param>
        /// <returns>Valid properties collection with maximum size of 5</returns>
        private IDictionary<string, string> ValidateProperties(IDictionary<string, string> properties, string logName, string logType)
        {
            if (properties == null)
            {
                return null;
            }
            var result = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                if (result.Count >= MaxEventProperties)
                {
                    AppCenterLog.Warn(LogTag,
                        $"{logType} '{logName}' : properties cannot contain more than {MaxEventProperties} items. Skipping other properties.");
                    break;
                }

                // Skip empty property.
                var key = property.Key;
                var value = property.Value;
                if (string.IsNullOrEmpty(key))
                {
                    AppCenterLog.Warn(LogTag,
                        $"{logType} '{logName}' : a property key cannot be null or empty. Property will be skipped.");
                    break;
                }
                if (value == null)
                {
                    AppCenterLog.Warn(LogTag,
                        $"{logType} '{logName}' : property '{key}' : property value cannot be null. Property will be skipped.");
                    break;
                }

                // Truncate exceeded property.
                if (key.Length > MaxEventPropertyKeyLength)
                {
                    AppCenterLog.Warn(LogTag,
                        $"{logType} '{logName}' : property '{key}' : property key length cannot be longer than {MaxEventPropertyKeyLength} characters. Property key will be truncated.");
                    key = key.Substring(0, MaxEventPropertyKeyLength);
                }
                if (value.Length > MaxEventPropertyValueLength)
                {
                    AppCenterLog.Warn(LogTag,
                        $"{logType} '{logName}' : property '{key}' : property value length cannot be longer than {MaxEventPropertyValueLength} characters. Property value will be truncated.");
                    value = value.Substring(0, MaxEventPropertyValueLength);
                }
                result.Add(key, value);
            }
            return result;
        }

        #endregion
    }
}
