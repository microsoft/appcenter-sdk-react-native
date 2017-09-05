using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Test
{
    public sealed class MockIngestion : IIngestion
    {

        public bool CallShouldSucceed { get; set; }
        public bool CloseShouldSucceed { get; set; }
        public bool IsClosed { get; set; }
        public Exception TaskError = new IngestionException();

        public MockIngestion()
        {
            CallShouldSucceed = CloseShouldSucceed = true;
            IsClosed = false;
        }

        public IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs)
        {
            return new MockServiceCall(this, logs, appSecret, installId);
        }

        public Task ExecuteCallAsync(IServiceCall call)
        {
            return CallShouldSucceed ? TaskExtension.GetCompletedTask() : TaskExtension.GetFaultedTask(TaskError);
        }

        public void Close()
        {
            if (!CloseShouldSucceed)
            {
                throw new IngestionException();
            }
            IsClosed = true;
        }

        public void Open()
        {
            IsClosed = false;
        }

        public void SetLogUrl(string logUrl)
        {
        }

        public void Dispose()
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
