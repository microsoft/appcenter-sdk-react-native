using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class HttpNetworkAdapterTest
    {
        private readonly HttpNetworkAdapter _adapter = new HttpNetworkAdapter();

        /// <summary>
        /// Validate that dispose is disposing http client
        /// </summary>
        [TestMethod]
        public void HttpNetworkAdapterDisposeTest()
        {
            _adapter.Dispose();

            Assert.ThrowsException<IngestionException>(() => _adapter.SendAsync(new HttpRequestMessage(), CancellationToken.None).RunNotAsync());
        }
    }
}
