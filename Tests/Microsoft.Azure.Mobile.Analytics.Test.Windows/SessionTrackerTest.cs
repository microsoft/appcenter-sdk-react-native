using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Analytics.Test.Windows
{
    [TestClass]
    public class SessionTrackerTest
    {
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannelUnit> _mockChannel;
        private SessionTracker _sessionTracker;

        [TestInitialize]
        public void InitializeSessionTrackerTest()
        {
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            DefaultApplicationSettings.Reset();
            _sessionTracker = new SessionTracker(_mockChannelGroup.Object, _mockChannel.Object);
            SessionTracker.SessionTimeout = 500;
        }

        /// <summary>
        /// Verify that the first call to resume sends a start session log
        /// </summary>
        [TestMethod]
        public void ResumeFirstTime()
        {
            _sessionTracker.Resume();

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Once());
        }

        /// <summary>
        /// Verify that after a timeout, the session tracker sends another start session log
        /// </summary>
        [TestMethod]
        public void ResumeAfterTimeout()
        {
            _sessionTracker.Resume();
            _sessionTracker.Pause();
            Task.Delay((int)SessionTracker.SessionTimeout).Wait();
            _sessionTracker.Resume();

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Exactly(2));
        }

        /// <summary>
        /// Verify that after a timeout, if we resume and send a log at the same time, only 1 new session is started
        /// </summary>
        [TestMethod]
        public void ResumeAfterTimeoutAndSendEvent()
        {
            _sessionTracker.Resume();
            _sessionTracker.Pause();
            Task.Delay((int)SessionTracker.SessionTimeout).Wait();
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Once());

            _sessionTracker.Resume();
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(new EventLog()));

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Exactly(2));
        }

        /// <summary>
        /// Verify that after a pause that is not long enough to be a timeout, the session tracker does not send a start session log
        /// </summary>
        [TestMethod]
        public void ResumeAfterShortPause()
        {
            _sessionTracker.Resume();
            _sessionTracker.Pause();
            _sessionTracker.Resume();

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Once());
        }

        /// <summary>
        /// Verify that ClearSessions actually removes sessions from storage
        /// </summary>
        [TestMethod]
        public void ClearSessions()
        {
            _sessionTracker.Resume();
            _sessionTracker.ClearSessions();

            Assert.IsTrue(DefaultApplicationSettings.IsEmpty);
        }

        /// <summary>
        /// Verify that an enqueuing log is handled properly while the tracker is in a session
        /// </summary>
        [TestMethod]
        public void HandleEnqueuingLogDuringSession()
        {
            _sessionTracker.Resume();
            var eventLog = new EventLog { Timestamp = DateTime.Now };
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(eventLog));
            Assert.IsNotNull(eventLog.Sid);
        }

        /// <summary>
        /// If two logs are enqueued during the same session, they should have the same session id
        /// </summary>
        [TestMethod]
        public void HandleEnueuingSecondLogDuringSession()
        {
            _sessionTracker.Resume();
            var time = DateTime.Now;
            var firstLog = new EventLog { Timestamp = time };
            var secondLog = new EventLog { Timestamp = time.AddMilliseconds(1) };
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(firstLog));
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, new EnqueuingLogEventArgs(secondLog));

            Assert.IsNotNull(secondLog.Sid);
            Assert.AreEqual(firstLog.Sid, secondLog.Sid);
        }

        /// <summary>
        /// Verify that an enqueuing log is adjusted and a session is started when a log is enqueued
        /// </summary>
        [TestMethod]
        public void HandleEnqueuingLogOutsideSession()
        {
            _sessionTracker.Pause();
            var eventLog = new EventLog { Name = "thisisaneventlog" };
            var eventArgs = new EnqueuingLogEventArgs(eventLog);
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, null, eventArgs);

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<StartSessionLog>()), Times.Once());
            Assert.IsNotNull(eventLog.Sid);
        }

        /// <summary>
        /// Verify that when a StartSessionLog is enqueued, a new session is not started
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
        /// Verify that there are never more than max sessions
        /// </summary>
        [TestMethod]
        public void StartMaxSessions()
        {
            for (var i = 0; i <= SessionTracker.StorageMaxSessions; ++i)
            {
                _sessionTracker.Pause();
                Task.Delay((int)SessionTracker.SessionTimeout).Wait();
                _sessionTracker.Resume();
            }

            Assert.IsTrue(_sessionTracker.NumSessions == SessionTracker.StorageMaxSessions);
        }

        /// <summary>
        /// Verify that creating a session dictionary from an invalid string returns a dictionary without the invalid sessions
        /// </summary>
        [TestMethod]
        public void SessionFromInvalidString()
        {
            var sessionString = "invalid session";
            var dict = SessionTracker.SessionsFromString(sessionString);

            Assert.IsTrue(dict.Count == 0);
        }

        /// <summary>
        /// Verify that creating a session dictionary from a null string returns an empty dictionary
        /// </summary>
        [TestMethod]
        public void SessionFromNullString()
        {
            var dict = SessionTracker.SessionsFromString(null);

            Assert.IsTrue(dict.Count == 0);
        }

        /// <summary>
        /// Verify that the session id of the session with the greatest toffset less than or equal to the log toffset is selected
        /// (when closest match is less than)
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
            Assert.AreEqual(intendedSid, log.Sid.Value);
        }

        /// <summary>
        /// Verify that the session id of the session with the greatest toffset less than or equal to the log toffset is selected
        /// (when closest match is equal to)
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
            Assert.AreEqual(intendedSid, log.Sid.Value);
        }

        /// <summary>
        /// Verify that when all session id toffsets are greater than that of the log, none is selected
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
        /// Verify session timeout is true if log was never sent and only pause has occurred 
        /// </summary>
        [TestMethod]
        public void HasSessionTimedOutPausedNeverResumed()
        {
            long now = 10;
            long lastQueuedLogTime = 0;
            long lastResumedTime = 0;
            long lastPausedTime = 5;

            Assert.IsTrue(SessionTracker.HasSessionTimedOut(now, lastQueuedLogTime, lastResumedTime, lastPausedTime));
        }

        /// <summary>
        /// Verify session timeout is false if session tracker was in background for long but a log was just sent
        /// </summary>
        [TestMethod]
        public void HasSessionTimedOutWasBackgroundForLongAndLogJustSent()
        {
            long now = 1000;
            long lastQueuedLogTime = 999;
            long lastResumedTime = 998;
            long lastPausedTime = 1;

            Assert.IsFalse(SessionTracker.HasSessionTimedOut(now, lastQueuedLogTime, lastResumedTime, lastPausedTime));
        }
    }
}
