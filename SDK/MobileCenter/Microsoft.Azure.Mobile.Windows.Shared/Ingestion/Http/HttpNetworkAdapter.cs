using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public sealed class HttpNetworkAdapter : IHttpNetworkAdapter
    {
        private HttpClient _httpClient;
        private TimeSpan? _timeout;
        private object _lockObject = new object();

        public HttpNetworkAdapter(TimeSpan? timeout = null)
        {
            _timeout = timeout;
        }

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

                    if (_timeout.HasValue)
                    {
                        _httpClient.Timeout = _timeout.Value;
                    }
                    return _httpClient;
                }
            }
        }

        /// <exception cref="IngestionException"/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException e)
            {
                throw new IngestionException(e);
            }
            catch (HttpRequestException e)
            {
                throw new IngestionException(e);
            }
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                    _httpClient = null;
                }
            } 
        }
    }
}
