using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public abstract class ServiceCallDecorator : IServiceCall
    {
        protected IServiceCall DecoratedApi { get; }

        public IIngestion Ingestion => DecoratedApi.Ingestion;
        public IList<Log> Logs => DecoratedApi.Logs;
        public string AppSecret => DecoratedApi.AppSecret;
        public Guid InstallId => DecoratedApi.InstallId;
        public virtual CancellationToken CancellationToken => DecoratedApi.CancellationToken;

        protected ServiceCallDecorator(IServiceCall decoratedApi)
        {
            DecoratedApi = decoratedApi;
        }
        public virtual Task ExecuteAsync()
        {
            return DecoratedApi.ExecuteAsync();
        }

        public virtual void Cancel()
        {
            DecoratedApi.Cancel();
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
