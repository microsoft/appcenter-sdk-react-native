using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public abstract class IngestionDecorator : IIngestion
    {
        protected IIngestion DecoratedApi { get; }

        protected IngestionDecorator(IIngestion decoratedApi)
        {
            DecoratedApi = decoratedApi;
        }

        public virtual void SetLogUrl(string logUrl)
        {
            DecoratedApi.SetLogUrl(logUrl);
        }

        public virtual IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
        {
            return DecoratedApi.Call(appSecret, installId, logs);
        }

        public virtual void Close()
        {
            DecoratedApi.Close();
        }

        public virtual void Dispose()
        {
            DecoratedApi.Dispose();
        }
    }
}
