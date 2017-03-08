using System;
using System.Collections.Generic;
using System.Linq;
using HyperMock;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Microsoft.Azure.Mobile.Test
{
    [TestClass]
    public class MobileCenterTest
    {
        [TestMethod]
        public void StartInstanceWithConfigure()
        {
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(service => service.OnChannelGroupReady(Param.IsAny<ChannelGroup>()), Occurred.Never());
        }

        [TestMethod]
        public void StartInstanceWithoutConfigure()
        {
            var mockService = Mock.Create<IMobileCenterService>();
            MobileCenter.Configure("appsecret");
            MobileCenter.Start(typeof(MockMobileCenterService));
            MockMobileCenterService.Instance.MockInstance.Verify(service => service.OnChannelGroupReady(Param.IsAny<ChannelGroup>()), Occurred.Once());
        }
    }
}
