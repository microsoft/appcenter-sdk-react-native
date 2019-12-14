// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using Com.Microsoft.Appcenter.Http;

namespace Microsoft.AppCenter
{
    internal class AndroidHttpClientAdapter : Java.Lang.Object, IAndroidHttpClient
    {
        private readonly IHttpNetworkAdapter _httpNetworkAdapter;

        public AndroidHttpClientAdapter(IHttpNetworkAdapter httpNetworkAdapter)
        {
            _httpNetworkAdapter = httpNetworkAdapter;
        }

        public IServiceCall CallAsync(string uri, string method, IDictionary<string, string> headers, IAndroidHttpClientCallTemplate callTemplate, IServiceCallback serviceCallback)
        {
            callTemplate?.OnBeforeCalling(new Java.Net.URL(uri), headers);
            var jsonContent = callTemplate?.BuildRequestBody();
            var cancellationTokenSource = new CancellationTokenSource();
            _httpNetworkAdapter.SendAsync(uri, method, headers, jsonContent, cancellationTokenSource.Token).ContinueWith(t =>
            {
                var innerException = t.Exception?.InnerException;
                if (innerException is HttpException)
                {
                    var response = (innerException as HttpException).HttpResponse;
                    serviceCallback.OnCallFailed(new AndroidHttpException(new AndroidHttpResponse(response.StatusCode, response.Content)));
                }
                else if (innerException != null)
                {
                    serviceCallback.OnCallFailed(new Java.Lang.Exception(innerException.Message));
                }
                else
                {
                    var response = t.Result;
                    serviceCallback.OnCallSucceeded(new AndroidHttpResponse(response.StatusCode, response.Content));
                }
            });
            return new ServiceCall(cancellationTokenSource);
        }

        public void Close()
        {
        }

        public void Reopen()
        {
        }
    }

    internal class ServiceCall : Java.Lang.Object, IServiceCall
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal ServiceCall(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
