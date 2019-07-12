// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Windows.Shared.Utils;

// ReSharper disable once CheckNamespace
namespace Microsoft.AppCenter.Analytics.Channel
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
        private readonly IChannelUnit _channel;

        private long _lastQueuedLogTime;
        private long _lastResumedTime;
        private long _lastPausedTime;
        private readonly object _lockObject = new object();

        public SessionTracker(IChannel channelGroup, IChannelUnit channel)
        {
            // Need to lock in constructor because of the event handler being set for channelGroup.
            lock (_lockObject)
            {
                _channel = channel;
                channelGroup.EnqueuingLog += HandleEnqueuingLog;
            }
        }

        public void Pause()
        {
            lock (_lockObject)
            {
                if (_currentSessionState == SessionState.Inactive)
                {
                    AppCenterLog.Warn(Analytics.Instance.LogTag, "Trying to pause already inactive session.");
                    return;
                }
                AppCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Pause");
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
                    AppCenterLog.Warn(Analytics.Instance.LogTag, "Trying to resume already active session.");
                    return;
                }
                AppCenterLog.Debug(Analytics.Instance.LogTag, "SessionTracker.Resume");
                _lastResumedTime = TimeHelper.CurrentTimeInMilliseconds();
                _currentSessionState = SessionState.Active;
                SendStartSessionIfNeeded();
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                SessionContext.SessionId = null;
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

                // Correlate current session only to logs not specifying timestamps.
                // Crash before restart when Analytics was disabled at that time must not have session identifier.
                if (e.Log.Timestamp == null)
                {
                    e.Log.Sid = SessionContext.SessionId;
                }

                _lastQueuedLogTime = TimeHelper.CurrentTimeInMilliseconds();
            }
        }

        private void SendStartSessionIfNeeded()
        {
            var now = TimeHelper.CurrentTimeInMilliseconds();
            if (SessionContext.SessionId != null && !HasSessionTimedOut(now))
            {
                return;
            }

            SessionContext.SessionId = Guid.NewGuid();
            _lastQueuedLogTime = TimeHelper.CurrentTimeInMilliseconds();
            var startSessionLog = new StartSessionLog { Sid = SessionContext.SessionId };
            _channel.EnqueueAsync(startSessionLog);
        }

        private bool HasSessionTimedOut(long now)
        {
            return HasSessionTimedOut(now, _lastQueuedLogTime, _lastResumedTime, _lastPausedTime);
        }

        // Internal and static so that it can be tested more easily
        internal static bool HasSessionTimedOut(long now, long lastQueuedLogTime, long lastResumedTime, long lastPausedTime)
        {
            if (lastPausedTime == 0)
            {
                return false;
            }
            var noLogSentForLong = lastQueuedLogTime == 0 || now - lastQueuedLogTime >= SessionTimeout;
            var wasBackgroundForLong = lastResumedTime - Math.Max(lastPausedTime, lastQueuedLogTime) >= SessionTimeout;
            AppCenterLog.Debug(Analytics.Instance.LogTag, $"noLogSentForLong={noLogSentForLong} " +
                                                    $"wasBackgroundForLong={wasBackgroundForLong}");
            return noLogSentForLong && wasBackgroundForLong;
        }

        internal static bool SetExistingSessionId(Log log, IDictionary<long, Guid> sessions)
        {
            if (log.Timestamp == null)
            {
                return false;
            }
            var logTime = log.Timestamp.Value.Ticks / TimeSpan.TicksPerMillisecond;
            var key = sessions.Keys.Where(num => num <= logTime).DefaultIfEmpty(-1).Max();
            if (key == -1)
            {
                return false;
            }
            log.Sid = sessions[key];
            return true;
        }
    }
}
