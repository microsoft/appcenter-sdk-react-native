// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    static partial class DependencyConfiguration
    {
        private static IHttpNetworkAdapter _httpNetworkAdapter;
        private static IHttpNetworkAdapter PlatformHttpNetworkAdapter
        {
            get => _httpNetworkAdapter;
            set
            {
                // TODO: Implement iOS.
                _httpNetworkAdapter = value;
            }
        }
    }
}
