// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Contoso.Forms.Demo.Droid;
using Microsoft.AppCenter.Push;
using Xamarin.Forms;

[assembly: Dependency(typeof(MainActivity))]
namespace Contoso.Forms.Demo.Droid
{
    [Activity(Label = "ACFDemo", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IClearCrashClick
    {
        public static readonly int FileAttachmentId = 1;

        private const string CrashesUserConfirmationStorageKey = "com.microsoft.appcenter.crashes.always.send";

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

        public void ClearCrashButton()
        {
            ISharedPreferences appCenterPreferences = Android.App.Application.Context.GetSharedPreferences("AppCenter", FileCreationMode.Private);
            appCenterPreferences.Edit().Remove(CrashesUserConfirmationStorageKey).Apply();
        }
    }
}