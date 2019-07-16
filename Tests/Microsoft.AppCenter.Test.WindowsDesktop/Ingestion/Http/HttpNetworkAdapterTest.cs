using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace Microsoft.AppCenter.Test.WindowsDesktop.Ingestion.Http
{
    [TestClass]
    public class HttpNetworkAdapterTest
    {
        [TestMethod]
        public void EnableTls12WhenDisabled()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
            HttpNetworkAdapter.EnableTls12();

            // Check protocol was added, not the whole value overridden.
            Assert.AreEqual(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12, ServicePointManager.SecurityProtocol);
        }

        [TestMethod]
        public void EnableTls12WhenAlreadyEnabled()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpNetworkAdapter.EnableTls12();

            // Just check no side effect.
            Assert.AreEqual(SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12, ServicePointManager.SecurityProtocol);
        }
    }
}
