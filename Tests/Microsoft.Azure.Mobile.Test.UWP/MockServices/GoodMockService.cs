using Moq;

namespace Microsoft.Azure.Mobile.Test.MockServices
{
    public class GoodMockService : AbstractMockService
    {
        public GoodMockService()
        {
            MockServiceInstance = new Mock<IMobileCenterService>();
        }

        private static GoodMockService _instance;
        public static GoodMockService Instance
        {
            get { return _instance ?? (_instance = new GoodMockService()); }
        }
    }
}
