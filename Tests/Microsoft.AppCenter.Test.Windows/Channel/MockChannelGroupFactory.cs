using Microsoft.AppCenter.Channel;
using Moq;

namespace Microsoft.AppCenter.Test.Channel
{
    public class MockChannelGroupFactory : IChannelGroupFactory
    {
        private readonly Mock<IChannelGroup> _channelGroupMock;

        public MockChannelGroupFactory(Mock<IChannelGroup> channelGroupMock)
        {
            _channelGroupMock = channelGroupMock;
        }

        public IChannelGroup CreateChannelGroup(string appSecret)
        {
            return _channelGroupMock.Object;
        }
    }
}
