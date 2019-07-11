// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Storage;
using Microsoft.AppCenter.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.AppCenter.Crashes.Test.Windows
{
    [TestClass]
    public class HandledErrorTest
    {

        [TestInitialize]
        public void InitializeCrashTest()
        {
            Crashes.Instance = new Crashes();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Crashes.Instance = null;
        }

        [TestMethod]
        public void TrackErrorWithValidParameters()
        {
            var mockHttpNetworkAdapter = Mock.Of<IHttpNetworkAdapter>();
            var storage = new Storage.Storage(new StorageAdapter("test.db"));
            var ingestion = new IngestionHttp(mockHttpNetworkAdapter);
            var channelGroup = new ChannelGroup(ingestion, storage, "app secret");
            Crashes.SetEnabledAsync(true).Wait();
            Crashes.Instance.OnChannelGroupReady(channelGroup, "app secret");
            var semaphore = new SemaphoreSlim(0);
            HandledErrorLog actualLog = null;
            Mock.Get(mockHttpNetworkAdapter).Setup(adapter => adapter.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .Callback((string uri, string method, IDictionary<string, string> headers, string content, CancellationToken cancellation) =>
                {
                    actualLog = JsonConvert.DeserializeObject<LogContainer>(content, LogSerializer.SerializationSettings).Logs.Single() as HandledErrorLog;
                    semaphore.Release();
                });
            var exception = new System.Exception("Something went wrong.");
            var properties = new Dictionary<string, string> { { "k1", "v1" }, { "p2", "v2" } };
            Crashes.TrackError(exception, properties);

            // Wait until the http layer sends the log.
            semaphore.Wait(10000);
            Assert.IsNotNull(actualLog);
            Assert.AreEqual(exception.Message, actualLog.Exception.Message);
            CollectionAssert.AreEquivalent(properties, actualLog.Properties as Dictionary<string, string>);
        }

        [TestMethod]
        public void TrackErrorWithTooManyProperties()
        {
            var mockHttpNetworkAdapter = Mock.Of<IHttpNetworkAdapter>();
            var storage = new Storage.Storage(new StorageAdapter("test.db"));
            var ingestion = new IngestionHttp(mockHttpNetworkAdapter);
            var channelGroup = new ChannelGroup(ingestion, storage, "app secret");
            Crashes.Instance.OnChannelGroupReady(channelGroup, "app secret");
            var semaphore = new SemaphoreSlim(0);
            HandledErrorLog actualLog = null;
            Mock.Get(mockHttpNetworkAdapter).Setup(adapter => adapter.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IDictionary<string, string>>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .Callback((string uri, string method, IDictionary<string, string> headers, string content, CancellationToken cancellation) =>
                {
                    actualLog = JsonConvert.DeserializeObject<LogContainer>(content, LogSerializer.SerializationSettings).Logs.Single() as HandledErrorLog;
                    semaphore.Release();
                });
            var exception = new System.Exception("Something went wrong.");
            var maxProperties = 20;
            var properties = new Dictionary<string, string>();
            for (int i = 0; i < maxProperties + 5; ++i)
            {
                properties[i.ToString()] = i.ToString();
            }
            Crashes.TrackError(exception, properties);

            // Wait until the http layer sends the log.
            semaphore.Wait(10000);
            Assert.IsNotNull(actualLog);
            Assert.AreEqual(exception.Message, actualLog.Exception.Message);
            CollectionAssert.IsSubsetOf(actualLog.Properties as Dictionary<string, string>, properties);
            Assert.AreEqual(maxProperties, actualLog.Properties.Count);
        }
    }
}
