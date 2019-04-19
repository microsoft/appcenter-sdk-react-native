// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AppCenter.Auth.iOS.Bindings;

namespace Microsoft.AppCenter.Auth
{
    public partial class Auth : AppCenterService
    {
        private static Task<SignInResult> PlatformSignInAsync()
        {
            MSAuth.SignIn(null);
            return null;
        }
    }
}