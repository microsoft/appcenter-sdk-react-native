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
            var log = new EventLog(0, null, Guid.NewGuid(), name, null, properties);
            Channel.Enqueue(log);
        }

        public override void OnChannelGroupReady(IChannelGroup channelGroup)
        {
            base.OnChannelGroupReady(channelGroup);
            ApplyEnabledState(InstanceEnabled);
            ApplicationLifecycleHelper.ApplicationResuming += () => SessionTracker?.Resume();
            ApplicationLifecycleHelper.ApplicationSuspending += () => SessionTracker?.Pause();
        }

        private void ApplyEnabledState(bool enabled)
        {
            if (enabled && ChannelGroup != null && SessionTracker == null)
            {
                SessionTracker = CreateSessionTracker(ChannelGroup, Channel);
                ApplicationLifecycleHelper.Enabled = true;
                SessionTracker.Resume();
            }
            else if (!enabled)
            {
                ApplicationLifecycleHelper.Enabled = false;
                SessionTracker?.ClearSessions();
                SessionTracker = null;
            }
        }

        private ISessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel)
        {
            return _sessionTrackerFactory?.CreateSessionTracker(channelGroup, channel) ?? new SessionTracker(channelGroup, channel);
        }

        #endregion
    }
}
