using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Moq;
using Xunit;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    public class IngestionHttpTest
    {
        private Mock<IHttpNetworkAdapter> _adapter;
        private IngestionHttp _ingestionHttp;

        public IngestionHttpTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _ingestionHttp = new IngestionHttp(_adapter.Object);
        }

        /// <summary>
        /// Verify that ingestion call http adapter and not fails on success.
        /// </summary>
        [Fact]
        public void IngestionHttpStatusCodeOK()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }

        /// <summary>
        /// Verify that ingestion throw exception on error response.
        /// </summary>
        [Fact]
        public void IngestionHttpStatusCodeError()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.NotFound));
            Assert.Throws<HttpIngestionException>(() => _ingestionHttp.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Verify that ingestion don't call http adapter when call is closed.
        /// </summary>
        [Fact]
        public void IngestionHttpCancel()
        {
            var call = PrepareServiceCall();
            call.Cancel();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Times.Never());
        }

        /// <summary>
        /// Verify that ingestion prepare ServiceCall correctly.
        /// </summary>
        [Fact]
        public void IngestionHttpPrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            var call = _ingestionHttp.PrepareServiceCall(appSecret, installId, logs);
            Assert.IsType(typeof(HttpServiceCall), call);
            Assert.Equal(call.Ingestion, _ingestionHttp);
            Assert.Equal(call.AppSecret, appSecret);
            Assert.Equal(call.InstallId, installId);
            Assert.Equal(call.Logs, logs);
        }

        /// <summary>
        /// Verify that ingestion create ServiceCall correctly.
        /// </summary>
        [Fact]
        public void IngestionHttpCreateRequest()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = string.Empty;
            var request = _ingestionHttp.CreateRequest(appSecret, installId, logs);

            Assert.Equal(request.Method, HttpMethod.Post);
            Assert.True(request.Headers.Contains(IngestionHttp.AppSecret));
            Assert.True(request.Headers.Contains(IngestionHttp.InstallId));
        }

        /// <summary>
        /// Helper for prepare ServiceCall.
        /// </summary>
        private IServiceCall PrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            return _ingestionHttp.PrepareServiceCall(appSecret, installId, logs);
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