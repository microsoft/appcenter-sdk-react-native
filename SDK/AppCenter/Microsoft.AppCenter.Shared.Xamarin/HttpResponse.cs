// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    internal partial class HttpResponse
    {
        internal int StatusCode
        {
            get => PlatformStatusCode;
            set => PlatformStatusCode = value;
        }

        internal string Content
        {
            get => PlatformContent;
            set => PlatformContent = value;
        }
    }
}
