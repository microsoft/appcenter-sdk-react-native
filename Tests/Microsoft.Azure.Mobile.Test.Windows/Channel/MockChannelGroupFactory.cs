using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private readonly  Mock<IChannelGroup> _channelGroupMock;

        public IChannelGroup CreateChannelGroup(string appSecret)
        {
            return _channelGroupMock.Object;
        }
    }
}
