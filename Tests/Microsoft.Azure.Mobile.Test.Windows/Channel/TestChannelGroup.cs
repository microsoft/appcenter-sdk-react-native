using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;

namespace Microsoft.Azure.Mobile.Test.Channel
{

    /* Custom class to expose protected constructor */
    public class TestChannelGroup : ChannelGroup
    {
        public TestChannelGroup(IStorage storage, IIngestion ingestion, string appSecret)
            : base(ingestion, storage, appSecret)
        {
        }

        public TestChannelGroup(string appSecret) : base(appSecret)
        {
        }
    }
}
