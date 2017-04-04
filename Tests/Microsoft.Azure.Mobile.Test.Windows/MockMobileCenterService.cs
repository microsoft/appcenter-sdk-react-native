using Microsoft.Azure.Mobile.Channel;
using Moq;

namespace Microsoft.Azure.Mobile.Test
{
    public class MockMobileCenterService : IMobileCenterService
    {
        public string ServiceName => nameof(MockMobileCenterService);
        private static MockMobileCenterService _instanceField;

        public static void Reset()
        {
            _instanceField = new MockMobileCenterService();
        }
        public static MockMobileCenterService Instance => _instanceField ?? (_instanceField = new MockMobileCenterService());
        public Mock<IMobileCenterService> MockInstance { get; }

        public MockMobileCenterService()
        {
            MockInstance = new Mock<IMobileCenterService>();
        }

        public bool InstanceEnabled {
            get { return MockInstance.Object.InstanceEnabled; }
            set { MockInstance.Object.InstanceEnabled = value; }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup)
        {
            MockInstance.Object.OnChannelGroupReady(channelGroup);
        }
    }
}
