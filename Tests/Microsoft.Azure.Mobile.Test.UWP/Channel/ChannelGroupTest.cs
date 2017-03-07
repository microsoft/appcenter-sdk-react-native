using System;
using System.Collections.Generic;
using System.Linq;
using HyperMock;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    [TestClass]
    public class ChannelGroupTest
    {
        private ChannelGroup _channelGroup;
        private Mock<IStorage> _mockStorage;
        private Mock<IIngestion> _mockIngestion;
        private readonly string _appSecret = Guid.NewGuid().ToString();

        [TestInitialize]
        public void InitializeChannelGroupTest()
        {
            _mockIngestion = Mock.Create<IIngestion>();
            _mockStorage = Mock.Create<IStorage>();
            _channelGroup = new TestChannelGroup(_mockStorage.Object, _mockIngestion.Object, _appSecret);
        }

        /// <summary>
        /// Verify that setting the server url works correctly.
        /// </summary>
        [TestMethod]
        public void TestSetServerUrl()
        {
            const string urlString = "here is a string dot com";
            _channelGroup.SetServerUrl(urlString);

            _mockIngestion.Verify(ingestion => ingestion.SetServerUrl(urlString), Occurred.Once());
        }
        
        /// <summary>
        /// Verify that a adding a Channel to a ChannelGroup works
        /// </summary>
        [TestMethod]
        public void TestAddChannel()
        {
            const string channelName = "some_channel";
            var addedChannel =
                _channelGroup.AddChannel(channelName, 2, TimeSpan.FromSeconds(3), 3) as Mobile.Channel.Channel;

            Assert.IsNotNull(addedChannel);
            Assert.AreEqual(channelName, addedChannel.Name);
        }

        /// <summary>
        /// Verify that channel group's enqueuing log event fires when appropriate
        /// </summary>
        [TestMethod]
        public void TestEnqueuingLogEvent()
        {
            var fired = false;
            _channelGroup.EnqueuingLog += (sender, args) => { fired = true; };
            var mockChannel = Mock.Create<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.EnqueuingLog += null, default(EnqueuingLogEventArgs));

            Assert.IsTrue(fired);
        }

        /// <summary>
        /// Verify that channel group's sending log event fires when appropriate
        /// </summary>
        [TestMethod]
        public void TestSendingLogEvent()
        {
            var fired = false;
            _channelGroup.SendingLog += (sender, args) => { fired = true; };
            var mockChannel = Mock.Create<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.SendingLog += null, default(SendingLogEventArgs));

            Assert.IsTrue(fired);
        }

        /// <summary>
        /// Verify that channel group's sent log event fires when appropriate
        /// </summary>
        [TestMethod]
        public void TestSentLogEvent()
        {
            var fired = false;
            _channelGroup.SentLog += (sender, args) => { fired = true; };
            var mockChannel = Mock.Create<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.SentLog += null, default(SentLogEventArgs));

            Assert.IsTrue(fired);
        }

        /// <summary>
        /// Verify that channel group's sent log event fires when appropriate
        /// </summary>
        [TestMethod]
        public void TestFailedToSendLogEvent()
        {
            var fired = false;
            _channelGroup.FailedToSendLog += (sender, args) => { fired = true; };
            var mockChannel = Mock.Create<IChannel>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.FailedToSendLog += null, default(FailedToSendLogEventArgs));

            Assert.IsTrue(fired);
        }

        /// <summary>
        /// Verify that an error is thrown when a duplicate channel is added
        /// </summary>
        [TestMethod]
        public void TestAddDuplicateChannel()
        {
            var channelMock = Mock.Create<IChannel>();
            _channelGroup.AddChannel(channelMock.Object);

            Assert.ThrowsException<MobileCenterException>(() => _channelGroup.AddChannel(channelMock.Object));
        }

        /// <summary>
        /// Verify that an error is thrown when a null channel is added
        /// </summary>
        [TestMethod]
        public void TestAddNullChannel()
        {
            Assert.ThrowsException<MobileCenterException>(() => _channelGroup.AddChannel(null));
        }

        /// <summary>
        /// Verify that enabling/disabling a channel group enables/disables all of its children.
        /// </summary>
        [TestMethod]
        public void TestEnableChannelGroup()
        {
            const int numChannels = 5;
            var channelMocks = new List<Mock>();
            for (var i = 0; i < numChannels; ++i)
            {
                channelMocks.Add(Mock.Create<IChannel>());
            }
            foreach (var mockedChannel in channelMocks.Select(mock => mock.Object as IChannel))
            {
                _channelGroup.AddChannel(mockedChannel);
            }
            _channelGroup.SetEnabled(true);
            _channelGroup.SetEnabled(false);

            foreach (var channelMock in channelMocks.Select(mock => mock as Mock<IChannel>))
            {
                channelMock.Verify(channel => channel.SetEnabled(Param.Is<bool>(p => p)), Occurred.Once());
                channelMock.Verify(channel => channel.SetEnabled(Param.Is<bool>(p => !p)), Occurred.Once());
            }
        }
    }
}
