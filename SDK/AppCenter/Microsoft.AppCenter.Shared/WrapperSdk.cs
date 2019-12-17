// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public partial class WrapperSdk
    {
        public const string Name = "appcenter.xamarin";

        /* We can't use reflection for assemblyInformationalVersion on iOS with "Link All" optimization. */
        internal const string Version = "2.6.5-SNAPSHOT";
    }
}
