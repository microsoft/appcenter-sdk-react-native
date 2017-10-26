namespace Microsoft.AppCenterChannel
{
    public interface IChannelGroupFactory
    {
        IChannelGroup CreateChannelGroup(string appSecret);
    }
}
