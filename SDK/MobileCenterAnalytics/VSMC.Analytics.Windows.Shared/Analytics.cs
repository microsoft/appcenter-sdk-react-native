using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Core;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Analytics.Channel;

namespace Microsoft.Azure.Mobile.Analytics
{
    public class Analytics : MobileCenterService
    {
        #region static

        private readonly static object AnalyticsLock = new object();

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

        private SessionTracker _sessionTracker;

        internal Analytics()
        {
            LogSerializer.AddFactory(PageLog.JsonIdentifier, new LogFactory<PageLog>());
            LogSerializer.AddFactory(EventLog.JsonIdentifier, new LogFactory<EventLog>());
            LogSerializer.AddFactory(StartSessionLog.JsonIdentifier, new LogFactory<StartSessionLog>());
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

        protected override string ServiceName => "Analytics";

        private void InstanceTrackEvent(string name, IDictionary<string, string> properties = null)
        {
            if (IsInactive)
            {
                return;
            }
            var log = new EventLog(0, null, Guid.NewGuid(), name, null, properties);
            Channel.Enqueue(log);
        }

        public override void OnChannelGroupReady(ChannelGroup channelGroup)
        {
            base.OnChannelGroupReady(channelGroup);
            ApplyEnabledState(InstanceEnabled);
        }

        private void ResumingHandler(object sender, object e)
        {
            _sessionTracker?.Resume();
        }
        private void SuspendingHandler(object sender, object e)
        {
            _sessionTracker?.Pause();
        }

        private void ApplyEnabledState(bool enabled)
        {
            if (enabled && ChannelGroup != null && _sessionTracker == null)
            {
                _sessionTracker = new SessionTracker(ChannelGroup, Channel);
                CoreApplication.Resuming += ResumingHandler;
                CoreApplication.Suspending += SuspendingHandler;
                _sessionTracker.Resume();
            }
            else if (!enabled)
            {
                CoreApplication.Resuming -= ResumingHandler;
                CoreApplication.Suspending -= SuspendingHandler;
                _sessionTracker?.ClearSessions();
                _sessionTracker = null;
            }
        }

        #endregion
    }
}
