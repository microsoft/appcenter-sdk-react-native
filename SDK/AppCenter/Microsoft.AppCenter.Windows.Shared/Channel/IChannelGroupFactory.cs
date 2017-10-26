namespace Microsoft.AppCenter.Channel
{
    public interface IChannelGroupFactory
    {
        IChannelGroup CreateChannelGroup(string appSecret);
    }
}
