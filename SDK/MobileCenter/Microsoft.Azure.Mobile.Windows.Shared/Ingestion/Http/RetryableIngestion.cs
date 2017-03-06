using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class RetryableIngestion : IngestionDecorator
    {
        private static readonly Random Random = new Random();
        private static readonly SemaphoreSlim RandomMutex = new SemaphoreSlim(1, 1);
        private static readonly TimeSpan[] DefaultIntervals =
            {
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMinutes(.5),
                TimeSpan.FromMinutes(20)
            };

        private readonly Func<Task>[] _retryIntervals;

        public RetryableIngestion(IIngestion decoratedApi)
            : this(decoratedApi, DefaultIntervals)
        {
        }

        public RetryableIngestion(IIngestion decoratedApi, TimeSpan[] retryIntervals) : base(decoratedApi)
        {
            if (retryIntervals == null) throw new ArgumentNullException("retryIntervals");
            _retryIntervals = new Func<Task>[retryIntervals.Length];
            for (int i = 0; i < retryIntervals.Length; i++)
            {
                _retryIntervals[i] = GetDelayFunc(retryIntervals, i);
            }
        }

        public RetryableIngestion(IIngestion decoratedApi, Func<Task>[] retryIntervals) : base(decoratedApi)
        {
                if (retryIntervals == null) throw new ArgumentNullException("retryIntervals");
                _retryIntervals = retryIntervals;
        }

        public override IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs)
        {
            var decoratedCall = DecoratedApi.PrepareServiceCall(appSecret, installId, logs);
            return new RetryableServiceCall(decoratedCall, _retryIntervals);
        }

        public override async Task ExecuteCallAsync(IServiceCall call)
        {
            var retryableCall = call as RetryableServiceCall;
            if (retryableCall == null)
            {
                await base.ExecuteCallAsync(call).ConfigureAwait(false);
                return;
            }
            await retryableCall.RunWithRetriesAsync().ConfigureAwait(false);
        }

        private static Func<Task> GetDelayFunc(TimeSpan[] intervals, int retry)
        {
            return async () =>
            {
                var delayMilliseconds = (int)(intervals[retry].TotalMilliseconds / 2.0);
                delayMilliseconds += await GetRandomIntAsync(delayMilliseconds).ConfigureAwait(false);
                var message = $"Try #{retry} failed and will be retried in {delayMilliseconds} ms";
                MobileCenterLog.Warn(MobileCenterLog.LogTag, message);
                await Task.Delay(delayMilliseconds).ConfigureAwait(false);
            };
        }

        private static async Task<int> GetRandomIntAsync(int max)
        {
            await RandomMutex.WaitAsync().ConfigureAwait(false);
            var randomInt = Random.Next(max);
            RandomMutex.Release();
            return randomInt;
        }
    }
}
