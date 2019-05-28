// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.App;
using Android.Runtime;
using Firebase.Iid;

namespace Microsoft.AppCenter.Push
{
    [Preserve]
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class TokenService : FirebaseInstanceIdService
    {
        public override void OnTokenRefresh()
        {
            Com.Microsoft.Appcenter.Push.AndroidPush.Instance.OnTokenRefresh(FirebaseInstanceId.Instance.Token);
        }
    }
}
