// Copyright (c) Microsoft Corporation.  All rights reserved.
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Net;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class IngestionHttp : ServiceClient<IngestionHttp>, IIngestion
    {
        private const string DefaultBaseUrl = "https://in.mobile.azure.com";
        private const string ApiVersion = "/logs?api_version=1.0.0-preview20160914";
        private const string ContentTypeValue = "application/json; charset=utf-8";
        private const string AppSecret = "App-Secret";
        private const string InstallId = "Install-ID";
        private readonly TimeSpan _requestTimeout = TimeSpan.FromMilliseconds(80000); //TODO not sure what to use here
        private const int MaximumCharactersDisplayedForAppSecret = 8;
        private string _baseUrl;

        public IngestionHttp()
        {
            _baseUrl = DefaultBaseUrl;
            this.HttpClient.Timeout = _requestTimeout;
        }

        /// <exception cref="IngestionException"/>
        public async Task ExecuteCallAsync(IServiceCall call)
        {
            var logContainer = new LogContainer(call.Logs);
            /* Create HTTP transport objects */
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("POST"),
                RequestUri = new Uri(_baseUrl + ApiVersion)
            };
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, $"Calling {request.RequestUri}...");

            /* Set Headers */
            request.Headers.Add(AppSecret, call.AppSecret);
            request.Headers.Add(InstallId, call.InstallId.ToString());

            /* Log headers */
            var headers = $"Headers: Content-Type={ContentTypeValue}, " +
                          $"{AppSecret}={GetRedactedAppSecret(call.AppSecret)}, " +
                          $"{InstallId}={call.InstallId}";
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, headers);

            /* Save times */
            foreach (var log in logContainer.Logs)
            {
                log.Toffset = TimeHelper.CurrentTimeInMilliseconds() - log.Toffset;
            }

            /* Serialize Request */
            var requestContent = LogSerializer.Serialize(logContainer);
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, requestContent);
            request.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
            request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(ContentTypeValue);

            call.CancellationToken.ThrowIfCancellationRequested();
            HttpResponseMessage response = null;
            try
            {
                response = await this.HttpClient.SendAsync(request, call.CancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                request.Dispose();
                throw new IngestionException(ex);
            }
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, $"HTTP response status={(int)response.StatusCode} ({response.StatusCode}) payload={response.Content.AsString()}");
            call.CancellationToken.ThrowIfCancellationRequested();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string responseContent;

                var ex = new HttpOperationException($"Operation returned an invalid status code '{response.StatusCode}'");
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    responseContent = string.Empty;
                }
                ex.Request = new HttpRequestMessageWrapper(request, requestContent);
                ex.Response = new HttpResponseMessageWrapper(response, responseContent);
                request.Dispose();
                response.Dispose();
                throw new IngestionException(ex);
            }
        }

        public void Close()
        {
            
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
    }
}

