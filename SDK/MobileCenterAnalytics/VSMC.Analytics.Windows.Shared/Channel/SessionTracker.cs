using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;
using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Linq;
using Windows.ApplicationModel.Core;

namespace Microsoft.Azure.Mobile.Analytics.Channel
{
    public class SessionTracker
    {
        private const string StorageKey = "MobileCenterSessions";
        private const int StorageMaxSessions = 5;
        private const char StorageKeyValueSeparator = '.';
        private const char StorageEntrySeparator = '/';
        private const long SessionTimeout = 20000;
        private readonly IChannel _channel;
        private readonly Dictionary<long, Guid> _sessions = new Dictionary<long, Guid>();
        private Guid? _sid;
        private long _lastQueuedLogTime;
        private long _lastResumedTime;
        private long _lastPausedTime;
        private readonly ApplicationSettings _applicationSettings = new ApplicationSettings();
        private readonly object _lockObject = new object();

        public SessionTracker(IChannelGroup channelGroup, IChannel channel)
        {
            _channel = channel;
            channelGroup.EnqueuingLog += HandleEnqueuingLog;
            var sessionsString = _applicationSettings.GetValue<string>(StorageKey, null);
            if (sessionsString == null) return;
            _sessions = SessionsFromString(sessionsString);
            if (_sessions.Count == 0) return;
            var loadedSessionsString = _sessions.Values.Aggregate("Loaded stored sessions:\n", (current, session) => current + ("\t" + session + "\n"));
            MobileCenterLog.Debug(Analytics.Instance.LogTag, loadedSessionsString);
        }

        private void HandleEnqueuingLog(object sender, EnqueuingLogEventArgs e)
        {
            lock (_lockObject)
            {
                if (e.Log is StartSessionLog) return;
                if (e.Log.Toffset > 0)
                {
                    long candidate = 0;
                    var foundPrevSession = false;
                    foreach (var key in _sessions.Keys)
                    {
                        if (key <= e.Log.Toffset && key > candidate)
                        {
                            candidate = key;
                            foundPrevSession = true;
                        }
                    }
                    if (foundPrevSession)
                    {
                        e.Log.Sid = _sessions[candidate];
                    }
                }
                if (e.Log.Sid != null) return;
                SendStartSessionIfNeeded();
                e.Log.Sid = _sid;
                _lastQueuedLogTime = TimeHelper.CurrentTimeInMilliseconds();
            }
        }

        public string SessionsAsString()
        {
            var sessionsString = "";
            foreach (var pair in _sessions)
            {
                if (sessionsString != "") sessionsString += StorageEntrySeparator;
                sessionsString += pair.Key.ToString() + StorageKeyValueSeparator + pair.Value;
            }
            return sessionsString;
        }

        public Dictionary<long, Guid> SessionsFromString(string sessionsString)
        {
            var sessionsDict = new Dictionary<long, Guid>();
            var sessions = sessionsString.Split(StorageEntrySeparator);
            if (sessions == null) return sessionsDict;

            foreach (var sessionString in sessions)
            {
                var splitSession = sessionString.Split(StorageKeyValueSeparator);
                try
                {
                    var time = long.Parse(splitSession[0]);
                    var sid = Guid.Parse(splitSession[1]);
                    sessionsDict.Add(time, sid);
                }
                catch (FormatException e) //TODO other exceptions?
                {
                    MobileCenterLog.Warn(Analytics.Instance.LogTag, $"Ignore invalid session in store: {sessionString}", e);
                }
            }
            return sessionsDict;
        }

        public void Pause()
        {
            lock (_lockObject)
            {
                MobileCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Pause");
                _lastPausedTime = TimeHelper.CurrentTimeInMilliseconds();
            }
        }

        public void Resume()
        {
            lock (_lockObject)
            {
                MobileCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Resume");
                _lastResumedTime = TimeHelper.CurrentTimeInMilliseconds();
                SendStartSessionIfNeeded();
            }
        }

        public void ClearSessions()
        {
            lock (_lockObject)
            {
                _applicationSettings.Remove(StorageKey);
            }
        }

        private void SendStartSessionIfNeeded()
        {
            if (_sid != null && !HasSessionTimedOut())
            {
                return;
            }

            if (_sessions.Count == StorageMaxSessions)
            {
                _sessions.Remove(_sessions.Keys.Min());
            }
            _sid = Guid.NewGuid();
            _sessions.Add(TimeHelper.CurrentTimeInMilliseconds(), _sid.Value);
            _applicationSettings[StorageKey] = SessionsAsString();
            var startSessionLog = new StartSessionLog {Sid = _sid};
            _channel.Enqueue(startSessionLog);
        }

        private bool HasSessionTimedOut()
        {
            var now = TimeHelper.CurrentTimeInMilliseconds();
            var noLogSentForLong = _lastQueuedLogTime == 0 || (now - _lastQueuedLogTime) >= SessionTimeout;
            if (_lastPausedTime == 0)
            {
                return _lastResumedTime == 0 && noLogSentForLong;
            }
            if (_lastResumedTime == 0)
            {
                return noLogSentForLong;
            }
            var isBackgroundForLong = (_lastPausedTime >= _lastResumedTime) && ((now - _lastPausedTime) >= SessionTimeout);
            var wasBackgroundForLong = (_lastResumedTime - Math.Max(_lastPausedTime, _lastQueuedLogTime)) >= SessionTimeout;
            MobileCenterLog.Debug(Analytics.Instance.LogTag, $"noLogSentForLong={noLogSentForLong} " +
                                                    $"isBackgroundForLong={isBackgroundForLong} " +
                                                    $"wasBackgroundForLong={wasBackgroundForLong}");
            return noLogSentForLong && (isBackgroundForLong || wasBackgroundForLong);
        }
    }
}
