// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    using AndroidHttpResponse = Com.Microsoft.Appcenter.Http.AndroidHttpResponse;

    internal partial class HttpResponse
    {
        private int PlatformStatusCode { get; set; }
        private string PlatformContent { get; set;}

        internal HttpResponse(AndroidHttpResponse httpResponse)
        {
            PlatformStatusCode = httpResponse.StatusCode;
            PlatformContent = httpResponse.Payload;
        }
    }
}
