// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public partial class HttpResponse
    {
        public int StatusCode
        {
            get => PlatformStatusCode;
            set => PlatformStatusCode = value;
        }

        public string Content
        {
            get => PlatformContent;
            set => PlatformContent = value;
        }
    }
}
