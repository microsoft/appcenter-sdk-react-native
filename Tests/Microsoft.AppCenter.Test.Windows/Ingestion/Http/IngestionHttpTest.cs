// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class IngestionHttpTest : HttpIngestionTest
    {
        private IngestionHttp _httpIngestion;

        [TestInitialize]
        public void InitializeHttpIngestionTest()
        {
            _adapter = new Mock<IHttpNetworkAdapter>();
            _httpIngestion = new IngestionHttp(_adapter.Object);
        }

        /// <summary>
        /// Verify that ingestion call http adapter and not fails on success.
        /// </summary>
        [TestMethod]
        public async Task HttpIngestionStatusCodeOk()
        {
            SetupAdapterSendResponse(HttpStatusCode.OK);
            var call = _httpIngestion.Call(AppSecret, InstallId, Logs);
            await call.ToTask();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }

        /// <summary>
        /// Verify that ingestion call http adapter and not fails on 2xx.
        /// </summary>
        [TestMethod]
        public async Task HttpIngestionStatusCodePartialContent()
        {
            SetupAdapterSendResponse(HttpStatusCode.PartialContent);
            var call = _httpIngestion.Call(AppSecret, InstallId, Logs);
            await call.ToTask();
            VerifyAdapterSend(Times.Once());

            // No throw any exception
        }

        /// <summary>
        /// Verify that ingestion throw exception on error response >= 300.
        /// </summary>
        [TestMethod]
        public async Task HttpIngestionStatusCodeError()
        {
            SetupAdapterSendResponse(HttpStatusCode.NotFound);
            var call = _httpIngestion.Call(AppSecret, InstallId, Logs);
            await Assert.ThrowsExceptionAsync<HttpIngestionException>(() => call.ToTask());
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Verify that ingestion throw exception on error response < 200.
        /// </summary>
        [TestMethod]
        public async Task HttpIngestionStatusCodeErrorBelow200()
        {
            SetupAdapterSendResponse(HttpStatusCode.SwitchingProtocols);
            var call = _httpIngestion.Call(AppSecret, InstallId, Logs);
            await Assert.ThrowsExceptionAsync<HttpIngestionException>(() => call.ToTask());
            VerifyAdapterSend(Times.Once());
        }

        /// <summary>
        /// Verify that ingestion don't call http adapter when call is closed.
        /// </summary>
        [TestMethod]
        public async Task HttpIngestionCancel()
        {
            _adapter
                .Setup(a => a.SendAsync(
                    It.IsAny<string>(),
                    "POST",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(500);
                    return "";
                });
            var call = _httpIngestion.Call(AppSecret, InstallId, Logs);
            call.Cancel();
            await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => call.ToTask());
        }

        /// <summary>
        /// Verify that ingestion create headers correctly.
        /// </summary>
        [TestMethod]
        public void HttpIngestionCreateHeaders()
        {
            var headers = _httpIngestion.CreateHeaders(AppSecret, InstallId);
            
            Assert.IsTrue(headers.ContainsKey(IngestionHttp.AppSecret));
            Assert.IsTrue(headers.ContainsKey(IngestionHttp.InstallId));
        }
    }
}
