// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter
{
    internal class HttpException : Exception
    {
        internal HttpResponse HttpResponse { get; private set; }

        internal HttpException(HttpResponse httpResponse)
        {
            HttpResponse = httpResponse;
        }
    }
}
