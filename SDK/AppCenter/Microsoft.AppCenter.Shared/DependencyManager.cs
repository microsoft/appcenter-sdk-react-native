// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    internal static partial class DependencyManager
    {
        internal static IHttpNetworkAdapter HttpNetworkAdapter { get; } = PlatformHttpNetworkAdapter;

        internal static void SetDependencies(IHttpNetworkAdapter httpNetworkAdapter = null)
        {
            PlatformHttpNetworkAdapter(httpNetworkAdapter);
        }
    }
}
