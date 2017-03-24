using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class IngestionHttpExceptionTest
    {
        [TestMethod]
        public void IsRecovarblePropertyTest()
        {
            HttpIngestionException exception = new HttpIngestionException("Test exception message");

            exception.StatusCode = System.Net.HttpStatusCode.ServiceUnavailable;
            Assert.IsTrue(exception.IsRecoverable);

            exception.StatusCode = System.Net.HttpStatusCode.RequestTimeout;
            Assert.IsTrue(exception.IsRecoverable);

            exception.StatusCode = (System.Net.HttpStatusCode)429;
            Assert.IsTrue(exception.IsRecoverable);

            exception.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            Assert.IsFalse(exception.IsRecoverable);
        }
    }
}
