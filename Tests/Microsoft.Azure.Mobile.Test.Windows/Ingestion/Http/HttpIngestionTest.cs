using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Moq;

namespace Microsoft.Azure.Mobile.Test.Ingestion.Http
{
    public class HttpIngestionTest
    {
        protected Mock<IHttpNetworkAdapter> _adapter;

        /// <summary>
        /// Helper for setup responce.
        /// </summary>
        protected void SetupAdapterSendResponse(HttpStatusCode statusCode)
        {
            var setup = _adapter
                .Setup(a => a.SendAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()));
            if (statusCode == HttpStatusCode.OK)
            {
                setup.Returns(Task.Run(() => ""));
            }
            else
            {
                setup.Throws(new HttpIngestionException("")
                {
                    StatusCode = (int)statusCode
                });
            }
        }

        /// <summary>
        /// Helper for verify SendAsync call.
        /// </summary>
        protected void VerifyAdapterSend(Times times)
        {
            _adapter
                .Verify(a => a.SendAsync(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()), times);
        }
    }
}
