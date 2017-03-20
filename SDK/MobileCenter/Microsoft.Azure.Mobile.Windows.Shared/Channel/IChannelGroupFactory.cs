namespace Microsoft.Azure.Mobile.Channel
{
    public interface IChannelGroupFactory
    {
        IChannelGroup CreateChannelGroup(string appSecret);
    }
}
