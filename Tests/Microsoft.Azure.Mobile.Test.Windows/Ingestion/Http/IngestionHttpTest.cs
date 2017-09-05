using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    [TestClass]
    public class IngestionHttpTest : HttpIngestionTest
    {
        private IngestionHttp _ingestionHttp;

        [TestInitialize]
        public void InitializeIngestionHttpTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _ingestionHttp = new IngestionHttp(_adapter.Object);
        }

        /// <summary>
        /// Verify that ingestion call http adapter and not fails on success.
        /// </summary>
        [TestMethod]
        public void IngestionHttpStatusCodeOk()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }

        /// <summary>
        /// Verify that ingestion throw exception on error response.
        /// </summary>
        [TestMethod]
        public void IngestionHttpStatusCodeError()
        {
            var call = PrepareServiceCall();
            SetupAdapterSendResponse(HttpStatusCode.NotFound);
            Assert.ThrowsException<HttpIngestionException>(() => _ingestionHttp.ExecuteCallAsync(call).RunNotAsync());
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Verify that ingestion don't call http adapter when call is closed.
        /// </summary>
        [TestMethod]
        public void IngestionHttpCancel()
        {
            var call = PrepareServiceCall();
            call.Cancel();
            SetupAdapterSendResponse(HttpStatusCode.OK);
            _ingestionHttp.ExecuteCallAsync(call).RunNotAsync();
            VerifyAdapterSend(Times.Never());
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
        /// Verify that ingestion create headers correctly.
        /// </summary>
        [TestMethod]
        public void IngestionHttpCreateHeaders()
        {
            var appSecret = Guid.NewGuid().ToString();
            var installId = Guid.NewGuid();
            var headers = _ingestionHttp.CreateHeaders(appSecret, installId);
            
            Assert.IsTrue(headers.ContainsKey(IngestionHttp.AppSecret));
            Assert.IsTrue(headers.ContainsKey(IngestionHttp.InstallId));
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
    }
}