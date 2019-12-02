// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public partial class DependencyManager
    {
        public static IHttpNetworkAdapter HttpNetworkAdapter
        {
            get => PlatformHttpNetworkAdapter;
            set => PlatformHttpNetworkAdapter = value;
        }
    }
}
