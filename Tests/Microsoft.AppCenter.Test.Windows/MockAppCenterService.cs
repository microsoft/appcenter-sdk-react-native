using Microsoft.AppCenter.Channel;
using Moq;

namespace Microsoft.AppCenter.Test
{
    public class MockAppCenterService : IAppCenterService
    {
        private static MockAppCenterService _instanceField;
        private readonly Mock<IAppCenterService> _mock;

        public string ServiceName => nameof(MockAppCenterService);

        public static void Reset()
        {
            _instanceField = new MockAppCenterService();
        }

        public static MockAppCenterService Instance => _instanceField ?? (_instanceField = new MockAppCenterService());
        public static Mock<IAppCenterService> Mock => Instance._mock;

        public MockAppCenterService()
        {
            _mock = new Mock<IAppCenterService>();
        }

        public bool InstanceEnabled {
            get { return _mock.Object.InstanceEnabled; }
            set { _mock.Object.InstanceEnabled = value; }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            _mock.Object.OnChannelGroupReady(channelGroup, appSecret);
        }
    }
}
