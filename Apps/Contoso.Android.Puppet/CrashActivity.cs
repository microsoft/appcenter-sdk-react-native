// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.App;
using Android.OS;

namespace Contoso.Android.Puppet
{
    [Activity(Label = "CrashActivity")]
    public class CrashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // will crash with super not called, a pure Java exception with no .NET crash handler.
        }
    }
}
