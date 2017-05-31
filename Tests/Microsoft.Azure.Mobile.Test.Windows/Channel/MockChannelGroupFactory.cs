using Microsoft.Azure.Mobile.Channel;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Windows.Channel
{
    public class MockChannelGroupFactory : IChannelGroupFactory
    {
        public MockChannelGroupFactory(Mock<IChannelGroup> channelGroupMock)
        {
            _channelGroupMock = channelGroupMock;
        }

        private readonly Mock<IChannelGroup> _channelGroupMock;

        public IChannelGroup CreateChannelGroup(string appSecret)
        {
            return _channelGroupMock.Object;
        }
    }
}
