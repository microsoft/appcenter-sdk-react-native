using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Threading;
using Microsoft.Rest;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class RetryableIngestion : IngestionDecorator
    {
        public RetryableIngestion(IIngestion decoratedApi) : base(decoratedApi)
        {
        }

        public override IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs)
        {
            var decoratedCall = DecoratedApi.PrepareServiceCall(appSecret, installId, logs);
            return new RetryableServiceCall(decoratedCall);
        }

        public override async Task ExecuteCallAsync(IServiceCall call)
        {
            var retryableCall = call as RetryableServiceCall;
            if (retryableCall == null)
            {
                await base.ExecuteCallAsync(call);
                return;
            }
            await retryableCall.RunWithRetriesAsync();
        }
    }
}
