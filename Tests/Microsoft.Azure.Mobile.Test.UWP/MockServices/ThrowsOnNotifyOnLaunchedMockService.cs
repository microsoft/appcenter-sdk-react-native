using Windows.ApplicationModel.Activation;
using Moq;

namespace Microsoft.Azure.Mobile.Test.MockServices
{
    public class ThrowsOnNotifyOnLaunchedMockService : AbstractMockService
    {
        public ThrowsOnNotifyOnLaunchedMockService()
        {
            MockServiceInstance = new Mock<IMobileCenterService>();
            MockServiceInstance.Setup(service => service.NotifyOnLaunched(It.IsAny<LaunchActivatedEventArgs>()))
                .Throws(new MobileCenterException("error"));
        }

        private static IMobileCenterService _instance;
        public static IMobileCenterService Instance
        {
            get { return _instance ?? (_instance = new ThrowsOnNotifyOnLaunchedMockService()); }
        }
    }
}
