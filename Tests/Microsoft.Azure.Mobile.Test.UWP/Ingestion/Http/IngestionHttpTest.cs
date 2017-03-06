using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HyperMock;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    [TestClass]
    public class IngestionHttpTest
    {
        private Mock<IHttpNetworkAdapter> _adapter;
        private IngestionHttp _ingestionHttp;

        [TestInitialize]
        public void InitializeIngestionHttpTest()
        {
            _adapter = Mock.Create<IHttpNetworkAdapter>();
            _ingestionHttp = new IngestionHttp(_adapter.Object);
        }

        /// <summary>
        /// Verify that ingestion call http adapter and not fails on success.
        /// </summary>
        [TestMethod]
        public void IngestionHttpStatusCodeOK()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Occurred.Once());

            // No throw any exception
        }

        /// <summary>
        /// Verify that ingestion throw exception on error response.
        /// </summary>
        [TestMethod]
        public void IngestionHttpStatusCodeError()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.NotFound));
            Assert.ThrowsException<HttpIngestionException>(() => _ingestionHttp.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Occurred.Once());
        }

        /// <summary>
        /// Verify that ingestion don't call http adapter when call is closed.
        /// </summary>
        [TestMethod]
        public void IngestionHttpCancel()
        {
            var call = PrepareServiceCall();
            call.Cancel();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Occurred.Never());
        }

        /// <summary>
        /// Verify that ingestion prepare ServiceCall correctly.
        /// </summary>
        [TestMethod]
        public void IngestionHttpPrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            var call = _ingestionHttp.PrepareServiceCall(appSecret, installId, logs);
            Assert.IsInstanceOfType(call, typeof(HttpServiceCall));
            Assert.AreEqual(call.Ingestion, _ingestionHttp);
            Assert.AreEqual(call.AppSecret, appSecret);
            Assert.AreEqual(call.InstallId, installId);
            Assert.AreEqual(call.Logs, logs);
        }

        /// <summary>
        /// Verify that ingestion create ServiceCall correctly.
        /// </summary>
        [TestMethod]
        public void IngestionHttpCreateRequest()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = string.Empty;
            var request = _ingestionHttp.CreateRequest(appSecret, installId, logs);

            Assert.AreEqual(request.Method, HttpMethod.Post);
            Assert.IsTrue(request.Headers.Contains(IngestionHttp.AppSecret));
            Assert.IsTrue(request.Headers.Contains(IngestionHttp.InstallId));
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
                .Setup(a => a.SendAsync(Param.IsAny<HttpRequestMessage>(), Param.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => response));
        }

        /// <summary>
        /// Helper for verify SendAsync call.
        /// </summary>
        private void VerifyAdapterSend(Occurred occurred)
        {
            _adapter
                .Verify(a => a.SendAsync(Param.IsAny<HttpRequestMessage>(), Param.IsAny<CancellationToken>()), occurred);
        }
    }
}