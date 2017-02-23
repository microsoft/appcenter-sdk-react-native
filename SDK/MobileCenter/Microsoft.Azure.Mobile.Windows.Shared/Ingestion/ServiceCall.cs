using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public abstract class ServiceCall : IServiceCall
    {
        public virtual event ServiceCallFailedHandler Failed;
        public virtual event Action Succeeded;

        public IIngestion Ingestion { get; }
        public IList<Log> Logs { get; }
        public string AppSecret { get; }
        public Guid InstallId { get; }

        protected ServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId)
        {
            Ingestion = ingestion;
            Logs = logs;
            AppSecret = appSecret;
            InstallId = installId;
        }

        public virtual void Cancel()
        {
        }

        public virtual void Execute()
        {
            Ingestion.SendLogsAsync(this).ContinueWith(completedTask =>
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
