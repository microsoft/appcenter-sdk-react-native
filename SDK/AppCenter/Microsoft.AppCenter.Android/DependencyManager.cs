// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Com.Microsoft.Appcenter;

namespace Microsoft.AppCenter
{
    public partial class DependencyManager
    {
        static void PlatformSetDependencies(System.Net.Http.HttpClient httpClient)
        {
            AndroidDependencyManager.PlatformSetDependencies(httpClient);
        }
    }
}
