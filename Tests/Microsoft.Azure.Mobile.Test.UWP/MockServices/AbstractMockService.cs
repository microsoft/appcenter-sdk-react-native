using Windows.ApplicationModel.Activation;
using Microsoft.Azure.Mobile.Channel;
using Moq;

namespace Microsoft.Azure.Mobile.Test.MockServices
{
    public abstract class AbstractMockService : IMobileCenterService
    {
        public Mock<IMobileCenterService> MockServiceInstance;

        public void NotifyOnLaunched(LaunchActivatedEventArgs e)
        {
            MockServiceInstance.Object.NotifyOnLaunched(e);
        }

        public string ServiceName => MockServiceInstance.Object.ServiceName;

        public bool InstanceEnabled
        {
            get { return MockServiceInstance.Object.InstanceEnabled; }
            set { MockServiceInstance.Object.InstanceEnabled = value; }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            MockServiceInstance.Object.OnChannelGroupReady(channelGroup, appSecret);
        }
    }
}
