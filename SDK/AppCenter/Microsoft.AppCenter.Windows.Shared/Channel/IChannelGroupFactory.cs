using Microsoft.AppCenter.Ingestion.Http;

namespace Microsoft.AppCenter.Channel
{
    public interface IChannelGroupFactory
    {
        IChannelGroup CreateChannelGroup(string appSecret, INetworkStateAdapter networkState);
    }
}
