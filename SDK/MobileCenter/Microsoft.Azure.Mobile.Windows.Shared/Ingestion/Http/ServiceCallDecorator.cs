using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

        public ServiceCallFailedHandler ServiceCallFailedCallback
        {
            get { return DecoratedApi.ServiceCallFailedCallback; }
            set { DecoratedApi.ServiceCallFailedCallback = value; }
        }
        public Action ServiceCallSucceededCallback
        {
            get { return DecoratedApi.ServiceCallSucceededCallback; }
            set { DecoratedApi.ServiceCallSucceededCallback = value; }
        }

        protected ServiceCallDecorator(IServiceCall decoratedApi)
        {
            DecoratedApi = decoratedApi;
        }
        public virtual void Execute()
        {
            DecoratedApi.Execute();
        }

        public virtual void Cancel()
        {
            DecoratedApi.Cancel();
        }
    }
}
