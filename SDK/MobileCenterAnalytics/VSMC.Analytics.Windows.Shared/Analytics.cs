using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using System.Runtime.InteropServices;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Windows.ApplicationModel.Core;

//TODO harmonize thread safety with abstract base class
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
        private readonly List<EventLog> _unenqueuedEvents = new List<EventLog>();

        internal Analytics()
        {
            LogSerializer.AddFactory(PageLog.JsonIdentifier, new LogFactory<PageLog>());
            LogSerializer.AddFactory(EventLog.JsonIdentifier, new LogFactory<EventLog>());
            LogSerializer.AddFactory(StartSessionLog.JsonIdentifier, new LogFactory<StartSessionLog>());

            CoreApplication.Resuming += (sender, e) =>
            {
                Resuming();
            };
            CoreApplication.Suspending += (sender, e) =>
            {
                Suspending();
            };
           //TODO handle exiting? what about launching?
        }

        private void Suspending()
        {
            _sessionTracker?.Pause();
        }

        private void Resuming()
        {
            _sessionTracker?.Resume();
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
            if (InstanceEnabled)
            {
                var log = new EventLog(0, null, Guid.NewGuid(), name, null, properties);
                if (ChannelGroup == null)
                {
                    _unenqueuedEvents.Add(log);
                }
                else
                {
                    ChannelGroup.GetChannel(ChannelName).Enqueue(log);
                }
            }
        }

        public override void OnChannelGroupReady(ChannelGroup channelGroup)
        {
            base.OnChannelGroupReady(channelGroup);
            ApplyEnabledState(InstanceEnabled);
        }

        private void ApplyEnabledState(bool enabled)
        {
            if (enabled && ChannelGroup != null)
            {
                if (_sessionTracker == null)
                {
                    _sessionTracker = new SessionTracker(ChannelGroup, ChannelName);
                    Resuming();
                }
                foreach (var log in _unenqueuedEvents)
                {
                    ChannelGroup.GetChannel(ChannelName).Enqueue(log);
                }
                _unenqueuedEvents.Clear();
            }
            else if (!enabled)
            {
                _sessionTracker?.ClearSessions();
                _sessionTracker = null;
            }
        }
        #endregion
    }
}
