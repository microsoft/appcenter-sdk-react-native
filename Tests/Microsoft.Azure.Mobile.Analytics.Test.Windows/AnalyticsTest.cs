using System;
using System.Collections.Generic;
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
    public class AnalyticsTest
    {
        private Mock<ISessionTracker> _mockSessionTracker;
        private Mock<IApplicationLifecycleHelper> _mockApplicationLifecycle;
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannelUnit> _mockChannel;

        [TestInitialize]
        public void InitializeAnalyticsTest()
        {
            var factory = new SessionTrackerFactory();
            _mockSessionTracker = factory.ReturningSessionTrackerMock;
            _mockApplicationLifecycle = new Mock<IApplicationLifecycleHelper>();
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            Analytics.Instance = new Analytics(factory, _mockApplicationLifecycle.Object);
        }
        
        /// <summary>
        /// Verify that a null instance is not retrieved
        /// </summary>
        [TestMethod]
        public void InstanceIsNotNull()
        {
            Analytics.Instance = null;

            Assert.IsNotNull(Analytics.Instance);
        }

        /// <summary>
        /// Verify that enabled value is correctly retrieved
        /// </summary>
        [TestMethod]
        public void GetEnabled()
        {
            Analytics.Enabled = false;
            var valWhenFalse = Analytics.Enabled;
            Analytics.Enabled = true;
            var valWhenTrue = Analytics.Enabled;

            Assert.IsFalse(valWhenFalse);
            Assert.IsTrue(valWhenTrue);
        }

        /// <summary>
        /// Verify that the session tracker is subscribed to the application lifecycle events as appropriate
        /// </summary>
        [TestMethod]
        public void SetupSessionTrackerEvents()
        {
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object);
            _mockApplicationLifecycle.Raise(lifecycle => lifecycle.ApplicationSuspended += null, null, null);
            _mockApplicationLifecycle.Raise(lifecycle => lifecycle.ApplicationResuming += null, null, null);

            _mockSessionTracker.Verify(tracker => tracker.Pause(), Times.Once());
            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Once());
        }

        /// <summary>
        /// Verify that disabling channel disables lifecycle callbacks
        /// </summary>
        [TestMethod]
        public void SetEnabledFalse()
        {
            Analytics.Enabled = false;
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object);
            _mockApplicationLifecycle.Raise(lifecycle => lifecycle.ApplicationSuspended += null, null, null);
            _mockApplicationLifecycle.Raise(lifecycle => lifecycle.ApplicationResuming += null, null, null);

            _mockSessionTracker.Verify(tracker => tracker.Pause(), Times.Never());
            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Never());
        }

        /// <summary>
        /// Verify that re-enabling Analytics enables lifecycle callbacks
        /// </summary>
        [TestMethod]
        public void EnableAfterDisabling()
        {
            Analytics.Enabled = false;
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object);
            Analytics.Enabled = true;

            _mockApplicationLifecycle.Raise(lifecycle => lifecycle.ApplicationSuspended += null, null, null);
            _mockApplicationLifecycle.Raise(lifecycle => lifecycle.ApplicationResuming += null, null, null);


            _mockSessionTracker.Verify(tracker => tracker.Pause(), Times.Once());
            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Once());
        }

        /// <summary>
        /// Verify that an event log is enqueued appropriately when TrackEvent is called
        /// </summary>
        [TestMethod]
        public void TrackEvent()
        {
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object);
            var eventName = "eventName";
            var key = "key";
            var val = "val";
            var properties = new Dictionary<string, string> {{key, val}};
            Analytics.TrackEvent(eventName, properties);

            _mockChannel.Verify(channel => channel.Enqueue(It.Is<EventLog>(log =>
                log.Name == eventName &&
                log.Properties != null &&
                log.Properties.Count == 1 &&
                log.Properties[key] == val)), Times.Once());
        }

        /// <summary>
        /// Verify that event logs are not sent when disabled
        /// </summary>
        [TestMethod]
        public void TrackEventWhileDisabled()
        {
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object);
            Analytics.Enabled = false;
            Analytics.TrackEvent("anevent");

            _mockChannel.Verify(channel => channel.Enqueue(It.IsAny<EventLog>()), Times.Never());
        }
    }
}
