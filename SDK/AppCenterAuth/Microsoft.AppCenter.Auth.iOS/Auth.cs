// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Auth.iOS.Bindings;

namespace Microsoft.AppCenter.Auth
{
    public partial class Auth : AppCenterService
    {
        private static Task<SignInResult> PlatformSignInAsync()
        {
            var promise = new TaskCompletionSource<SignInResult>();

            MSIdentity.SignIn((userInformation, error) =>
            {
                SignInResult result = new SignInResult();
                result.UserInformation = new UserInformation();
                result.UserInformation.AccountId = userInformation.AccountId;
                result.Exception = new Exception(error.LocalizedDescription);
                promise.TrySetResult(result);
            });
            return promise.Task;
        }
    }
}