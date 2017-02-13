using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using System.Linq;

namespace Microsoft.Azure.Mobile.Analytics.Channel
{
    public class SessionTracker
    {
        private const string StorageKey = "sessions";
        private const int StorageMaxSessions = 5;
        private const char StorageKeyValueSeparator = '.';
        private const char StorageEntrySeparator = '/';
        private const long SessionTimeout = 20000;
        private ChannelGroup _channelGroup;
        private string _channelName;
        private Dictionary<long, Guid> _sessions = new Dictionary<long, Guid>();
        private Guid? _sid;
        private long? _lastQueuedLogTime;
        private long? _lastResumedTime;
        private long? _lastPausedTime;
        private ApplicationSettings _applicationSettings = new ApplicationSettings();
        private object _lockObject = new object();

        public SessionTracker(ChannelGroup channelGroup, string channelName)
        {
            _channelGroup = channelGroup;
            _channelName = channelName;
            _channelGroup.EnqueuingLog += HandleEnqueuingLog;

            string sessionsString = _applicationSettings.GetValue<string>(StorageKey, null);
            if (sessionsString == null)
            {
                return;
            }
            _sessions = SessionsFromString(sessionsString);
            if (_sessions.Count == 0)
            {
                return;
            }
            string loadedSessionsString = "Loaded stored sessions:\n";
            foreach (var session in _sessions.Values)
            {
                loadedSessionsString += "\t" + session + "\n";
            }
            MobileCenterLog.Debug(Analytics.LogTag, loadedSessionsString);

        }

        private void HandleEnqueuingLog(object sender, EnqueuingLogEventArgs e)
        {
            lock (_lockObject)
            {

                if (e.Log is StartSessionLog)
                {
                    return;
                }
                if (e.Log.Toffset > 0)
                {
                    long candidate = 0;
                    bool foundPrevSession = false;
                    //TODO change to linq?
                    foreach (long key in _sessions.Keys)
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
                if (e.Log.Sid == null)
                {
                    SendStartSessionIfNeeded();
                    e.Log.Sid = _sid;
                    _lastQueuedLogTime = TimeHelper.CurrentTimeInMilliseconds(); //TODO this?
                }
            }
        }

        public string SessionsAsString()
        {
            string sessionsString = "";
            foreach (var pair in _sessions)
            {
                if (sessionsString != "")
                {
                    sessionsString += StorageEntrySeparator;
                }
                sessionsString += pair.Key.ToString() + StorageKeyValueSeparator + pair.Value.ToString();
            }
            return sessionsString;
        }

        public Dictionary<long, Guid> SessionsFromString(string sessionsString)
        {
            var sessionsDict = new Dictionary<long, Guid>();
            string[] sessions = sessionsString.Split(StorageEntrySeparator);
            if (sessions == null)
            {
                return sessionsDict;
            }
            foreach (string sessionString in sessions)
            {
                string[] splitSession = sessionString.Split(StorageKeyValueSeparator);
                try
                {
                    long time = long.Parse(splitSession[0]);
                    Guid sid = Guid.Parse(splitSession[1]);
                    sessionsDict.Add(time, sid);
                }
                catch (Exception e)
                {
                    MobileCenterLog.Warn(Analytics.LogTag, $"Ignore invalid session in store: {sessionString}", e);
                }
            }
            return sessionsDict;
        }

        public void Pause()
        {
            lock (_lockObject)
            {
                MobileCenterLog.Debug(Analytics.LogTag, "SessionTracker.Pause");
                _lastPausedTime = TimeHelper.CurrentTimeInMilliseconds();
            }
        }

        public void Resume()
        {
            lock (_lockObject)
            {
                MobileCenterLog.Debug(Analytics.LogTag, "SessionTracker.Resume");
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
            StartSessionLog startSessionLog = new StartSessionLog();
            startSessionLog.Sid = _sid;
            _channelGroup.GetChannel(_channelName).Enqueue(startSessionLog);
        }

      
        private bool HasSessionTimedOut()
        {
            long now = TimeHelper.CurrentTimeInMilliseconds();
            bool noLogSentForLong = (now - _lastQueuedLogTime) >= SessionTimeout;

            if (_lastPausedTime == null)
            {
                return (_lastResumedTime == null) && noLogSentForLong;
            }

            if (_lastResumedTime == null)
            {
                return noLogSentForLong;
            }

            bool isBackgroundForLong = (_lastPausedTime >= _lastResumedTime) && ((now - _lastPausedTime) >= SessionTimeout);
            bool wasBackgroundForLong = (_lastResumedTime - Math.Max(_lastPausedTime.Value, _lastQueuedLogTime.Value)) >= SessionTimeout;
            MobileCenterLog.Debug(Analytics.LogTag, $"noLogSentForLong={noLogSentForLong} isBackgroundForLong={isBackgroundForLong} wasBackgroundForLong={wasBackgroundForLong}");
            return noLogSentForLong && (isBackgroundForLong || wasBackgroundForLong);
        }
    }
}
