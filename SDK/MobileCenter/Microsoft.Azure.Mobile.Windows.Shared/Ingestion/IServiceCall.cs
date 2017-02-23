using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion
{
    public delegate void ServiceCallFailedHandler(IngestionException exception);

    public interface IServiceCall
    {
        IIngestion Ingestion { get; }
        IList<Log> Logs { get; }
        string AppSecret { get; }
        Guid InstallId { get; }
        CancellationToken CancellationToken { get; }
        void Execute();
        void Cancel();

        ServiceCallFailedHandler ServiceCallFailedCallback { get; set; }
        Action ServiceCallSucceededCallback { get; set; }
  
    }
}
