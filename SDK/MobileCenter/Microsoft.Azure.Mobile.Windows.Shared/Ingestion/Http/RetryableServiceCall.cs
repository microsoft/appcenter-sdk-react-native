using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class RetryableServiceCall : ServiceCallDecorator
    {
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private int _retryCount;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly Func<Task>[] _retryIntervals;

        public override CancellationToken CancellationToken => _tokenSource.Token;

        public RetryableServiceCall(IServiceCall decoratedApi, Func<Task>[] retryIntervals) : base(decoratedApi)
        {
            _retryIntervals = retryIntervals;
        }

        ///<exception cref="IngestionException"/>
        public async Task RunWithRetriesAsync()
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            try
            {
                _tokenSource = new CancellationTokenSource();
                await RunWithRetriesAsyncHelper().ConfigureAwait(false);
            }
            finally
            {
                _mutex.Release();
            }
        }

        ///<exception cref="IngestionException"/>
        private async Task RunWithRetriesAsyncHelper()
        {
            while (true)
            {
                try
                {
                    await Ingestion.ExecuteCallAsync(this).ConfigureAwait(false);
                    return;
                }
                catch (IngestionException e)
                {
                    if (!e.IsRecoverable || _retryCount >= _retryIntervals.Length)
                    {
                        throw;
                    }
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Failed to execute service call", e);
                }
                await _retryIntervals[_retryCount++]().ConfigureAwait(false);
                _tokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        public override void Cancel()
        {
            _tokenSource?.Cancel();
        }

        public override void Execute()
        {
            RunWithRetriesAsync().ContinueWith(completedTask =>
            {
                if (completedTask.IsFaulted)
                {
                    ServiceCallFailedCallback?.Invoke(completedTask.Exception?.InnerException as IngestionException);
                }
                else
                {
                    ServiceCallSucceededCallback?.Invoke();
                }
            });
        }

    }
}
