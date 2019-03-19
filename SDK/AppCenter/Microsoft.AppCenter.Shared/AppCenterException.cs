// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter
{
    public class AppCenterException : Exception
    {
        public AppCenterException(string message) : base(message) { }
        public AppCenterException(string message, Exception innerException) : base(message, innerException) { }
    }
}
