using System;
using Microsoft.Azure.Mobile.Analytics.Channel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Analytics.Test.Windows
{
    [TestClass]
    public class AnalyticsTest
    {
        private Mock<SessionTracker> _mockSessionTracker;
        
        [TestInitialize]
        public void InitializeAnalyticsTest()
        {
            var factory = new SessionTrackerFactory();
            _mockSessionTracker = factory.ReturningSessionTrackerMock;
            var analyticsInstance = new Analytics(factory);
            //Analytics.Instance = analyticsInstance;
        }

        [TestMethod]
        public void SetEnabled()
        {
        }
    }
}
