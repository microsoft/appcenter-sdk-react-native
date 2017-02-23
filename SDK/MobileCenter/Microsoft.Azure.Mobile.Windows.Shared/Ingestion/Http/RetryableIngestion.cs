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

        public override IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var decoratedCall = DecoratedApi.PrepareServiceCall(appSecret, installId, logs, cancellationToken);
            return new RetryableServiceCall(decoratedCall, this, logs, appSecret, installId);
        }
    }
}
