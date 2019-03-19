// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public static class WrapperSdk
    {
        public const string Name = "appcenter.xamarin";

        /* We can't use reflection for assemblyInformationalVersion on iOS with "Link All" optimization. */
        internal const string Version = "1.14.1-SNAPSHOT";
    }
}
