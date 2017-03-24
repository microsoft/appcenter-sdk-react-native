using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class HttpNetworkAdapterTest
    {
        /// <summary>
        /// Validate that dispose is disposing http client
        /// </summary>
        [TestMethod]
        public void HttpNetworkAdapterDisposeTest()
        {
            HttpNetworkAdapter adapter = new HttpNetworkAdapter();
            adapter.Dispose();

            Assert.ThrowsException<IngestionException>(() => adapter.SendAsync(new HttpRequestMessage(), CancellationToken.None).RunNotAsync());
        }
    }
}
