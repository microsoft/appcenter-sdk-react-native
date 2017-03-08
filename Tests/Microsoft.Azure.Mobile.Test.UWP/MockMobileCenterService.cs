using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using HyperMock;

namespace Microsoft.Azure.Mobile.Test
{
    public class MockMobileCenterService : IMobileCenterService
    {
        private static MockMobileCenterService _instanceField;

        public static MockMobileCenterService Instance
        {
            get
            {
                    return _instanceField ?? (_instanceField = new MockMobileCenterService());
            }
            set
            {
                    _instanceField = value;
            }
        }
        public Mock<IMobileCenterService> MockInstance { get; private set; }

        public MockMobileCenterService()
        {
            MockInstance = Mock.Create<IMobileCenterService>();
        }

        public bool InstanceEnabled {
            get
            {
                return MockInstance.Object.InstanceEnabled;
            }
             set
            {
                MockInstance.Object.InstanceEnabled = value;
            }

        }

        public void OnChannelGroupReady(ChannelGroup channelGroup)
        {
            MockInstance.Object.OnChannelGroupReady(channelGroup);
        }
    }
}
