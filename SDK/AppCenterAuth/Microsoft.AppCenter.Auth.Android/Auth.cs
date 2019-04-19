// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Com.Microsoft.Appcenter.Identity;

namespace Microsoft.AppCenter.Auth
{
    using AndroidSignInResult = Com.Microsoft.Appcenter.Identity.SignInResult;

    public partial class Auth
    {
        private static Task<SignInResult> PlatformSignInAsync()
        {
            var future = AndroidIdentity.SignIn();
            return Task.Run(() => {
                var result = (AndroidSignInResult)future.Get();
                var userInformation = result.UserInformation != null ? new UserInformation { AccountId = result.UserInformation.AccountId } : null;
                return new SignInResult
                {
                    UserInformation = userInformation,
                    Exception = result.Exception
                };
            });
        }
    }
}
