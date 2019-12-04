// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    internal static partial class DependencyConfiguration
    {
        internal static IHttpNetworkAdapter HttpNetworkAdapter
        {
            get => PlatformHttpNetworkAdapter;
            set => PlatformHttpNetworkAdapter = value;
        }
    }
}
