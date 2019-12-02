// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public partial class DependencyManager
    {
        internal DependencyManager()
        {
        }

        static IHttpNetworkAdapter PlatformHttpNetworkAdapter { get; set; }
    }
}
