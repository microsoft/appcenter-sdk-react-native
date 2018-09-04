using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Com.Microsoft.Appcenter.Analytics;
using Com.Microsoft.Appcenter.Analytics.Channel;
using Com.Microsoft.Appcenter.Ingestion.Models;
using HockeyApp.Android;
using HockeyApp.Android.Utils;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "ACFPuppet", Icon = "@drawable/icon", Theme = "@style/PuppetTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
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

            AndroidAnalytics.SetListener(new AndroidAnalyticsListener());
            LoadApplication(new App());
        }

        protected override void OnResume()
        {
            base.OnResume();
            HockeyLog.LogLevel = 2;
            CrashManager.Register(this, "760386e0bff149268f270f30fde3d6e4");
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

    public class AndroidAnalyticsListener : Java.Lang.Object, IAnalyticsListener
    {
        public void OnSendingFailed(ILog log, Java.Lang.Exception e)
        {
            AppCenterLog.Debug(App.LogTag, "Analytics listener OnSendingFailed with exception: " + e);
        }

        public void OnSendingSucceeded(ILog log)
        {
            AppCenterLog.Debug(App.LogTag, "Analytics listener OnSendingSucceeded");
        }

        public void OnBeforeSending(ILog log)
        {
            AppCenterLog.Debug(App.LogTag, "Analytics listener OnBeforeSendingEventLog");
        }
    }
}

