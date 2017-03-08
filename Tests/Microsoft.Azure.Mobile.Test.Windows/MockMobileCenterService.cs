using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using Moq;

namespace Microsoft.Azure.Mobile.Test
{
    public class MockMobileCenterService : IMobileCenterService
    {
        private static MockMobileCenterService _instanceField;

        public static MockMobileCenterService Instance
        {
            get => _instanceField ?? (_instanceField = new MockMobileCenterService());
            set => _instanceField = value;
        }
        public Mock<IMobileCenterService> MockInstance { get; }

        public MockMobileCenterService()
        {
            MockInstance = new Mock<IMobileCenterService>();
        }

        public bool InstanceEnabled {
            get => MockInstance.Object.InstanceEnabled;
            set => MockInstance.Object.InstanceEnabled = value;
        }

        public void OnChannelGroupReady(ChannelGroup channelGroup)
        {
            MockInstance.Object.OnChannelGroupReady(channelGroup);
        }
    }
}
