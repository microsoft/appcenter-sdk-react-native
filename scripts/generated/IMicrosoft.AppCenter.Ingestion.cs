// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.AppCenter.Ingestion
{
    using Microsoft.AppCenter;
    using Microsoft.Rest;
    using Models;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Microsoft Avalanche Ingestion REST API.
    /// </summary>
    public partial interface IMicrosoft.Ingestion : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// API Version.
        /// </summary>
        string ApiVersion { get; set; }


        /// <summary>
        /// Send logs to the Ingestion service.
        /// </summary>
        /// <param name='appSecret'>
        /// A unique and secret key used to identify the application.
        /// </param>
        /// <param name='installID'>
        /// Installation identifier.
        /// </param>
        /// <param name='parameters'>
        /// Payload.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<Microsoft.Rest.HttpOperationResponse> SendWithHttpMessagesAsync(System.Guid appSecret, System.Guid installID, LogContainer parameters, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

    }
}

