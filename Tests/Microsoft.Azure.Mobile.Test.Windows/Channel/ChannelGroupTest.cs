using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Moq;
using Xunit;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    public class ChannelGroupTest
    {
        private ChannelGroup _channelGroup;
        private Mock<IStorage> _mockStorage;
        private Mock<IIngestion> _mockIngestion;
        private readonly string _appSecret = Guid.NewGuid().ToString();

        public ChannelGroupTest()
        {
            _mockIngestion = new Mock<IIngestion>();
            _mockStorage = new Mock<IStorage>();
            _channelGroup = new TestChannelGroup(_mockStorage.Object, _mockIngestion.Object, _appSecret);
        }

        /// <summary>
        /// Verify that setting the server url works correctly.
        /// </summary>
        [Fact]
        public void TestSetServerUrl()
        {
            const string urlString = "here is a string dot com";
            _channelGroup.SetServerUrl(urlString);

            _mockIngestion.Verify(ingestion => ingestion.SetServerUrl(urlString), Times.Once());
        }
        
        /// <summary>
        /// Verify that a adding a Channel to a ChannelGroup works
        /// </summary>
        [Fact]
        public void TestAddChannel()
        {
            const string channelName = "some_channel";
            var addedChannel =
                _channelGroup.AddChannel(channelName, 2, TimeSpan.FromSeconds(3), 3) as Mobile.Channel.Channel;

            Assert.NotNull(addedChannel);
            Assert.Equal(channelName, addedChannel.Name);
        }

        /// <summary>
        /// Verify that channel group's enqueuing log event fires when appropriate
        /// </summary>
        [Fact]
        public void TestEnqueuingLogEvent()
        {
            var fired = false;
            _channelGroup.EnqueuingLog += (sender, args) => { fired = true; };
            var mockChannel = new Mock<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.EnqueuingLog += null, default(EnqueuingLogEventArgs));

            Assert.True(fired);
        }

        /// <summary>
        /// Verify that channel group's sending log event fires when appropriate
        /// </summary>
        [Fact]
        public void TestSendingLogEvent()
        {
            var fired = false;
            _channelGroup.SendingLog += (sender, args) => { fired = true; };
            var mockChannel = new Mock<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.SendingLog += null, default(SendingLogEventArgs));

            Assert.True(fired);
        }

        /// <summary>
        /// Verify that channel group's sent log event fires when appropriate
        /// </summary>
        [Fact]
        public void TestSentLogEvent()
        {
            var fired = false;
            _channelGroup.SentLog += (sender, args) => { fired = true; };
            var mockChannel = new Mock<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.SentLog += null, default(SentLogEventArgs));

            Assert.True(fired);
        }

        /// <summary>
        /// Verify that channel group's sent log event fires when appropriate
        /// </summary>
        [Fact]
        public void TestFailedToSendLogEvent()
        {
            var fired = false;
            _channelGroup.FailedToSendLog += (sender, args) => { fired = true; };
            var mockChannel = new Mock<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.FailedToSendLog += null, default(FailedToSendLogEventArgs));

            Assert.True(fired);
        }

        /// <summary>
        /// Verify that an error is thrown when a duplicate channel is added
        /// </summary>
        [Fact]
        public void TestAddDuplicateChannel()
        {
            var channelMock = new Mock<IChannel>();
            _channelGroup.AddChannel(channelMock.Object);

            Assert.Throws<MobileCenterException>(() => _channelGroup.AddChannel(channelMock.Object));
        }

        /// <summary>
        /// Verify that an error is thrown when a null channel is added
        /// </summary>
        [Fact]
        public void TestAddNullChannel()
        {
            Assert.Throws<MobileCenterException>(() => _channelGroup.AddChannel(null));
        }

        /// <summary>
        /// Verify that enabling/disabling a channel group enables/disables all of its children.
        /// </summary>
        [Fact]
        public void TestEnableChannelGroup()
        {
            const int numChannels = 5;
            var channelMocks = new List<Mock>();
            for (var i = 0; i < numChannels; ++i)
            {
                channelMocks.Add(new Mock<IChannel>());
            }
            foreach (var mockedChannel in channelMocks.Select(mock => mock.Object as IChannel))
            {
                _channelGroup.AddChannel(mockedChannel);
            }
            _channelGroup.SetEnabled(true);
            _channelGroup.SetEnabled(false);

            foreach (var channelMock in channelMocks.Select(mock => mock as Mock<IChannel>))
            {
                channelMock.Verify(channel => channel.SetEnabled(It.Is<bool>(p => p)), Times.Once());
                channelMock.Verify(channel => channel.SetEnabled(It.Is<bool>(p => !p)), Times.Once());
            }
        }
    }
}
