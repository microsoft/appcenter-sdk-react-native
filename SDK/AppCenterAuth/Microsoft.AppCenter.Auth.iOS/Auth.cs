// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Auth.iOS.Bindings;

namespace Microsoft.AppCenter.Auth
{
    public partial class Auth : AppCenterService
    {
        [Preserve]
        public static Type BindingType => typeof(MSIdentity);

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSIdentity.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSIdentity.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        private static Task<UserInformation> PlatformSignInAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<UserInformation>();
            MSIdentity.SignIn((userInformation, error) =>
            {
                if (error != null)
                {
                    throw new NSErrorException(error);
                }
                taskCompletionSource.TrySetResult(new UserInformation
                {
                    AccountId = userInformation?.AccountId
                });
            });
            return taskCompletionSource.Task;
        }

        private static void PlatformSignOut()
        {
            MSIdentity.SignOut();
        }

        private static void PlatformSetConfigUrl(string configUrl)
        {
            MSIdentity.SetConfigUrl(configUrl);
        }
    }
}