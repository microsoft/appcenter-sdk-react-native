using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Analytics
{
    public class Analytics : MobileCenterService
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
        ///     Enable or disable Analytics module.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                lock (AnalyticsLock)
                {
                    return Instance.InstanceEnabled;
                }
            }
            set
            {
                lock (AnalyticsLock)
                {
                    Instance.InstanceEnabled = value;
                }
            }
        }

        /// <summary>
        ///     Track a custom event.
        /// </summary>
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

        /* Internal for testing purposes */
        internal ISessionTracker SessionTracker;
        internal readonly IApplicationLifecycleHelper ApplicationLifecycleHelper = new ApplicationLifecycleHelper();
        private readonly ISessionTrackerFactory _sessionTrackerFactory;
        private bool _hasResumed;

        internal Analytics()
        {
            LogSerializer.AddLogType(PageLog.JsonIdentifier, typeof(PageLog));
            LogSerializer.AddLogType(EventLog.JsonIdentifier, typeof(EventLog));
            LogSerializer.AddLogType(StartSessionLog.JsonIdentifier, typeof(StartSessionLog));
        }

        internal Analytics(ISessionTrackerFactory sessionTrackerFactory, IApplicationLifecycleHelper lifecycleHelper) : this()
        {
            _sessionTrackerFactory = sessionTrackerFactory;
            ApplicationLifecycleHelper = lifecycleHelper;
        }

        public override bool InstanceEnabled
        {
            get { return base.InstanceEnabled; }
            set
            {
                var prevValue = InstanceEnabled;
                base.InstanceEnabled = value;
                if (value != prevValue)
                {
                    ApplyEnabledState(value);
                }
            }
        }

        protected override string ChannelName => "analytics";

        public override string ServiceName => "Analytics";

        private void InstanceTrackEvent(string name, IDictionary<string, string> properties = null)
        {
            if (IsInactive)
            {
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                MobileCenterLog.Error(LogTag, "Name cannot be null or empty");
                return;
            }
            if (name.Length > MaxEventNameLength)
            {
                MobileCenterLog.Error(LogTag, "Name cannot be longer then " + MaxEventNameLength + " characters");
                return;
            }
            if (!ValidateProperties(properties))
            {
                /* Error already logged */
                return;
            }
            var log = new EventLog(0, null, Guid.NewGuid(), name, null, properties);
            Channel.Enqueue(log);
        }

        public override void OnChannelGroupReady(IChannelGroup channelGroup)
        {
            base.OnChannelGroupReady(channelGroup);
            ApplyEnabledState(InstanceEnabled);
            ApplicationLifecycleHelper.ApplicationResuming += (sender, e) =>
            {
                SessionTracker?.Resume();
                _hasResumed = true;
            };
            ApplicationLifecycleHelper.ApplicationSuspended += (sender, e) => SessionTracker?.Pause();
        }

        private void ApplyEnabledState(bool enabled)
        {
            if (enabled && ChannelGroup != null && SessionTracker == null)
            {
                SessionTracker = CreateSessionTracker(ChannelGroup, Channel);
                ApplicationLifecycleHelper.Enabled = true;
                if (_hasResumed)
                {
                    SessionTracker.Resume();
                }
            }
            else if (!enabled)
            {
                ApplicationLifecycleHelper.Enabled = false;
                SessionTracker?.ClearSessions();
                SessionTracker = null;
            }
        }

        private ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannelUnit channel)
        {
            return _sessionTrackerFactory?.CreateSessionTracker(channelGroup, channel) ?? new SessionTracker(channelGroup, channel);
        }

        private bool ValidateProperties(IDictionary<string, string> properties)
        {
            if (properties == null)
                return true;
            if (properties.Count > MaxEventProperties)
            {
                MobileCenterLog.Error(LogTag, "Properties cannot be more then " + MaxEventProperties);
                return false;
            }
            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.Key))
                {
                    MobileCenterLog.Error(LogTag, "Property key cannot be null or empty");
                    return false;
                }
                if (property.Key.Length > MaxEventPropertyKeyLength)
                {
                    MobileCenterLog.Error(LogTag, "Property key cannot be longer then " + MaxEventPropertyKeyLength + " characters");
                    return false;
                }
                if (property.Value == null)
                {
                    MobileCenterLog.Error(LogTag, "Property value cannot be null");
                    return false;
                }
                if (property.Value.Length > MaxEventPropertyValueLength)
                {
                    MobileCenterLog.Error(LogTag, "Property value cannot be longer then " + MaxEventPropertyValueLength + " characters");
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
