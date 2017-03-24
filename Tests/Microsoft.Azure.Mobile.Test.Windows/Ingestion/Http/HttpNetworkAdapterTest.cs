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
        HttpNetworkAdapter _adapter = new HttpNetworkAdapter();

        /// <summary>
        /// Validate that dispose is disposing http client
        /// </summary>
        [TestMethod]
        public void HttpNetworkAdapterDisposeTest()
        {
            _adapter.Dispose();

            Assert.ThrowsException<IngestionException>(() => _adapter.SendAsync(new HttpRequestMessage(), CancellationToken.None).RunNotAsync());
        }

        /// <summary>
        /// Validate that timeout is saving correctly
        /// </summary>
        [TestMethod]
        public void HttpNetworkAdatapterTimeoutTest()
        {
            TimeSpan oneSecond = new TimeSpan(0, 0, 1);
            _adapter.Timeout = oneSecond;

            Assert.AreEqual(oneSecond, _adapter.Timeout);
        }
    }
}
