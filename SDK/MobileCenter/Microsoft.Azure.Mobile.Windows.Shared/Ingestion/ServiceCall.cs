using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class ServiceCall : IServiceCall
    {
        public virtual event ServiceCallFailedHandler Failed;
        public virtual event Action Succeeded;

        protected IIngestion Ingestion { get; }
        protected IList<Log> Logs { get; }
        protected string AppSecret { get; }
        protected Guid InstallId { get; }

        public ServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId)
        {
            Ingestion = ingestion;
            Logs = logs;
            AppSecret = appSecret;
            InstallId = installId;
        }
        public ServiceCall(IIngestion ingestion)
        {
            Ingestion = ingestion;
        }

        public virtual void Execute()
        {
            Ingestion.SendLogsAsync(AppSecret, InstallId, Logs).ContinueWith(completedTask =>
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
