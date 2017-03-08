using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public interface IHttpNetworkAdapter
    {
        TimeSpan Timeout { get; set; }

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }

    public class HttpNetworkAdapter : IHttpNetworkAdapter
    {
        private readonly HttpClient _httpClient = new HttpClient();
        public TimeSpan Timeout
        {
            get { return _httpClient.Timeout; }
            set { _httpClient.Timeout = value; }
        }

        /// <exception cref="IngestionException"/>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
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
    }
}
