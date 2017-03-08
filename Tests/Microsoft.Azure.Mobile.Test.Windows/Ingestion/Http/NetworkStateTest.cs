using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    [TestClass]
    public class NetworkStateTest
    {
        private Mock<IHttpNetworkAdapter> _adapter;
        private NetworkStateAdapter _networkState;
        private NetworkStateIngestion _networkStateIngestion;

        [TestInitialize]
        public void InitializeNetworkStateTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _networkState = new NetworkStateAdapter();
            _networkStateIngestion = new NetworkStateIngestion(new IngestionHttp(_adapter.Object), _networkState);
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
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
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
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _networkState.IsConnected = false;
            Assert.ThrowsException<NetworkUnavailableException>(() => _networkStateIngestion.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Times.Never());
        }

        /// <summary>
        /// Verify that call resended when network is available again.
        /// </summary>
        [TestMethod]
        public void NetworkStateIngestionComeBackOnline()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _networkState.IsConnected = false;
            Assert.ThrowsException<NetworkUnavailableException>(() => _networkStateIngestion.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Times.Never());
            _networkState.IsConnected = true;
            _networkStateIngestion.WaitAllCalls().RunNotAsync();
            VerifyAdapterSend(Times.Once());
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

        /// <summary>
        /// Helper for setup responce.
        /// </summary>
        private void SetupAdapterSendResponse(HttpResponseMessage response)
        {
            _adapter
                .Setup(a => a.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => response));
        }

        /// <summary>
        /// Helper for verify SendAsync call.
        /// </summary>
        private void VerifyAdapterSend(Times times)
        {
            _adapter
                .Verify(a => a.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()), times);
        }
    }
}
