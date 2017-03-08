using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Test
{
    public class MockIngestion : IIngestion
    {
        public bool CallShouldSucceed { get; set; }
        public bool CloseShouldSucceed { get; set; }

        public MockIngestion()
        {
            CallShouldSucceed = CloseShouldSucceed = true;
        }

        public IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs)
        {
            return new MockServiceCall(this, logs, appSecret, installId);
        }

        public Task ExecuteCallAsync(IServiceCall call)
        {
            return CallShouldSucceed ? TaskExtension.GetCompletedTask() : TaskExtension.GetFaultedTask();
        }

        public void Close()
        {
            if (!CloseShouldSucceed)
            {
                throw new IngestionException();
            }
        }

        public void SetServerUrl(string serverUrl)
        {
        }
    }

    public class MockServiceCall : ServiceCall
    {
        public MockServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId) : base(ingestion, logs, appSecret, installId)
        {
        }
    }
}
