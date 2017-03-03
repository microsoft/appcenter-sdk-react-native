using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using HyperMock;
using Microsoft.Azure.Mobile.Ingestion.Models;

//TODO need to test events somehow

namespace Microsoft.Azure.Mobile.Test.Channel
{
    [TestClass]
    public class ChannelGroupTest
    {
        /// <summary>
        /// Verify that setting the server url works correctly.
        /// </summary>
        [TestMethod]
        public void TestSetServerUrl()
        {
            var mockIngestion = Mock.Create<IIngestion>();
            var mockStorage = Mock.Create<IStorage>();
            var appSecret = Guid.NewGuid().ToString();
            var channelGroup = new TestChannelGroup(mockStorage.Object, mockIngestion.Object, appSecret);
            var urlString = "here is a string dot com";
            channelGroup.SetServerUrl(urlString);
            mockIngestion.Verify(ingestion => ingestion.SetServerUrl(Param.Is<string>(s => s == (urlString))), Occurred.Once()); //this should fail until dot net is removed
        }
        
        //TODO make the purpose of this test a bit more clear. currently, adding a channel will never return null. maybe use this test to verify events are subscribed to?
        /// <summary>
        /// Verify that a adding a Channel to a ChannelGroup works
        /// </summary>
        [TestMethod]
        public void TestAddChannel()
        {
            var mockIngestion = Mock.Create<IIngestion>();
            var mockStorage = Mock.Create<IStorage>();
            var appSecret = Guid.NewGuid().ToString();
            var channelGroup = new TestChannelGroup(mockStorage.Object, mockIngestion.Object, appSecret);
            var channelName = "some_channel";
            var addedChannel = channelGroup.AddChannel(channelName, 2, TimeSpan.FromSeconds(3), 3);

            Assert.IsNotNull(addedChannel);
        }

        /// <summary>
        /// Verify that an error is thrown when a duplicate channel is added
        /// </summary>
        [TestMethod]
        public void TestAddDuplicateChannel()
        {
            var mockIngestion = Mock.Create<IIngestion>();
            var mockStorage = Mock.Create<IStorage>();
            var appSecret = Guid.NewGuid().ToString();
            var channelGroup = new TestChannelGroup(mockStorage.Object, mockIngestion.Object, appSecret);
            var channelMock = Mock.Create<IChannel>();
            channelGroup.AddChannel(channelMock.Object);
            Assert.ThrowsException<MobileCenterException>(() => channelGroup.AddChannel(channelMock.Object));
        }

        /// <summary>
        /// Verify that an error is thrown when a null channel is added
        /// </summary>
        [TestMethod]
        public void TestAddNullChannel()
        {
            var mockIngestion = Mock.Create<IIngestion>();
            var mockStorage = Mock.Create<IStorage>();
            var appSecret = Guid.NewGuid().ToString();
            var channelGroup = new TestChannelGroup(mockStorage.Object, mockIngestion.Object, appSecret);
            Assert.ThrowsException<MobileCenterException>(() => channelGroup.AddChannel(null));
        }

        /// <summary>
        /// Verify that enabling/disabling a channel group enables/disables all of its children.
        /// </summary>
        [TestMethod]
        public void TestEnableChannelGroup()
        {
            var mockIngestion = Mock.Create<IIngestion>();
            var mockStorage = Mock.Create<IStorage>();
            var appSecret = Guid.NewGuid().ToString();
            var channelGroup = new TestChannelGroup(mockStorage.Object, mockIngestion.Object, appSecret);
            var numChannels = 5;
            var channelMocks = new List<Mock>();
            for (var i = 0; i < numChannels; ++i)
            {
                channelMocks.Add(Mock.Create<IChannel>());
            }
            foreach (var mock in channelMocks)
            {
                var mockedChannel = mock.Object as IChannel;
                channelGroup.AddChannel(mockedChannel);
            }

            channelGroup.SetEnabled(true);
            channelGroup.SetEnabled(false);

            foreach (var channelMock in channelMocks.Select(mock => mock as Mock<IChannel>))
            {
                channelMock.Verify(channel => channel.SetEnabled(Param.Is<bool>(p => p)), Occurred.Once());
                channelMock.Verify(channel => channel.SetEnabled(Param.Is<bool>(p => !p)), Occurred.Once());
            }

        }

        private static Task GetCompletedTask()
        {
            Task completedTask = Task.Delay(0);
            completedTask.Wait();
            return completedTask;
        }

        private static Task<string> GetCompletedTaskString()
        {
            var completedTask = new Task<string>(() => "hello");
            completedTask.Wait();
            return completedTask;
        }

    }
    /* Custom class to expose protected constructor */
    public class TestChannelGroup : ChannelGroup
    {
        public TestChannelGroup(IStorage storage, IIngestion ingestion, string appSecret)
            : base(ingestion, storage, appSecret)
        {
        }

        public TestChannelGroup(string appSecret) : base(appSecret)
        {
        }
    }
}
