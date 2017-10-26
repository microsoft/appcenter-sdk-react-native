using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Ingestion.Http
{
    public interface IHttpNetworkAdapter : IDisposable
    {
        Task<string> SendAsync(string uri, HttpMethod method, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken);
    }
}
