using Android.App;
using Android.Content.PM;
using Android.OS;
using Com.Microsoft.Azure.Mobile.Analytics;
using Com.Microsoft.Azure.Mobile.Analytics.Channel;
using Com.Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Push;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "MCFPuppet", Icon = "@drawable/icon", Theme = "@style/PuppetTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            AndroidAnalytics.SetListener(new AndroidAnalyticsListener());

            LoadApplication(new App());
        }

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);
            Push.CheckLaunchedFromNotification(this, intent);
        }
    }

    public class AndroidAnalyticsListener : Java.Lang.Object, IAnalyticsListener
    {
        public void OnSendingFailed(ILog log, Java.Lang.Exception e)
        {
            MobileCenterLog.Debug(App.LogTag, "Analytics listener OnSendingFailed with exception: " + e);
        }

        public void OnSendingSucceeded(ILog log)
        {
            MobileCenterLog.Debug(App.LogTag, "Analytics listener OnSendingSucceeded");
        }

        public void OnBeforeSending(ILog log)
        {
            MobileCenterLog.Debug(App.LogTag, "Analytics listener OnBeforeSendingEventLog");
        }
    }

}
