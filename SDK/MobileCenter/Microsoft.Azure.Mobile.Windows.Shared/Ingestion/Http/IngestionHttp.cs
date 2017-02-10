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
    public partial class IngestionHttp : ServiceClient<IngestionHttp>, IIngestion
    {
        private const string DefaultBaseUrl = "https://in.mobile.azure.com";
        private const string ApiVersion = "/logs?api_version=1.0.0-preview20160914";
        private const string ContentTypeValue = "application/json; charset=utf-8";
        private const string AppSecret = "App-Secret";
        private const string InstallId = "Install-ID";
        private TimeSpan RequestTimeout = TimeSpan.FromMilliseconds(80000); //TODO not sure what to use here
        private const int MaximumCharactersDisplayedForAppSecret = 8;
        private string _baseUrl;

        public IngestionHttp()
        {
            _baseUrl = DefaultBaseUrl;
            this.HttpClient.Timeout = RequestTimeout;
        }

        public async Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            var logContainer = new LogContainer(logs);
            await SendHttpAsync(appSecret, installId, logContainer, cancellationToken);
        }

        public void Close()
        {
        }

        public void SetServerUrl(string serverUrl)
        {
            _baseUrl = serverUrl;
        }

        private async Task SendHttpAsync(string appSecret, Guid installId, LogContainer logContainer, CancellationToken cancellationToken)
        {
            /* Create HTTP transport objects */
            var request = new HttpRequestMessage();
            request.Method = new HttpMethod("POST");
            request.RequestUri = new Uri(_baseUrl + ApiVersion);
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, "Calling " + request.RequestUri.ToString() + " ...");

            /* Set Headers */
            request.Headers.Add(AppSecret, appSecret);
            request.Headers.Add(InstallId, installId.ToString());

            /* Log headers */
            string headers = string.Format("Headers: Content-Type={0}, {1}={2}, {3}={4}", 
                ContentTypeValue, AppSecret, GetRedactedAppSecret(appSecret), InstallId, installId.ToString());
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, headers);

            /* Save times */
            foreach (var log in logContainer.Logs)
            {
                log.Toffset = TimeHelper.CurrentTimeInMilliseconds() - log.Toffset;
            }

            /* Serialize Request */
            string requestContent = null;
            if (logContainer != null)
            {
                requestContent = LogSerializer.Serialize(logContainer);
                MobileCenterLog.Verbose(MobileCenterLog.LogTag, requestContent);
                request.Content = new StringContent(requestContent, System.Text.Encoding.UTF8);
                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(ContentTypeValue);
            }
            cancellationToken.ThrowIfCancellationRequested();
            HttpResponseMessage response = await this.HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, $"HTTP response status={(int)response.StatusCode} ({response.StatusCode}) payload={response.Content.AsString()}");
            cancellationToken.ThrowIfCancellationRequested();
            string responseContent = null;
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", response.StatusCode));
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
                response?.Dispose();
                throw ex;
            }
        }

        private string GetRedactedAppSecret(string appSecret)
        {
            int endHidingIndex = Math.Max(appSecret.Length - MaximumCharactersDisplayedForAppSecret, 0);
            string redactedAppSecret = "";
            for (int i = 0; i < endHidingIndex; ++i)
            {
                redactedAppSecret += '*';
            }
            redactedAppSecret += appSecret.Substring(endHidingIndex);
            return redactedAppSecret;
        }
    }
}

