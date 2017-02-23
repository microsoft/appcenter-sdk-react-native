using System;
using System.Collections.Generic;
using System.Text;
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
        void Execute();
        void Cancel();
        event ServiceCallFailedHandler Failed;
        event Action Succeeded;
    }
}
