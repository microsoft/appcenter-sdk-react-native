// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public sealed partial class HttpNetworkAdapter : IHttpNetworkAdapter
    {
        internal const string ContentTypeValue = "application/json; charset=utf-8";

        private HttpClient _httpClient;
        private readonly object _lockObject = new object();

        // Exception codes (HResults) involving poor network connectivity:
        private static readonly uint[] NetworkUnavailableCodes =
        {
            0x80072EE7, // WININET_E_NAME_NOT_RESOLVED
            0x80072EFD, // WININET_E_CANNOT_CONNECT
            0x80072EFE, // WININET_E_CONNECTION_ABORTED
            0x80072EFF  // WININET_E_CONNECTION_RESET
        };

        private HttpClient HttpClient
        {
            get
            {
                lock (_lockObject)
                {
                    if (_httpClient != null)
                    {
                        return _httpClient;
                    }

                    _httpClient = new HttpClient();
                    return _httpClient;
                }
            }
        }

        /// <exception cref="IngestionException"/>
        public async Task<string> SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken)
        {
            using (var request = CreateRequest(uri, method, headers, jsonContent))
            using (var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false))
            {
                if (response == null)
                {
                    throw new IngestionException("Null response received");
                }
                var responseContent = "(null)";
                string contentType = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (response.Content.Headers.TryGetValues("Content-Type", out var contentTypeHeaders))
                    {
                        contentType = contentTypeHeaders.FirstOrDefault();
                    }
                }
                string logPayload;
                if (contentType == null || contentType.StartsWith("text/") || contentType.StartsWith("application/"))
                {
                    logPayload = responseContent;
                }
                else
                {
                    logPayload = "<binary>";
                }
                var logMessage = $"HTTP response status={(int)response.StatusCode} ({response.StatusCode}) payload={logPayload}";
                AppCenterLog.Verbose(AppCenterLog.LogTag, logMessage);
                if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300)
                {
                    throw new HttpIngestionException(logMessage)
                    {
                        Method = request.Method.ToString(),
                        RequestUri = request.RequestUri,
                        StatusCode = (int)response.StatusCode,
                        RequestContent = jsonContent,
                        ResponseContent = responseContent
                    };
                }
                return responseContent;
            }
        }

        internal HttpRequestMessage CreateRequest(string uri, string method, IDictionary<string, string> headers, string jsonContent)
        {
            // Create HTTP transport objects.
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(uri),
            };

            // Set Headers.
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            // Request content.
            request.Content = new StringContent(jsonContent, Encoding.UTF8);
            request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(ContentTypeValue);
            return request;
        }

        /// <summary>
        /// Asynchronously makes an HTTP request.
        /// </summary>
        /// <param name="request">The request message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task containing the HTTP response</returns>
        /// <exception cref="IngestionException"/>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException e)
            {
                throw new IngestionException(e);
            }
            catch (Exception e)
            {
                // If the HResult indicates a network outage, throw a NetworkIngestionException so
                // it can be dealt with properly
                if (Array.Exists(NetworkUnavailableCodes, code => code == (uint)e.HResult))
                {
                    throw new NetworkIngestionException();
                }
                throw;
            }
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                _httpClient?.Dispose();
                _httpClient = null;
            }
        }
    }
}
