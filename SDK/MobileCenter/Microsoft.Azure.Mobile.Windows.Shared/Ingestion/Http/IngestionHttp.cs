// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.Rest;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
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
		private IHttpNetworkAdapter _httpNetwork;

		public IngestionHttp() :
			this(new HttpNetworkAdapter())
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
			HttpRequestMessage request = CreateRequest(call.AppSecret, call.InstallId, call.Logs);
			HttpResponseMessage response = null;
            try
            {
                response = await _httpNetwork.SendAsync(request, call.CancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                request.Dispose();
                throw new IngestionException(ex);
            }
            MobileCenterLog.Verbose(MobileCenterLog.LogTag, $"HTTP response status={(int)response.StatusCode} ({response.StatusCode}) payload={response.Content.AsString()}");
            if (call.CancellationToken.IsCancellationRequested)
            {
                return;
            }
			if (response.StatusCode != HttpStatusCode.OK)
			{
				await ThrowHttpOperationException(request, response).ConfigureAwait(false);
            }
        }

        public void Close()
        {
			// TODO What if re-enabled after?
			// _httpNetwork.Dispose();
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

		internal async Task ThrowHttpOperationException(HttpRequestMessage request, HttpResponseMessage response)
		{
			var requestContent = string.Empty;
			var responseContent = string.Empty;

			var ex = new HttpOperationException($"Operation returned an invalid status code '{response.StatusCode}'");
			if (request.Content != null)
			{
				requestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			if (response.Content != null)
			{
				responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			ex.Request = new HttpRequestMessageWrapper(request, requestContent);
			ex.Response = new HttpResponseMessageWrapper(response, responseContent);
			request.Dispose();
			response.Dispose();

			throw new IngestionException(ex);
		}

		internal HttpRequestMessage CreateRequest(string appSecret, Guid installId, IList<Log> logs)
		{
			var logContainer = new LogContainer(logs);
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

			return request;
		}
	}
}

