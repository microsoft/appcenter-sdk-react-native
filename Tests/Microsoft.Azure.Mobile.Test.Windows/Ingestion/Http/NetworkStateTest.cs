using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    [TestClass]
    public class NetworkStateTest : HttpIngestionTest
    {
        private NetworkStateAdapter _networkState;
        private NetworkStateIngestion _networkStateIngestion;

        [TestInitialize]
        public void InitializeNetworkStateTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _networkState = new NetworkStateAdapter();

            var httpIngestion = new IngestionHttp(_adapter.Object);
            _networkStateIngestion = new NetworkStateIngestion(httpIngestion, _networkState);
        }

        /// <summary>
        /// Verify that ingestion create ServiceCall correctly.
        /// </summary>
        [TestMethod]
        public void NetworkStateIngestionPrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            var call = _networkStateIngestion.PrepareServiceCall(appSecret, installId, logs);
            Assert.IsInstanceOfType(call, typeof(NetworkStateServiceCall));
            Assert.AreEqual(call.AppSecret, appSecret);
            Assert.AreEqual(call.InstallId, installId);
            Assert.AreEqual(call.Logs, logs);
        }

        /// <summary>
        /// Verify that call executed when network is available.
        /// </summary>
        [TestMethod]
        public void NetworkStateIngestionOnline()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = true;
            _networkStateIngestion.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }
        
        /// <summary>
        /// Verify that call not executed when network is not available.
        /// </summary>
        [TestMethod]
        public void NetworkStateIngestionOffline()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = false;
            var completedInTime = false;
            _networkStateIngestion.ExecuteCallAsync(call).ContinueWith(task => completedInTime = true);
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            Assert.IsFalse(completedInTime);
            VerifyAdapterSend(Times.Never());
        }

        /// <summary>
        /// Verify that call resent when network is available again.
        /// </summary>
        [TestMethod]
        public void NetworkStateIngestionComeBackOnline()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = false;
            var task = _networkStateIngestion.ExecuteCallAsync(call);
            VerifyAdapterSend(Times.Never());
            _networkState.IsConnected = true;
            task.Wait();
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Verify that multiple calls are resent when network is available again.
        /// </summary>
        [TestMethod]
        public void NetworkStateIngestionComeBackOnlineMultipleCalls()
        {
            int numCalls = 5;
            var calls = new List<IServiceCall>();
            for (int i = 0; i < numCalls; ++i)
            {
                calls.Add(PrepareServiceCall());
            }
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _networkState.IsConnected = false;

            var tasks = new List<Task>();
            foreach (var call in calls)
            {
                tasks.Add(_networkStateIngestion.ExecuteCallAsync(call));
            }
            _networkState.IsConnected = true;
            Task.WaitAll(tasks.ToArray());
            VerifyAdapterSend(Times.Exactly(numCalls));
        }

        /// <summary>
        /// Helper for prepare ServiceCall.
        /// </summary>
        private IServiceCall PrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            return _networkStateIngestion.PrepareServiceCall(appSecret, installId, logs);
        }
    }
}
