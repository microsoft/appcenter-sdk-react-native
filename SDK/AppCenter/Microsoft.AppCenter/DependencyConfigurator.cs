// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    internal static partial class DependencyConfigurator
    {
        private static IHttpNetworkAdapter PlatformHttpNetworkAdapter { get; set; }

        internal static void PlatformSetDependencies(IHttpNetworkAdapter httpNetworkAdapter = null)
        {
        }
    }
}
