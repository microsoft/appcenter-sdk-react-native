using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public abstract class ServiceCall : IServiceCall
    {
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public IIngestion Ingestion { get; }
        public IList<Log> Logs { get; }
        public string AppSecret { get; }
        public Guid InstallId { get; }

        public CancellationToken CancellationToken => _tokenSource.Token;

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

        public virtual async Task ExecuteAsync()
        {
            _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();
            await Ingestion.ExecuteCallAsync(this).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tokenSource?.Dispose();
            }
        }
    }
}
