using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public abstract class ServiceCallDecorator : ServiceCall
    {
        protected IServiceCall DecoratedApi { get; }
        public override event ServiceCallFailedHandler Failed;
        public override event Action Succeeded {
            add
            {
                DecoratedApi.Succeeded += value;
                base.Succeeded += value;
            }
            remove
            {
                DecoratedApi.Succeeded -= value;
                base.Succeeded -= value;
            }
        }
        protected ServiceCallDecorator(IServiceCall decoratedApi, IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId) : base(ingestion, logs, appSecret, installId)
        {
            DecoratedApi = decoratedApi;
        }
        public override void Execute()
        {
            DecoratedApi.Execute();
        }
    

    }
}
