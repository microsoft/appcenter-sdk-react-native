using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Analytics.Channel;
using Microsoft.AppCenter.Analytics.Ingestion.Models;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Analytics.Test.Windows
{
    [TestClass]
    public class AnalyticsTest
    {
        private Mock<ISessionTracker> _mockSessionTracker;
        private ApplicationLifecycleHelper _applicationLifecycleHelper;
        private Mock<IChannelGroup> _mockChannelGroup;
        private Mock<IChannelUnit> _mockChannel;

        [TestInitialize]
        public void InitializeAnalyticsTest()
        {
            var factory = new SessionTrackerFactory();
            _mockSessionTracker = factory.ReturningSessionTrackerMock;
            _applicationLifecycleHelper = new ApplicationLifecycleHelper();
            _mockChannelGroup = new Mock<IChannelGroup>();
            _mockChannel = new Mock<IChannelUnit>();
            _mockChannelGroup.Setup(
                    group => group.AddChannel(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<int>()))
                .Returns(_mockChannel.Object);
            ApplicationLifecycleHelper.Instance = _applicationLifecycleHelper;
            Analytics.Instance = new Analytics(factory);
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
            Analytics.SetEnabledAsync(false).Wait();
            var valWhenFalse = Analytics.IsEnabledAsync().Result;
            Analytics.SetEnabledAsync(true).Wait();
            var valWhenTrue = Analytics.IsEnabledAsync().Result;

            Assert.IsFalse(valWhenFalse);
            Assert.IsTrue(valWhenTrue);
        }

        /// <summary>
        /// Verify that the session tracker is subscribed to the application lifecycle events as appropriate
        /// </summary>
        [TestMethod]
        public void SetupSessionTrackerEvents()
        {
            _applicationLifecycleHelper.InvokeSuspended();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            _applicationLifecycleHelper.InvokeSuspended();
            _applicationLifecycleHelper.InvokeResuming();

            _mockSessionTracker.Verify(tracker => tracker.Pause(), Times.Once());
            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Once());
        }

        /// <summary>
        /// Verify that Analytics starts the session tracker at startup even if the start event already occurred
        /// </summary>
        [TestMethod]
        public void StartAnalyticsAfterResumeWasInvoked()
        {
            _applicationLifecycleHelper.InvokeResuming();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Once());
        }

        /// <summary>
        /// Verify that Analytics does not start the session tracker at startup if the start event already occurred
        /// but lifecycle is in suspended state.
        /// </summary>
        [TestMethod]
        public void StartAnalyticsAfterResumeWasInvokedWhileSuspended()
        {
            _applicationLifecycleHelper.InvokeResuming();
            _applicationLifecycleHelper.InvokeSuspended();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Never());
        }

        /// <summary>
        /// Verify that Analytics starts the session tracker if application was started and suspended before
        /// OnChannelGroupReady, but is resumed after
        /// </summary>
        [TestMethod]
        public void StartAnalyticsAfterResumeWasInvokedAndNotSuspended()
        {
            _applicationLifecycleHelper.InvokeResuming();
            _applicationLifecycleHelper.InvokeSuspended();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            _applicationLifecycleHelper.InvokeResuming();

            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Once());
        }

        /// <summary>
        /// Verify that disabling channel disables lifecycle callbacks
        /// </summary>
        [TestMethod]
        public void SetEnabledFalse()
        {
            Analytics.SetEnabledAsync(false).Wait();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            _applicationLifecycleHelper.InvokeResuming();
            _applicationLifecycleHelper.InvokeSuspended();
            _applicationLifecycleHelper.InvokeResuming();

            _mockSessionTracker.Verify(tracker => tracker.Pause(), Times.Never());
            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Never());
        }

        /// <summary>
        /// Verify that re-enabling Analytics enables lifecycle callbacks
        /// </summary>
        [TestMethod]
        public void EnableAfterDisabling()
        {
            Analytics.SetEnabledAsync(false).Wait();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty); 
            Analytics.SetEnabledAsync(true).Wait();

            _applicationLifecycleHelper.InvokeSuspended();
            _applicationLifecycleHelper.InvokeResuming();

            _mockSessionTracker.Verify(tracker => tracker.Pause(), Times.Once());
            _mockSessionTracker.Verify(tracker => tracker.Resume(), Times.Exactly(2));
        }

        /// <summary>
        /// Verify that an event log is enqueued appropriately when TrackEvent is called
        /// </summary>
        [TestMethod]
        public void TrackEvent()
        {
            Analytics.SetEnabledAsync(true).Wait();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            var eventName = "eventName";
            var key = "key";
            var val = "val";
            var properties = new Dictionary<string, string> {{key, val}};
            Analytics.TrackEvent(eventName, properties);

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<EventLog>(log =>
                log.Name == eventName &&
                log.Properties != null &&
                log.Properties.Count == 1 &&
                log.Properties[key] == val)), Times.Once());
        }

        /// <summary>
        /// Verify that an event log is not enqueued when TrackEvent is called with invalid parameters
        /// </summary>
        [TestMethod]
        public void TrackEventInvalid()
        {
            Analytics.SetEnabledAsync(true).Wait();
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);

            // Event name is null or empty
            Analytics.TrackEvent(null);
            Analytics.TrackEvent("");
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<EventLog>()), Times.Never());

            // Event name exceeds max length
            Analytics.TrackEvent(new string('?', 257));
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<EventLog>(log =>
                log.Name.Length == 256)), Times.Once());

            // Without properties
            _mockChannel.ResetCalls();
            Analytics.TrackEvent("test", null);
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<EventLog>()), Times.Once());

            // Property key is null or empty 
            _mockChannel.ResetCalls();
            Analytics.TrackEvent("test", new Dictionary<string, string> { { "", "test" } });
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<EventLog>(log =>
                log.Properties == null || log.Properties.Count == 0)), Times.Once());

            // Property key length exceeds maximum
            _mockChannel.ResetCalls();
            Analytics.TrackEvent("test", new Dictionary<string, string> { { new string('?', 126), "test" } });
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<EventLog>(log =>
                log.Properties.First().Key.Length == 125)), Times.Once());
            
            // Property value is null
            _mockChannel.ResetCalls();
            Analytics.TrackEvent("test", new Dictionary<string, string> { { "test", null } });
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<EventLog>(log =>
                log.Properties == null || log.Properties.Count == 0)), Times.Once());

            // Property value length exceeds maximum
            _mockChannel.ResetCalls();
            Analytics.TrackEvent("test", new Dictionary<string, string> { { "test", new string('?', 126) } });
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<EventLog>(log =>
                log.Properties.First().Value.Length == 125)), Times.Once());

            // Properties size exceeds maximum
            _mockChannel.ResetCalls();
            var manyProperties = new Dictionary<string, string>();
            for (int i = 0; i < 21; i++)
            {
                manyProperties["test" + i] = "test" + i;
            }
            Analytics.TrackEvent("test", manyProperties);
            _mockChannel.Verify(channel => channel.EnqueueAsync(It.Is<EventLog>(log =>
                log.Properties.Count == 20)), Times.Once());
        }

        /// <summary>
        /// Verify that event logs are not sent when disabled
        /// </summary>
        [TestMethod]
        public void TrackEventWhileDisabled()
        {
            Analytics.Instance.OnChannelGroupReady(_mockChannelGroup.Object, string.Empty);
            Analytics.SetEnabledAsync(false).Wait();
            Analytics.TrackEvent("anevent");

            _mockChannel.Verify(channel => channel.EnqueueAsync(It.IsAny<EventLog>()), Times.Never());
        }
    }
}
