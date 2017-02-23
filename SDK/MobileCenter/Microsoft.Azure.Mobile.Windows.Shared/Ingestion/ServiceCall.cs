using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public abstract class ServiceCall : IServiceCall
    {
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public IIngestion Ingestion { get; }
        public IList<Log> Logs { get; }
        public string AppSecret { get; }
        public Guid InstallId { get; }

        public CancellationToken CancellationToken => _tokenSource.Token;

        public event ServiceCallFailedHandler Failed;
        public event Action Succeeded;

        protected ServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId)
        {
            Ingestion = ingestion;
            Logs = logs;
            AppSecret = appSecret;
            InstallId = installId;
        }

        public virtual void Cancel()
        {
            _tokenSource.Cancel();
        }

        public virtual void Execute()
        {
            _tokenSource = new CancellationTokenSource();
            Ingestion.ExecuteCallAsync(this).ContinueWith(completedTask =>
            {
                if (completedTask.IsFaulted)
                {
                    Failed?.Invoke(completedTask.Exception?.InnerException as IngestionException);
                }
                else
                {
                    Succeeded?.Invoke();
                }
            });
        }
    }
}
