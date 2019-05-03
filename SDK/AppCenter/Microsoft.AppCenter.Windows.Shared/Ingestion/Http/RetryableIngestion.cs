// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http
{
    internal sealed class RetryableIngestion : IngestionDecorator
    {
        private static readonly TimeSpan[] DefaultIntervals =
        {
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromMinutes(20)
        };
        
        private readonly IDictionary<ServiceCall, Timer> _calls = new Dictionary<ServiceCall, Timer>();

        private readonly TimeSpan[] _retryIntervals;

        public RetryableIngestion(IIngestion decoratedApi)
            : base(decoratedApi)
        {
            var random = new Random();
            _retryIntervals = DefaultIntervals.Select(defaultInterval =>
            {
                var interval = (int)(defaultInterval.TotalMilliseconds / 2.0);
                interval += random.Next(interval);
                return TimeSpan.FromMilliseconds(interval);
            }).ToArray();
        }

        public RetryableIngestion(IIngestion decoratedApi, TimeSpan[] retryIntervals)
            : base(decoratedApi)
        {
            _retryIntervals = retryIntervals ?? throw new ArgumentNullException(nameof(retryIntervals));
        }

        private Timer IntervalCall(int retry, Action action)
        {
            var interval = (int)_retryIntervals[retry - 1].TotalMilliseconds;
            AppCenterLog.Warn(AppCenterLog.LogTag, $"Try #{retry} failed and will be retried in {interval} ms");
            return new Timer(state => action(), null, interval, Timeout.Infinite);
        }

        private void RetryCall(ServiceCall call, int retry)
        {
            if (call.IsCanceled)
            {
                return;
            }
            var result = base.Call(call.AppSecret, call.InstallId, call.Logs);

            // Cancel retry if cancel call.
            call.ContinueWith(_ =>
            {
                if (call.IsCanceled && !result.IsCanceled)
                {
                    result.Cancel();
                }
            });
            result.ContinueWith(_ => RetryCallContinuation(call, result, retry + 1));
        }

        private void RetryCallContinuation(ServiceCall call, IServiceCall result, int retry)
        {
            // Canceled.
            if (call.IsCanceled)
            {
                return;
            }
            if (result.IsCanceled)
            {
                call.Cancel();
                return;
            }

            // Faulted.
            if (result.IsFaulted)
            {
                var isRecoverable = result.Exception is IngestionException ingestionException && ingestionException.IsRecoverable;
                if (!isRecoverable || retry - 1 >= _retryIntervals.Length)
                {
                    call.SetException(result.Exception);
                    return;
                }

                // Shedule next retry.
                var timer = IntervalCall(retry, () =>
                {
                    lock (_calls)
                    {
                        if (!_calls.Remove(call))
                        {
                            return;
                        }
                    }
                    RetryCall(call, retry);
                });
                lock (_calls)
                {
                    _calls.Add(call, timer);
                }
                return;
            }

            // Succeeded.
            call.SetResult(result.Result);
        }

        public override IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
        {
            var call = new ServiceCall(appSecret, installId, logs);
            RetryCall(call, 0);
            return call;
        }

        public override void Close()
        {
            CancelAllCalls();
            base.Close();
        }

        public override void Dispose()
        {
            CancelAllCalls();
            base.Dispose();
        }

        private void CancelAllCalls()
        {
            IList<ServiceCall> calls;
            lock (_calls)
            {
                if (_calls.Count == 0)
                {
                    return;
                }
                calls = _calls.Select(i => i.Key).ToList();
                foreach (var timer in _calls.Values)
                {
                    timer.Dispose();
                }
                _calls.Clear();
            }
            foreach (var call in calls)
            {
                call.Cancel();
            }
        }
    }
}
