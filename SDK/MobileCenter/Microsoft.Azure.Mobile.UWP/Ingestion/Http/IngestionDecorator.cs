using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using System.Threading;

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Http
{
    public abstract class IngestionDecorator : IIngestion
    {
        protected IIngestion _decoratedApi { get; private set; }

        public IngestionDecorator(IIngestion decoratedApi)
        {
            _decoratedApi = decoratedApi;
        }

        public virtual void Close()
        {
            _decoratedApi.Close();
        }

        public virtual async Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _decoratedApi.SendLogsAsync(appSecret, installId, logs);
        }

        public virtual void SetServerUrl(string serverUrl)
        {
            _decoratedApi.SetServerUrl(serverUrl);
        }
    }
}
