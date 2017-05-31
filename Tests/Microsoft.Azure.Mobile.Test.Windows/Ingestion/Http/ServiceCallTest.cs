using System;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.Mobile.Ingestion;

namespace Microsoft.Azure.Mobile.Test.Windows.Ingestion.Http
{
    [TestClass]
    public class ServiceCallTest
    {
        private MockIngestion _ingestion;
        private ServiceCall _serviceCall;

        [TestInitialize]
        public void InitializeServiceCallTest()
        {
            _ingestion = new MockIngestion();
            _serviceCall = new MockServiceCall(_ingestion, null, string.Empty, Guid.NewGuid());
        }

        [TestMethod]
        public void CheckSuccessCallback()
        {
            _ingestion.CallShouldSucceed = true;

            _serviceCall.ExecuteAsync().RunNotAsync();
        }

        [TestMethod]
        public void CheckUnsuccessCallback()
        {
            _ingestion.CallShouldSucceed = false;
            Assert.ThrowsException<IngestionException>(() => _serviceCall.ExecuteAsync().RunNotAsync());
        }
    }
}
