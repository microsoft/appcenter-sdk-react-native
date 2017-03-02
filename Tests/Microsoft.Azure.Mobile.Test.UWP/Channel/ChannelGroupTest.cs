using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Channel;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using HyperMock;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Test.Channel
{
    [TestClass]
    public class ChannelGroupTest
    {
        [TestMethod]
        public void TestSetServerUrl()
        {
            var mockIngestion = Mock.Create<IIngestion>();
            var mockStorage = Mock.Create<IStorage>();
            var appSecret = Guid.NewGuid().ToString();
            var channelGroup = new TestChannelGroup(mockStorage.Object, mockIngestion.Object, appSecret);
            var urlString = "here is a string dot com";
            channelGroup.SetServerUrl(urlString);
            mockIngestion.Verify(ingestion => ingestion.SetServerUrl(Param.Is<string>(s => s == (urlString + "dot net"))), Occurred.Once()); //this should fail until dot net is removed
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
