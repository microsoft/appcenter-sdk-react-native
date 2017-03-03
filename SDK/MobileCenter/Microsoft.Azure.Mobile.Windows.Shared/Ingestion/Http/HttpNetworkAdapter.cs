using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class HttpNetworkAdapter : ServiceClient<HttpNetworkAdapter>, IHttpNetworkAdapter
	{
		public TimeSpan Timeout
		{
			get { return this.HttpClient.Timeout; }
			set { this.HttpClient.Timeout = value; }
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return this.HttpClient.SendAsync(request, cancellationToken);
		}
	}
}
