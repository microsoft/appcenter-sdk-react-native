// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Ingestion.Http
{
    internal sealed class NetworkStateIngestion : IngestionDecorator
    {
        private readonly ISet<ServiceCall> _calls = new HashSet<ServiceCall>();

        private readonly INetworkStateAdapter _networkStateAdapter;

        public NetworkStateIngestion(IIngestion decoratedApi, INetworkStateAdapter networkStateAdapter)
            : base(decoratedApi)
        {
            _networkStateAdapter = networkStateAdapter;
            _networkStateAdapter.NetworkStatusChanged += OnNetworkStateChange;
        }

        private void OnNetworkStateChange(object sender, EventArgs e)
        {
            if (!_networkStateAdapter.IsConnected)
            {
                return;
            }
            var calls = new List<ServiceCall>();
            lock (_calls)
            {
                calls.AddRange(_calls);
                _calls.Clear();
            }
            foreach (var call in calls)
            {
                RetryCall(call);
            }
        }

        private void RetryCall(ServiceCall call)
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

            // Pass result.
            result.ContinueWith(call.CopyState);
        }

        public override IServiceCall Call(string appSecret, Guid installId, IList<Log> logs)
        {
            if (_networkStateAdapter.IsConnected)
            {
                return base.Call(appSecret, installId, logs);
            }
            var call = new ServiceCall(appSecret, installId, logs);
            lock (_calls)
            {
                _calls.Add(call);
            }
            return call;
        }

        public override void Close()
        {
            CancelAllCalls();
            base.Close();
        }

        public override void Dispose()
        {
            _networkStateAdapter.NetworkStatusChanged -= OnNetworkStateChange;
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
                calls = new List<ServiceCall>(_calls);
                _calls.Clear();
            }
            foreach (var call in calls)
            {
                call.Cancel();
            }
        }
    }
}
