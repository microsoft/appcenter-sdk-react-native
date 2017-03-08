using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test
{
    [TestClass]
    public class MobileCenterTest
    {
        [TestMethod]
        public void StartInstanceWithConfigure()
        {
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(service => service.OnChannelGroupReady(It.IsAny<ChannelGroup>()), Times.Never());
        }

        [TestMethod]
        public void StartInstanceWithoutConfigure()
        {
            var mockService = new Mock<IMobileCenterService>();
            MobileCenter.Configure("appsecret");
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(service => service.OnChannelGroupReady(It.IsAny<ChannelGroup>()), Times.Once());
        }
    }
}
