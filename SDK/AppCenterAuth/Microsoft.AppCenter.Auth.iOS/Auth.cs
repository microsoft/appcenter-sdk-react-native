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

        private static void PlatformSetConfigUrl(string configUrl)
        {
            MSIdentity.SetConfigUrl(configUrl);
        }

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
                    taskCompletionSource.TrySetException(new NSErrorException(error));
                }
                else
                {
                    taskCompletionSource.TrySetResult(new UserInformation
                    {
                        AccountId = userInformation.AccountId
                    });
                }
            });
            return taskCompletionSource.Task;
        }

        private static void PlatformSignOut()
        {
            MSIdentity.SignOut();
        }

        /// <summary>
        /// Process URL request for the Auth service.
        /// Place this method call into app delegate openUrl method.
        /// </summary>
        /// <param name="url">The url with parameters.</param>
        public static void OpenUrl(NSUrl url)
        {
            MSIdentity.OpenUrl(url);
        }
    }
}