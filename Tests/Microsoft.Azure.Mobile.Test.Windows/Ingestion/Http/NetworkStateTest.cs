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
using Moq;
using Xunit;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    public class NetworkStateTest
    {
        private Mock<IHttpNetworkAdapter> _adapter;
        private NetworkStateAdapter _networkState;
        private NetworkStateIngestion _networkStateIngestion;

        public NetworkStateTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _networkState = new NetworkStateAdapter();
            _networkStateIngestion = new NetworkStateIngestion(new IngestionHttp(_adapter.Object), _networkState);
        }

        /// <summary>
        /// Verify that ingestion create ServiceCall correctly.
        /// </summary>
        [Fact]
        public void NetworkStateIngestionPrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            var call = _networkStateIngestion.PrepareServiceCall(appSecret, installId, logs);
            Assert.IsType(typeof(NetworkStateServiceCall), call);
            Assert.Equal(call.AppSecret, appSecret);
            Assert.Equal(call.InstallId, installId);
            Assert.Equal(call.Logs, logs);
        }

        /// <summary>
        /// Verify that call executed when network is available.
        /// </summary>
        [Fact]
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
        [Fact]
        public void NetworkStateIngestionOffline()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _networkState.IsConnected = false;
            Assert.Throws<NetworkUnavailableException>(() => _networkStateIngestion.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Times.Never());
        }

        /// <summary>
        /// Verify that call resended when network is available again.
        /// </summary>
        [Fact]
        public void NetworkStateIngestionComeBackOnline()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _networkState.IsConnected = false;
            Assert.Throws<NetworkUnavailableException>(() => _networkStateIngestion.ExecuteCallAsync(call).RunNotAsync());
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
