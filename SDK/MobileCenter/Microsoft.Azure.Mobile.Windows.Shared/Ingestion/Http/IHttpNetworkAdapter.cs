using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public interface IHttpNetworkAdapter : IDisposable
    {
        Task<string> SendAsync(string uri, IDictionary<string, string> headers, string jsonContent, CancellationToken cancellationToken);
    }
}
