// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using System.Net.Http;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public sealed class IngestionHttp : IIngestion
    {
        internal const string DefaultBaseUrl = "https://in.mobile.azure.com";
        internal const string ApiVersion = "/logs?api_version=1.0.0-preview20160914";
        internal const string AppSecret = "App-Secret";
        internal const string InstallId = "Install-ID";
        
        private const int MaximumCharactersDisplayedForAppSecret = 8;
        private string _baseLogUrl;
        private readonly IHttpNetworkAdapter _httpNetwork;

        public IngestionHttp(IHttpNetworkAdapter httpNetwork)
        {
            _httpNetwork = httpNetwork;
        }

        /// <exception cref="IngestionException"/>
        public async Task ExecuteCallAsync(IServiceCall call)
        {
            if (call.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            var baseUrl = string.IsNullOrEmpty(_baseLogUrl) ? DefaultBaseUrl : _baseLogUrl;
            AppCenterLog.Verbose(AppCenterLog.LogTag, $"Calling {baseUrl + ApiVersion}...");

            // Create request headers.
            var requestHeaders = CreateHeaders(call.AppSecret, call.InstallId);
            AppCenterLog.Verbose(AppCenterLog.LogTag, "Headers: " +
                    $"{AppSecret}={GetRedactedAppSecret(call.AppSecret)}, " +
                    $"{InstallId}={call.InstallId}");

            // Create request content.
            var requestContent = CreateLogsContent(call.Logs);
            AppCenterLog.Verbose(AppCenterLog.LogTag, requestContent);
            
            // Send request.
            await _httpNetwork.SendAsync(baseUrl + ApiVersion, HttpMethod.Post, requestHeaders, requestContent, call.CancellationToken).ConfigureAwait(false);
        }

        public void Close()
        {
            //No-op
        }

        public void SetLogUrl(string logUrl)
        {
            _baseLogUrl = logUrl;
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

        internal IDictionary<string, string> CreateHeaders(string appSecret, Guid installId)
        {
            return new Dictionary<string, string>
            {
                { AppSecret, appSecret },
                { InstallId, installId.ToString() }
            };
        }

        private string CreateLogsContent(IList<Log> logs)
        {
            var logContainer = new LogContainer(logs);

            // Serialize request.
            var requestContent = LogSerializer.Serialize(logContainer);
            return requestContent;
        }

        public void Dispose()
        {
            _httpNetwork.Dispose();
        }
    }
}
