// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Com.Microsoft.Appcenter;

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
                AndroidDependencyConfiguration.HttpClient = new AndroidHttpClientAdapter(value);
                _httpNetworkAdapter = value;
            }
        }
    }
}
