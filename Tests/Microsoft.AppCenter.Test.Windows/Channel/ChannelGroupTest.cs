// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Ingestion;
using Microsoft.AppCenter.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Channel
{
    using Channel = Microsoft.AppCenter.Channel.Channel;

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
            _mockIngestion = new Mock<IIngestion>();
            _mockStorage = new Mock<IStorage>();
            _channelGroup = new ChannelGroup(_mockIngestion.Object, _mockStorage.Object, _appSecret);
        }

        /// <summary>
        /// Verify that setting the log url works correctly.
        /// </summary>
        [TestMethod]
        public void TestSetLogUrl()
        {
            const string urlString = "here is a string dot com";
            _channelGroup.SetLogUrl(urlString);

            _mockIngestion.Verify(ingestion => ingestion.SetLogUrl(urlString), Times.Once());
        }

        /// <summary>
        /// Verify that a adding a Channel to a ChannelGroup works
        /// </summary>
        [TestMethod]
        public void TestAddChannel()
        {
            const string channelName = "some_channel";
            var addedChannel =
                _channelGroup.AddChannel(channelName, 2, TimeSpan.FromSeconds(3), 3) as Channel;

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
            var mockChannel = new Mock<IChannelUnit>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.EnqueuingLog += null, null, default(EnqueuingLogEventArgs));

            Assert.IsTrue(fired);
        }

        /// <summary>
        /// Verify that channel group's filtering log event fires when appropriate
        /// </summary>
        [TestMethod]
        public void TestFilteringLogEvent()
        {
            var fired = false;
            _channelGroup.FilteringLog += (sender, args) => { fired = true; };
            var mockChannel = new Mock<IChannelUnit>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.FilteringLog += null, null, default(FilteringLogEventArgs));

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
            var mockChannel = new Mock<IChannelUnit>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.SendingLog += null, null, default(SendingLogEventArgs));

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
            var mockChannel = new Mock<IChannelUnit>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.SentLog += null, null, default(SentLogEventArgs));

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
            var mockChannel = new Mock<IChannelUnit>();
            _channelGroup.AddChannel(mockChannel.Object);
            mockChannel.Raise(channel => channel.FailedToSendLog += null, null, default(FailedToSendLogEventArgs));

            Assert.IsTrue(fired);
        }

        /// <summary>
        /// Verify that an error is thrown when a duplicate channel is added
        /// </summary>
        [TestMethod]
        public void TestAddDuplicateChannel()
        {
            var channelMock = new Mock<IChannelUnit>();
            _channelGroup.AddChannel(channelMock.Object);

            Assert.ThrowsException<AppCenterException>(() => _channelGroup.AddChannel(channelMock.Object));
        }

        /// <summary>
        /// Verify that an error is thrown when a null channel is added
        /// </summary>
        [TestMethod]
        public void TestAddNullChannel()
        {
            Assert.ThrowsException<AppCenterException>(() => _channelGroup.AddChannel(null));
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
                channelMocks.Add(new Mock<IChannelUnit>());
            }
            foreach (var mockedChannel in channelMocks.Select(mock => mock.Object as IChannelUnit))
            {
                _channelGroup.AddChannel(mockedChannel);
            }
            _channelGroup.SetEnabled(true);
            _channelGroup.SetEnabled(false);

            foreach (var channelMock in channelMocks.Select(mock => mock as Mock<IChannelUnit>))
            {
                channelMock.Verify(channel => channel.SetEnabled(true), Times.Once);
                channelMock.Verify(channel => channel.SetEnabled(false), Times.Once);
            }
        }

        [TestMethod]
        public void TestDisposeChannelGroup()
        {
            _channelGroup.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => _channelGroup.SetEnabled(true));
        }

        /// <summary>
        /// Veriy that all channels are disabled after channel group disabling
        /// </summary>
        [TestMethod]
        public async Task TestShutdownChannelGroup()
        {
            const string channelName = "some_channel";
            var addedChannel =
                _channelGroup.AddChannel(channelName, 2, TimeSpan.FromSeconds(3), 3) as Channel;

            await _channelGroup.ShutdownAsync();

            Assert.IsFalse(addedChannel.IsEnabled);
            _mockIngestion.Verify(ingestion => ingestion.Close(), Times.Once);
        }
    }
}
