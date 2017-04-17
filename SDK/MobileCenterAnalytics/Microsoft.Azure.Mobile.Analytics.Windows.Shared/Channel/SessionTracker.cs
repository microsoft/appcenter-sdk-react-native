using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;
using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Linq;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Analytics.Channel
{
    internal class SessionTracker : ISessionTracker
    {
        // Represents the state of a session
        private enum SessionState
        {
            Inactive,
            Active,
            None
        }

        // Need to track the current state because if not, successive calls to resume could cause
        // a session to be started multiple times. Worse, if the calls occur in rapid succession,
        // the session entries could have the same time in milliseconds, causing an exception to
        // be thrown.
        //
        // TL;DR - it is incorrect to resume an active session, and to pause an inactive session,
        // so we track the current session state.
        private SessionState _currentSessionState = SessionState.None;

        // Some fields are internal for testing
        internal static long SessionTimeout = 20000;
        internal const int StorageMaxSessions = 5;
        internal const string StorageKey = "MobileCenterSessions";
        private const char StorageKeyValueSeparator = '.';
        private const char StorageEntrySeparator = '/';
        private readonly IChannelUnit _channel;
        private readonly Dictionary<long, Guid> _sessions = new Dictionary<long, Guid>();
        private Guid? _sid;
        private long _lastQueuedLogTime;
        private long _lastResumedTime;
        private long _lastPausedTime;
        private readonly ApplicationSettings _applicationSettings = new ApplicationSettings();
        private readonly object _lockObject = new object();

        // This field is purely for testing
        internal int NumSessions => _sessions.Count;

        public SessionTracker(IChannelGroup channelGroup, IChannelUnit channel)
        {
            _channel = channel;
            channelGroup.EnqueuingLog += HandleEnqueuingLog;
            var sessionsString = _applicationSettings.GetValue<string>(StorageKey, null);
            if (sessionsString == null) return;
            _sessions = SessionsFromString(sessionsString);
            // Re-write sessions in storage in case of any invalid strings
            _applicationSettings[StorageKey] = SessionsAsString();
            if (_sessions.Count == 0) return;
            var loadedSessionsString = _sessions.Values.Aggregate("Loaded stored sessions:\n", (current, session) => current + ("\t" + session + "\n"));
            MobileCenterLog.Debug(Analytics.Instance.LogTag, loadedSessionsString);
        }

        public void Pause()
        {
            lock (_lockObject)
            {
                if (_currentSessionState == SessionState.Inactive)
                {
                    MobileCenterLog.Warn(Analytics.Instance.LogTag, "Trying to pause already inactive session.");
                    return;
                }
                MobileCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Pause");
                _lastPausedTime = TimeHelper.CurrentTimeInMilliseconds();
                _currentSessionState = SessionState.Inactive;
            }
        }

        public void Resume()
        {
            lock (_lockObject)
            {
                if (_currentSessionState == SessionState.Active)
                {
                    MobileCenterLog.Warn(Analytics.Instance.LogTag, "Trying to resume already active session.");
                    return;
                }
                MobileCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Resume");
                _lastResumedTime = TimeHelper.CurrentTimeInMilliseconds();
                _currentSessionState = SessionState.Active;
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

        private void HandleEnqueuingLog(object sender, EnqueuingLogEventArgs e)
        {
            lock (_lockObject)
            {
                // Skip StartSessionLogs to avoid an infinite loop
                // Skip StartServiceLog because enqueuing a startservicelog should not trigger the start of a session
                if (e.Log is StartSessionLog || e.Log is StartServiceLog)
                {
                    return;
                }
                if (SetExistingSessionId(e.Log, _sessions))
                {
                    return;
                }
                SendStartSessionIfNeeded();
                e.Log.Sid = _sid;
                _lastQueuedLogTime = TimeHelper.CurrentTimeInMilliseconds();
            }
        }

        private void SendStartSessionIfNeeded()
        {
            var now = TimeHelper.CurrentTimeInMilliseconds();
            if (_sid != null && !HasSessionTimedOut(now))
            {
                return;
            }

            if (_sessions.Count == StorageMaxSessions)
            {
                _sessions.Remove(_sessions.Keys.Min());
            }
            _sid = Guid.NewGuid();
            _sessions.Add(now, _sid.Value);
            _applicationSettings[StorageKey] = SessionsAsString();
            var startSessionLog = new StartSessionLog { Sid = _sid };
            _channel.Enqueue(startSessionLog);
        }

        private string SessionsAsString()
        {
            var sessionsString = "";
            foreach (var pair in _sessions)
            {
                if (sessionsString != "") sessionsString += StorageEntrySeparator;
                sessionsString += pair.Key.ToString() + StorageKeyValueSeparator + pair.Value;
            }
            return sessionsString;
        }

        internal static Dictionary<long, Guid> SessionsFromString(string sessionsString)
        {
            var sessionsDict = new Dictionary<long, Guid>();
            if (sessionsString == null) return sessionsDict;
            var sessions = sessionsString.Split(StorageEntrySeparator);

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

        private bool HasSessionTimedOut(long now)
        {
            return HasSessionTimedOut(now, _lastQueuedLogTime, _lastResumedTime, _lastPausedTime);
        }

        // Internal and static so that it can be tested more easily
        internal static bool HasSessionTimedOut(long now, long lastQueuedLogTime, long lastResumedTime, long lastPausedTime)
        {
            var noLogSentForLong = lastQueuedLogTime == 0 || (now - lastQueuedLogTime) >= SessionTimeout;
            if (lastPausedTime == 0)
            {
                return lastResumedTime == 0 && noLogSentForLong;
            }
            if (lastResumedTime == 0)
            {
                return noLogSentForLong;
            }
            var isBackgroundForLong = (lastPausedTime >= lastResumedTime) && ((now - lastPausedTime) >= SessionTimeout);
            var wasBackgroundForLong = (lastResumedTime - Math.Max(lastPausedTime, lastQueuedLogTime)) >= SessionTimeout;
            MobileCenterLog.Debug(Analytics.Instance.LogTag, $"noLogSentForLong={noLogSentForLong} " +
                                                    $"isBackgroundForLong={isBackgroundForLong} " +
                                                    $"wasBackgroundForLong={wasBackgroundForLong}");
            return noLogSentForLong && (isBackgroundForLong || wasBackgroundForLong);
        }

        internal static bool SetExistingSessionId(Log log, IDictionary<long, Guid> sessions)
        {
            if (log.Toffset <= 0)
            {
                return false;
            }
                        var key = sessions.Keys.Where(num => num <= log.Toffset).DefaultIfEmpty(-1).Max();
            if (key == -1)
            {
                return false;
            }
            log.Sid = sessions[key];
            return true;
        }
    }
}
