// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion.Models.Serialization;

namespace Microsoft.Azure.Mobile.Ingestion.Http
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

        public IngestionHttp() : this(new HttpNetworkAdapter())
        {
        }

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
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, $"Calling {baseUrl + ApiVersion}...");
            var requestHeaders = CreateHeaders(call.AppSecret, call.InstallId);
            var requestContent = CreateLogsContent(call.Logs);
            await _httpNetwork.SendAsync(baseUrl + ApiVersion, requestHeaders, requestContent, call.CancellationToken).ConfigureAwait(false);
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

        private IDictionary<string, string> CreateHeaders(string appSecret, Guid installId)
        {
            // Log headers.
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, "Headers: " +
                    $"{AppSecret}={GetRedactedAppSecret(appSecret)}, " +
                    $"{InstallId}={installId}");
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
