using System;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AppCenter.Ingestion;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Http
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
