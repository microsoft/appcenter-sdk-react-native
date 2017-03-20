using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

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

        public virtual IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs)
        {
            return DecoratedApi.PrepareServiceCall(appSecret, installId, logs);
        }

        public virtual Task ExecuteCallAsync(IServiceCall call)
        {
            return DecoratedApi.ExecuteCallAsync(call);
        }

        public virtual void SetServerUrl(string serverUrl)
        {
            DecoratedApi.SetServerUrl(serverUrl);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            /* No-op */
        }
    }
}
