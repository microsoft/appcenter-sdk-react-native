// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.Runtime;

namespace Microsoft.AppCenter.Push
{
    [Preserve]
    class TokenService : Com.Microsoft.Appcenter.Push.TokenService
    {
        /* We don't use that subclass, it's only for preserving the parent Java class when linker is used. */
    }
}
