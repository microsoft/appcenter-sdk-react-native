// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net;

namespace Microsoft.AppCenter.Ingestion.Http
{
    /// <inheritdoc />
    public sealed partial class HttpNetworkAdapter
    {
        // Static initializer specific to windows desktop platforms.
        static HttpNetworkAdapter()
        {
            EnableTls12();
        }

        internal static void EnableTls12()
        {
            // ReSharper disable once InvertIf
            if ((ServicePointManager.SecurityProtocol & SecurityProtocolType.Tls12) != SecurityProtocolType.Tls12)
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                AppCenterLog.Debug(AppCenterLog.LogTag, "Enabled TLS 1.2 explicitly as it was disabled.");
            }
        }
    }
}
