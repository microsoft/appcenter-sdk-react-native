// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics.Channel;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Windows.Shared.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Analytics.Test.Windows
{
    [TestClass]
    public class SessionTrackerTest
    {
        private Mock<IChannelUnit> _mockChannel;
        private Mock<IChannelGroup> _mockChannelGroup;
        private SessionTracker _sessionTracker;

        [TestInitialize]
        public void InitializeSessionTrackerTest()
        {
            SessionContext.SessionId = null;
            SessionTracker.SessionTimeout = 500; // 0.5s
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(),
                        It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            _sessionTracker = new SessionTracker(_mockChannelGroup.Object, _mockChannel.Object);
        }

        /// <summary>
        ///     Verify that the first call to resume sends a start session log
        /// </summary>
        [TestMethod]
        public void ResumeFirstTime()
        {
            Log actualLog = null;
            _mockChannel.Setup(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>())).Callback<Log>(log => actualLog = log);
            _sessionTracker.Resume();
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsNotNull<StartSessionLog>()), Times.Once());
            Assert.IsNotNull(actualLog.Sid);
        }

        /// <summary>
        ///     Verify that after a timeout, the session tracker sends another start session log
        /// </summary>
        [TestMethod]
        public void ResumeAfterTimeout()
        {
            Log actualLog = null;
            _mockChannel.Setup(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>())).Callback<Log>(log => actualLog = log);

            _sessionTracker.Resume();
            var firstLog = actualLog;

            _sessionTracker.Pause();
            Task.Delay((int)SessionTracker.SessionTimeout * 2).Wait();
            _sessionTracker.Resume();
            var secondLog = actualLog;

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsNotNull<StartSessionLog>()), Times.Exactly(2));
            Assert.IsNotNull(firstLog.Sid);
            Assert.IsNotNull(secondLog.Sid);
            Assert.AreNotEqual(firstLog.Sid, secondLog.Sid);
        }

        /// <summary>
        ///     Verify that after a timeout, if we resume and send a log at the same time, only 1 new session is started
        /// </summary>
        [TestMethod]
        public void ResumeAfterTimeoutAndSendEvent()
        {
            Log actualLog = null;
            _mockChannel.Setup(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>())).Callback<Log>(log => actualLog = log);

            _sessionTracker.Resume();
            var firstSessionLog = actualLog;

            _sessionTracker.Pause();
            Task.Delay((int)SessionTracker.SessionTimeout * 2).Wait();
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsNotNull<StartSessionLog>()), Times.Once());

            _sessionTracker.Resume();
            var secondSessionLog = actualLog;

            var eventLog = new EventLog();
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null,
                new EnqueuingLogEventArgs(eventLog));

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsNotNull<StartSessionLog>()), Times.Exactly(2));
            Assert.IsNotNull(firstSessionLog.Sid);
            Assert.IsNotNull(secondSessionLog.Sid);
            Assert.AreNotEqual(firstSessionLog.Sid, secondSessionLog.Sid);
            Assert.AreEqual(secondSessionLog.Sid, eventLog.Sid);
        }

        /// <summary>
        ///     Verify that after a pause that is not long enough to be a timeout, the session tracker does not send a start
        ///     session log
        /// </summary>
        [TestMethod]
        public void ResumeAfterShortPause()
        {
            // Make a first session.
            _sessionTracker.Resume();
            var firstSessionId = SessionContext.SessionId;

            // Short pause and resume.
            _sessionTracker.Pause();
            _sessionTracker.Resume();

            // There is only 1 session.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Once());
            Assert.IsNotNull(firstSessionId);
            Assert.AreEqual(firstSessionId, SessionContext.SessionId);
        }

        /// <summary>
        ///     Verify that an enqueuing log is handled properly while the tracker is in a session
        /// </summary>
        [TestMethod]
        public void HandleEnqueuingLogDuringSession()
        {
            _sessionTracker.Resume();
            var eventLog = new EventLog();
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(eventLog));
            Assert.IsNotNull(eventLog.Sid);
            Assert.AreNotEqual(Guid.Empty, eventLog.Sid);
        }

        /// <summary>
        ///     Verify that an enqueuing log with already specified timestamp is ignored from session tracker, like crash logs.
        /// </summary>
        [TestMethod]
        public void HandleEnqueuingLogWithExplicitTimestamp()
        {
            _sessionTracker.Resume();
            var eventLog = new EventLog { Timestamp = DateTime.Now };
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(eventLog));
            Assert.IsNull(eventLog.Sid);
        }

        /// <summary>
        ///     If two logs are enqueued during the same session, they should have the same session id
        /// </summary>
        [TestMethod]
        public void HandleEnqueingSecondLogDuringSession()
        {
            _sessionTracker.Resume();
            var firstLog = new EventLog();
            var secondLog = new EventLog();
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(firstLog));
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(secondLog));

            Assert.IsNotNull(secondLog.Sid);
            Assert.AreNotEqual(Guid.Empty, secondLog.Sid);
            Assert.AreEqual(firstLog.Sid, secondLog.Sid);
        }

        /// <summary>
        ///     Verify that an enqueuing log is adjusted and a session is not started when a log is enqueued
        /// </summary>
        [TestMethod]
        public void HandleEnqueuingLogOutsideSession()
        {
            _sessionTracker.Pause();
            var eventLog = new EventLog { Name = "thisisaneventlog" };
            var eventArgs = new EnqueuingLogEventArgs(eventLog);
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, eventArgs);

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Never());
            Assert.IsNull(eventLog.Sid);

            // Make sure a session starts after this.
            _sessionTracker.Resume();
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Once());
        }

        /// <summary>
        ///     Verify that when a StartSessionLog is enqueued, a new session is not started
        /// </summary>
        [TestMethod]
        public void HandleEnqueuingStartSessionLog()
        {
            _sessionTracker.Pause();
            var sessionLog = new StartSessionLog();
            var eventArgs = new EnqueuingLogEventArgs(sessionLog);
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, eventArgs);

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Never());
        }

        /// <summary>
        ///     Verify that the session id of the session with the greatest toffset less than or equal to the log toffset is
        ///     selected
        ///     (when closest match is less than)
        /// </summary>
        [TestMethod]
        public void TestSetSessionIdLessThan()
        {
            var time = new DateTime(430 * TimeSpan.TicksPerMillisecond);
            var log = new EventLog { Timestamp = time };
            var intendedSid = Guid.NewGuid();
            var sessions = new Dictionary<long, Guid> { { 4, Guid.Empty }, { 429, intendedSid }, { 431, Guid.Empty } };
            var success = SessionTracker.SetExistingSessionId(log, sessions);

            Assert.IsTrue(success);
            Assert.AreEqual(intendedSid, log.Sid);
        }

        /// <summary>
        ///     Verify that the session id of the session with the greatest toffset less than or equal to the log toffset is
        ///     selected
        ///     (when closest match is equal to)
        /// </summary>
        [TestMethod]
        public void TestSetSessionIdEqualTo()
        {
            var time = new DateTime(430 * TimeSpan.TicksPerMillisecond);
            var log = new EventLog { Timestamp = time };
            var intendedSid = Guid.NewGuid();
            var sessions = new Dictionary<long, Guid> { { 4, Guid.Empty }, { 430, intendedSid }, { 431, Guid.Empty } };
            var success = SessionTracker.SetExistingSessionId(log, sessions);

            Assert.IsTrue(success);
            Assert.AreEqual(intendedSid, log.Sid);
        }

        /// <summary>
        ///     Verify that when all session id toffsets are greater than that of the log, none is selected
        /// </summary>
        [TestMethod]
        public void TestSetSessionIdNone()
        {
            var time = new DateTime(430 * TimeSpan.TicksPerMillisecond);
            var log = new EventLog { Timestamp = time };
            var sessions = new Dictionary<long, Guid> { { 431, Guid.Empty }, { 632, Guid.Empty }, { 461, Guid.Empty } };
            var success = SessionTracker.SetExistingSessionId(log, sessions);

            Assert.IsFalse(success);
            Assert.IsFalse(log.Sid.HasValue);
        }

        /// <summary>
        ///     Verify session timeout is false if session tracker was in background for long but a log was just sent
        /// </summary>
        [TestMethod]
        public void HasSessionTimedOutWasBackgroundForLongAndLogJustSent()
        {
            const long now = 1000;
            const long lastQueuedLogTime = 999;
            const long lastResumedTime = 998;
            const long lastPausedTime = 1;

            Assert.IsFalse(SessionTracker.HasSessionTimedOut(now, lastQueuedLogTime, lastResumedTime, lastPausedTime));
        }

        [TestMethod]
        public void VerifySessionChangesOnDisableThenEnable()
        {
            // Do a first session.
            Log actualLog = null;
            _mockChannel.Setup(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>())).Callback<Log>(log => actualLog = log);
            _sessionTracker.Resume();
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsNotNull<StartSessionLog>()), Times.Once());
            var firstLog = actualLog;
            Assert.IsNotNull(actualLog.Sid);
            Assert.AreEqual(SessionContext.SessionId, actualLog.Sid);

            // Disable.
            _sessionTracker.Stop();
            Assert.IsNull(SessionContext.SessionId);

            // Re-enable.
            _sessionTracker = new SessionTracker(_mockChannelGroup.Object, _mockChannel.Object);
            _sessionTracker.Resume();

            // Verify second session has a different identifier, not the new one Analytics wanted but the updated correlation identifier instead.
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsNotNull<StartSessionLog>()), Times.Exactly(2));
            var secondLog = actualLog;
            Assert.IsNotNull(secondLog.Sid);
            Assert.AreNotEqual(firstLog.Sid, secondLog.Sid);
        }
    }
}
