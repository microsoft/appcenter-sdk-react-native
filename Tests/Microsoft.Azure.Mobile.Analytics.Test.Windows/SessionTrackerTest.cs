using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.Azure.Mobile.Analytics.Ingestion.Models;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Analytics.Test.Windows
{
    [TestClass]
    public class SessionTrackerTest
    {
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannel> _mockChannel;
        private SessionTracker _sessionTracker;

        [TestInitialize]
        public void InitializeSessionTrackerTest()
        {
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannel>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            _sessionTracker = new SessionTracker(_mockChannelGroup.Object, _mockChannel.Object);
            SessionTracker.SessionTimeout = 500;
            ApplicationSettings.Reset();
        }


        /// <summary>
        /// Verify that the first call to resume sends a start session log
        /// </summary>
        [TestMethod]
        public void ResumeFirstTime()
        {
            _sessionTracker.Resume();

            _mockChannel.Verify(channel => channel.Enqueue(It.IsAny<StartSessionLog>()), Times.Once());
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

            _mockChannel.Verify(channel => channel.Enqueue(It.IsAny<StartSessionLog>()), Times.Exactly(2));
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

            _mockChannel.Verify(channel => channel.Enqueue(It.IsAny<StartSessionLog>()), Times.Once());
        }

        /// <summary>
        /// Verify that ClearSessions actually removes sessions from storage
        /// </summary>
        [TestMethod]
        public void ClearSessions()
        {
            _sessionTracker.Resume();
            _sessionTracker.ClearSessions();

            Assert.IsTrue(ApplicationSettings.IsEmpty);
        }

        /// <summary>
        /// Verify that an enqueuing log is handled properly while the tracker is in a session
        /// </summary>
        [TestMethod]
        public void HandleEnqueuingLogDuringSession()
        {
            _sessionTracker.Resume();
            var eventLog = new EventLog {Name = "thisisaneventlog"};
            var eventArgs = new EnqueuingLogEventArgs(eventLog);
            _mockChannelGroup.Raise(group => group.EnqueuingLog += null, eventArgs);

            //verify that sid was set
            Assert.IsNotNull(eventLog.Sid);
        }
    }
}
