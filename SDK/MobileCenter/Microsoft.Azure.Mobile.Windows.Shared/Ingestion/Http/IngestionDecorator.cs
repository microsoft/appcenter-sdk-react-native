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

        public virtual async Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            await DecoratedApi.SendLogsAsync(appSecret, installId, logs, cancellationToken);
        }

        public virtual void SetServerUrl(string serverUrl)
        {
            DecoratedApi.SetServerUrl(serverUrl);
        }
    }
}
