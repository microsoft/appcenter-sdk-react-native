using Microsoft.AppCenter.Channel;
using Moq;

namespace Microsoft.AppCenter.Test
{
    public class MockAppCenterService : IAppCenterService
    {
        public string ServiceName => nameof(MockAppCenterService);
        private static MockAppCenterService _instanceField;

        public static void Reset()
        {
            _instanceField = new MockAppCenterService();
        }
        public static MockAppCenterService Instance => _instanceField ?? (_instanceField = new MockAppCenterService());
        public Mock<IAppCenterService> MockInstance { get; }

        public MockAppCenterService()
        {
            MockInstance = new Mock<IAppCenterService>();
        }

        public bool InstanceEnabled {
            get { return MockInstance.Object.InstanceEnabled; }
            set { MockInstance.Object.InstanceEnabled = value; }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            MockInstance.Object.OnChannelGroupReady(channelGroup, appSecret);
        }
    }
}
