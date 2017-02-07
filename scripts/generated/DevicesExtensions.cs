// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.Azure.Mobile.UWP.Ingestion
{
    using Microsoft.Azure;
    using Microsoft.Azure.Mobile;
    using Microsoft.Azure.Mobile.UWP;
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Devices.
    /// </summary>
    public static partial class DevicesExtensions
    {
            /// <summary>
            /// Gets the device model mappings in CSV format
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static Stream ModelMappingCsv(this IDevices operations)
            {
                return operations.ModelMappingCsvAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets the device model mappings in CSV format
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Stream> ModelMappingCsvAsync(this IDevices operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                var _result = await operations.ModelMappingCsvWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false);
                _result.Request.Dispose();
                return _result.Body;
            }

            /// <summary>
            /// Gets the device model mappings in JSON format
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static IDictionary<string, DeviceMapping> ModelMappingJson(this IDevices operations)
            {
                return operations.ModelMappingJsonAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets the device model mappings in JSON format
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IDictionary<string, DeviceMapping>> ModelMappingJsonAsync(this IDevices operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ModelMappingJsonWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Gets the device model mapping
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static DeviceMapping ModelMapping(this IDevices operations)
            {
                return operations.ModelMappingAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Gets the device model mapping
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<DeviceMapping> ModelMappingAsync(this IDevices operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ModelMappingWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}

