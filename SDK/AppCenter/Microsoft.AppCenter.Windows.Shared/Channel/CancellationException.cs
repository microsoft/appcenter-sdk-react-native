// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public class CancellationException : AppCenterException
    {
        public CancellationException() : base("Request cancelled because channel is disabled.") { }
    }
}
