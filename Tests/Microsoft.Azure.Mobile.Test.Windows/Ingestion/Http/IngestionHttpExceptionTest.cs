using Microsoft.AppCenterIngestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class IngestionHttpExceptionTest
    {
        [TestMethod]
        public void IsRecoverablePropertyTest()
        {
            HttpIngestionException exception = new HttpIngestionException("Test exception message");

            exception.StatusCode = (int) System.Net.HttpStatusCode.ServiceUnavailable;
            Assert.IsTrue(exception.IsRecoverable);

            exception.StatusCode = (int) System.Net.HttpStatusCode.RequestTimeout;
            Assert.IsTrue(exception.IsRecoverable);

            exception.StatusCode = 429;
            Assert.IsTrue(exception.IsRecoverable);

            exception.StatusCode = (int) System.Net.HttpStatusCode.InternalServerError;
            Assert.IsTrue(exception.IsRecoverable);
        }
    }
}
