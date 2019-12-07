// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.AppCenter.Test.Functional
{
    internal class HttpNetworkAdapter : IHttpNetworkAdapter
    {
        private readonly int _expectedStatusCode;
        private readonly string _expectedContent;
        private readonly string _expectedLogType;

        private readonly TaskCompletionSource<HttpResponse> _taskCompletionSource = new TaskCompletionSource<HttpResponse>();

        internal Task<HttpResponse> HttpResponseTask { get; private set; }

        internal string Uri { get; private set; }
        internal string Method { get; private set; }
        internal IDictionary<string, string> Headers { get; private set; }
        internal JObject JsonContent { get; private set; }
        internal int CallCount { get; private set; }

        internal HttpNetworkAdapter(int expectedStatusCode = 200, string expectedContent = "", string expectedLogType = null)
        {
            _expectedStatusCode = expectedStatusCode;
            _expectedContent = expectedContent;
            _expectedLogType = expectedLogType;
            HttpResponseTask = _taskCompletionSource.Task;
        }

        public Task<HttpResponse> SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken)
        {
            var response = new HttpResponse
            {
                StatusCode = _expectedStatusCode,
                Content = _expectedContent
            };
            var jsonLogContainer = JObject.Parse(jsonContent);
            if (string.IsNullOrEmpty(_expectedLogType) || jsonLogContainer.SelectTokens($"$.logs[?(@.type == '{_expectedLogType}')]").ToList().Count > 0)
            {
                Uri = uri;
                Method = method;
                Headers = headers;
                JsonContent = jsonLogContainer;
                lock (this)
                {
                    CallCount++;
                }
                _taskCompletionSource.TrySetResult(response);
                return HttpResponseTask;
            }
            return Task.FromResult(response);
        }

        public void Dispose()
        {
        }
    }
}
