// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Ingestion.Models;
using Moq;
using Moq.Language.Flow;

namespace Microsoft.AppCenter.Test.Windows.Ingestion.Http
{
    public class HttpIngestionTest
    {
        protected Mock<IHttpNetworkAdapter> _adapter;

        protected string AppSecret => Guid.NewGuid().ToString();
        protected Guid InstallId => Guid.NewGuid();
        protected IList<Log> Logs => new List<Log>();

        /// <summary>
        /// Helper for setup response.
        /// </summary>
        protected ISetup<IHttpNetworkAdapter, Task<string>> SetupAdapterSendResponse(params HttpStatusCode[] statusCodes)
        {
            var index = 0;
            var setup = _adapter
                .Setup(a => a.SendAsync(
                    It.IsAny<string>(),
                    "POST",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()));
            setup.Returns(() =>
            {
                var statusCode = statusCodes[index < statusCodes.Length ? index++ : statusCodes.Length - 1];
                return ((int)statusCode >= 200 && (int)statusCode < 300)
                    ? TaskExtension.GetCompletedTask("")
                    : TaskExtension.GetFaultedTask<string>(new HttpIngestionException("")
                    {
                        StatusCode = (int) statusCode
                    });
            });
            return setup;
        }

        /// <summary>
        /// Helper for verify SendAsync call.
        /// </summary>
        protected void VerifyAdapterSend(Times times)
        {
            _adapter
                .Verify(a => a.SendAsync(
                    It.IsAny<string>(),
                    "POST",
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()), times);
        }
    }
}
