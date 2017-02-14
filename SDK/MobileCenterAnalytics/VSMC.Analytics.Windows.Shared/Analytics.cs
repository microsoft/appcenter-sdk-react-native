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

namespace Microsoft.Azure.Mobile.Analytics
{
    public class Analytics : IMobileCenterService
    {
        #region static
        internal static string LogTag = MobileCenterLog.LogTag + "Analytics";

        private const string EnabledKey = "MobileCenterAnalyticsEnabled";
        private const string ChannelName = "analytics";
        private static object _analyticsLock = new object();

        private static Analytics _instanceField;

        public static Analytics Instance
        {
            get
            {
                lock (_analyticsLock)
                {
                    if (_instanceField == null)
                    {
                        _instanceField = new Analytics();
                    }
                    return _instanceField;
                }
            }
            set
            {
                lock (_analyticsLock)
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
                lock (_analyticsLock)
                {
                    return Instance.InstanceEnabled;
                }
            }
            set
            {
                lock (_analyticsLock)
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
            lock (_analyticsLock)
            {
                Instance.InstanceTrackEvent(name, properties);
            }
        }
        #endregion

        #region instance

        private ChannelGroup _channelGroup = null;
        private bool _enabled = true;
        private IApplicationSettings _applicationSettings = new ApplicationSettings();
        private SessionTracker _sessionTracker;
        private const int MaxLogsPerBatch = 50;
        private static TimeSpan BatchTimeInterval = TimeSpan.FromSeconds(3);
        private const int MaxParallelBatches = 3;
        private List<EventLog> _unenqueuedEvents = new List<EventLog>();
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

        public bool InstanceEnabled
        {
            get
            {
                return _applicationSettings.GetValue(EnabledKey, defaultValue: true);
            }
            set
            {
                _applicationSettings[EnabledKey] = value;
                ApplyEnabledState(value);
            }
        }

        private void InstanceTrackEvent(string name, IDictionary<string, string> properties = null)
        {
            if (_enabled)
            {
                var log = new EventLog(0, null, Guid.NewGuid(), name, null, properties);
                if (_channelGroup == null)
                {
                    _unenqueuedEvents.Add(log);
                }
                else
                {
                    _channelGroup.GetChannel(ChannelName).Enqueue(log);
                }
            }
        }

        public void OnChannelGroupReady(ChannelGroup channelGroup)
        {
            _channelGroup = channelGroup;
            _channelGroup.AddChannel(ChannelName, MaxLogsPerBatch, BatchTimeInterval, MaxParallelBatches);
            ApplyEnabledState(InstanceEnabled);
        }

        private void ApplyEnabledState(bool enabled)
        {
            if (enabled && _channelGroup != null)
            {
                if (_sessionTracker == null)
                {
                    _sessionTracker = new SessionTracker(_channelGroup, ChannelName);
                    Resuming();
                }
                foreach (var log in _unenqueuedEvents)
                {
                    _channelGroup.GetChannel(ChannelName).Enqueue(log);
                }
                _unenqueuedEvents.Clear();
            }
            else if (!enabled && _sessionTracker != null)
            {
                _sessionTracker.ClearSessions();
                _sessionTracker = null;
            }
        }

        #endregion
    }
}
