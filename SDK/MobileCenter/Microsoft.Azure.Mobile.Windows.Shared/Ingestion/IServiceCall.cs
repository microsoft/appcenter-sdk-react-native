using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion
{
    public interface IServiceCall : IDisposable
    {
        IIngestion Ingestion { get; }
        IList<Log> Logs { get; }
        string AppSecret { get; }
        Guid InstallId { get; }
        CancellationToken CancellationToken { get; }

        ///<exception cref="IngestionException"/>
        Task ExecuteAsync();
        void Cancel();
    }
}
