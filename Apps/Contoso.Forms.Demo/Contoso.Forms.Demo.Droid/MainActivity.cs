// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.AppCenter.Push;

namespace Contoso.Forms.Demo.Droid
{
    [Activity(Label = "ACFDemo", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static readonly int FileAttachmentId = 1;

        public TaskCompletionSource<string> FileAttachmentTaskCompletionSource { set; get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Push.CheckLaunchedFromNotification(this, intent);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == FileAttachmentId)
            {
                var uri = resultCode == Result.Ok && data != null ? data.Data : null;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat && uri != null)
                {
                    ContentResolver.TakePersistableUriPermission(uri, data.Flags & ActivityFlags.GrantReadUriPermission);
                }
                FileAttachmentTaskCompletionSource.SetResult(uri?.ToString());
            }
        }
    }
}
