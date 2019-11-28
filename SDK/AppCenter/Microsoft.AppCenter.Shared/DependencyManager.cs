// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public partial class DependencyManager
    {
        /// <summary>
        /// Inject dependencies.
        /// </summary>
        /// <param name="httpClient">Http client.</param>
        public static void SetDependencies(System.Net.Http.HttpClient httpClient)
        {
            PlatformSetDependencies(httpClient);
        }
    }
}
