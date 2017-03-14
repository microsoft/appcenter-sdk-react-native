using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Utils;
using VSMC.Analytics.Windows.Shared;

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
        internal SessionTracker SessionTracker;
        internal readonly IApplicationLifecycleHelper ApplicationLifecycleHelper = new ApplicationLifecycleHelper();
        private readonly ISessionTrackerFactory _sessionTrackerFactory;

        internal Analytics()
        {
            LogSerializer.AddFactory(PageLog.JsonIdentifier, new LogFactory<PageLog>());
            LogSerializer.AddFactory(EventLog.JsonIdentifier, new LogFactory<EventLog>());
            LogSerializer.AddFactory(StartSessionLog.JsonIdentifier, new LogFactory<StartSessionLog>());
        }

        internal Analytics(ISessionTrackerFactory sessionTrackerFactory) : this()
        {
            _sessionTrackerFactory = sessionTrackerFactory;
        }

        public override bool InstanceEnabled
        {
            get { return base.InstanceEnabled; }
            set
            {
                base.InstanceEnabled = value;
                ApplyEnabledState(value);
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

        private SessionTracker CreateSessionTracker(IChannelGroup channelGroup, IChannel channel)
        {
            return _sessionTrackerFactory?.CreateSessionTracker(channelGroup, channel) ?? new SessionTracker(channelGroup, channel);
        }

        #endregion
    }
}
