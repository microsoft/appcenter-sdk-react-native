// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion
{
    using Microsoft.AppCenter;
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Microsoft.AppCenter.Ingestion.
    /// </summary>
    public static partial class Microsoft.AppCenter.IngestionExtensions
    {
            /// <summary>
            /// Send logs to the Ingestion service.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='appSecret'>
            /// A unique and secret key used to identify the application.
            /// </param>
            /// <param name='installID'>
            /// Installation identifier.
            /// </param>
            /// <param name='parameters'>
            /// Payload.
            /// </param>
            public static void Send(this IMicrosoftAppCenterIngestion operations, System.Guid appSecret, System.Guid installID, LogContainer parameters)
            {
                ((IMicrosoftAppCenterIngestion)operations).SendAsync(appSecret, installID, parameters).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Send logs to the Ingestion service.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='appSecret'>
            /// A unique and secret key used to identify the application.
            /// </param>
            /// <param name='installID'>
            /// Installation identifier.
            /// </param>
            /// <param name='parameters'>
            /// Payload.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task SendAsync(this IMicrosoftAppCenterIngestion operations, System.Guid appSecret, System.Guid installID, LogContainer parameters, CancellationToken cancellationToken = default(CancellationToken))
            {
                await operations.SendWithHttpMessagesAsync(appSecret, installID, parameters, null, cancellationToken).ConfigureAwait(false);
            }

    }
}

