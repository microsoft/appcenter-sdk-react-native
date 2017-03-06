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

        [TestMethod]
        public void IngestionHttpStatusCodeOK()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Occurred.Once());

            // No throw any exception
        }

        [TestMethod]
        public void IngestionHttpStatusCodeError()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.NotFound));
            Assert.ThrowsException<HttpIngestionException>(() => _ingestionHttp.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Occurred.Once());
        }

        [TestMethod]
        public void IngestionHttpCancel()
        {
            var call = PrepareServiceCall();
            call.Cancel();
            SetupAdapterSendResponse(new HttpResponseMessage(HttpStatusCode.OK));
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Occurred.Never());
        }

        [TestMethod]
        public void IngestionHttpPrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            var call = _ingestionHttp.PrepareServiceCall(appSecret, installId, logs);
            Assert.AreEqual(call.GetType(), typeof(HttpServiceCall));
            Assert.AreEqual(call.Ingestion, _ingestionHttp);
            Assert.AreEqual(call.AppSecret, appSecret);
            Assert.AreEqual(call.InstallId, installId);
            Assert.AreEqual(call.Logs, logs);
        }

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

        private IServiceCall PrepareServiceCall()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            return _ingestionHttp.PrepareServiceCall(appSecret, installId, logs);
        }

        private void SetupAdapterSendResponse(HttpResponseMessage response)
        {
            _adapter
                .Setup(a => a.SendAsync(Param.IsAny<HttpRequestMessage>(), Param.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => response));
        }

        private void VerifyAdapterSend(Occurred occurred)
        {
            _adapter
                .Verify(a => a.SendAsync(Param.IsAny<HttpRequestMessage>(), Param.IsAny<CancellationToken>()), occurred);
        }
    }
}