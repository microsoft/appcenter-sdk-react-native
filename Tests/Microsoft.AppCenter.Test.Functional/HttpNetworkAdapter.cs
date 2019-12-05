// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Test.Functional
{
    internal class HttpNetworkAdapter : IHttpNetworkAdapter
    {
        private readonly int _expectedStatusCode;
        private readonly string _expectedContent;

        private readonly TaskCompletionSource<HttpResponse> _taskCompletionSource = new TaskCompletionSource<HttpResponse>();

        internal Task<HttpResponse> HttpResponseTask { get; private set; }

        internal string Uri { get; private set; }
        internal string Method { get; private set; }
        internal IDictionary<string, string> Headers { get; private set; }
        internal string JsonContent { get; private set; }

        internal HttpNetworkAdapter(int expectedStatusCode = 200, string expectedContent = "")
        {
            _expectedStatusCode = expectedStatusCode;
            _expectedContent = expectedContent;
            HttpResponseTask = _taskCompletionSource.Task;
        }

        public Task<HttpResponse> SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken)
        {
            var response = new HttpResponse
            {
                StatusCode = _expectedStatusCode,
                Content = _expectedContent
            };

            if (jsonContent.Contains("event"))
            {
                Uri = uri;
                Method = method;
                Headers = headers;
                JsonContent = jsonContent;

                _taskCompletionSource.SetResult(response);
                return HttpResponseTask;
            }
            return Task.FromResult(response);
        }

        public void Dispose()
        {
        }
    }
}
