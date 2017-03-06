// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class HttpIngestionException : IngestionException
    {
        public HttpMethod Method { get; set; }
        public Uri RequestUri { get; set; }
        public string RequestContent { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }

        public override bool IsRecoverable
        {
            get
            {
                var statusCode = (int)StatusCode;
                return statusCode >= 501 || statusCode == 408 || statusCode == 429 || statusCode == 401;
            }
        }

        public HttpIngestionException(string message) : base(message)
        {
        }
    }

    public class IngestionHttp : IIngestion
    {
        internal const string DefaultBaseUrl = "https://in.mobile.azure.com";
        internal const string ApiVersion = "/logs?api_version=1.0.0-preview20160914";
        internal const string ContentTypeValue = "application/json; charset=utf-8";
        internal const string AppSecret = "App-Secret";
        internal const string InstallId = "Install-ID";

        private readonly TimeSpan _requestTimeout = TimeSpan.FromMilliseconds(80000); //TODO not sure what to use here
        private const int MaximumCharactersDisplayedForAppSecret = 8;
        private string _baseUrl;
        private readonly IHttpNetworkAdapter _httpNetwork;

        public IngestionHttp() : this(new HttpNetworkAdapter())
        {
        }

        public IngestionHttp(IHttpNetworkAdapter httpNetwork)
        {
            _httpNetwork = httpNetwork;
            _httpNetwork.Timeout = _requestTimeout;
        }

        /// <exception cref="IngestionException"/>
        public async Task ExecuteCallAsync(IServiceCall call)
        {
            if (call.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            var requestContent = CreateLogsContent(call.Logs);
            var request = CreateRequest(call.AppSecret, call.InstallId, requestContent);
            HttpResponseMessage response = null;
            try
            {
                response = await _httpNetwork.SendAsync(request, call.CancellationToken).ConfigureAwait(false);
            }
            catch (IngestionException)
            {
                request.Dispose();
                throw;
            }
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, $"HTTP response status={(int) response.StatusCode} ({response.StatusCode}) payload={requestContent}");
            if (call.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var responseContent = string.Empty;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                var ex = new HttpIngestionException($"Operation returned an invalid status code '{response.StatusCode}'")
                {
                    Method = request.Method,
                    RequestUri = request.RequestUri,
                    RequestContent = requestContent,
                    StatusCode = response.StatusCode,
                    ResponseContent = responseContent
                };

                request.Dispose();
                response.Dispose();

                throw ex;
            }
        }

        public void Close()
        {
            //TODO is there anything to do here?
        }

        public void SetServerUrl(string serverUrl)
        {
            _baseUrl = serverUrl;
        }

        private static string GetRedactedAppSecret(string appSecret)
        {
            var endHidingIndex = Math.Max(appSecret.Length - MaximumCharactersDisplayedForAppSecret, 0);
            var redactedAppSecret = "";
            for (var i = 0; i < endHidingIndex; ++i)
            {
                redactedAppSecret += '*';
            }
            redactedAppSecret += appSecret.Substring(endHidingIndex);
            return redactedAppSecret;
        }

        public IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs)
        {
            return new HttpServiceCall(this, logs, appSecret, installId);
        }

        internal HttpRequestMessage CreateRequest(string appSecret, Guid installId, string requestContent)
        {
            var baseUrl = string.IsNullOrEmpty(_baseUrl) ? DefaultBaseUrl : _baseUrl;

            /* Create HTTP transport objects */
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl + ApiVersion)
            };
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, $"Calling {request.RequestUri}...");

            /* Set Headers */
            request.Headers.Add(AppSecret, appSecret);
            request.Headers.Add(InstallId, installId.ToString());

            /* Log headers */
            var headers = $"Headers: Content-Type={ContentTypeValue}, " +
                            $"{AppSecret}={GetRedactedAppSecret(appSecret)}, " +
                            $"{InstallId}={installId}";
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, headers);

            /* Request content */
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, requestContent);
            request.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
            request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(ContentTypeValue);

            return request;
        }

        private string CreateLogsContent(IList<Log> logs)
        {
            var logContainer = new LogContainer(logs);

            /* Save times */
            foreach (var log in logContainer.Logs)
            {
                log.Toffset = TimeHelper.CurrentTimeInMilliseconds() - log.Toffset;
            }

            /* Serialize Request */
            var requestContent = LogSerializer.Serialize(logContainer);
            return requestContent;
        }
    }
}
