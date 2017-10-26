namespace Microsoft.AAppCenterChannel
{
    public interface IChannelGroupFactory
    {
        IChannelGroup CreateChannelGroup(string appSecret);
    }
}
