// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Com.Microsoft.Appcenter;

namespace Microsoft.AppCenter
{
    static partial class DependencyConfigurator
    {
        private static IHttpNetworkAdapter PlatformHttpNetworkAdapter { get; set; }

        internal static void PlatformSetDependencies(IHttpNetworkAdapter httpNetworkAdapter = null)
        {
            PlatformHttpNetworkAdapter = httpNetworkAdapter;
            AndroidDependencyConfiguration.HttpClient = httpNetworkAdapter;
        }
    }
}
