using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Threading;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public abstract class IngestionDecorator : IIngestion
    {
        protected IIngestion DecoratedApi { get; }

        protected IngestionDecorator(IIngestion decoratedApi)
        {
            DecoratedApi = decoratedApi;
        }

        public virtual void Close()
        {
            DecoratedApi.Close();
        }

        public abstract IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs,
            CancellationToken cancellationToken = new CancellationToken());

        public virtual async Task SendLogsAsync(IServiceCall call)
        {
            await DecoratedApi.SendLogsAsync(call);
        }

        public virtual void SetServerUrl(string serverUrl)
        {
            DecoratedApi.SetServerUrl(serverUrl);
        }
    }
}
