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
using Microsoft.Rest;
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
            Assert.ThrowsException<IngestionException>(() => _ingestionHttp.ExecuteCallAsync(call).RunNotAsync());
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
        public void IngestionHttpThrowThrowHttpOperationException()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
            var request = _ingestionHttp.CreateRequest(appSecret, installId, logs);
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
            var ex = Assert.ThrowsException<IngestionException>(() => _ingestionHttp.ThrowHttpOperationException(request, response).RunNotAsync());
            var inner = ex.InnerException as HttpOperationException;
            Assert.IsNotNull(inner);
            Assert.AreEqual(inner.Response.StatusCode, response.StatusCode);
        }

        [TestMethod]
        public void IngestionHttpCreateRequest()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var logs = new List<Log>();
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