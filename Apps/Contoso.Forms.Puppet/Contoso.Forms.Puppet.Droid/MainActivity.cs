using Android.App;
using Android.Content.PM;
using Android.OS;
using Com.Microsoft.AppCenter.Analytics;
using Com.Microsoft.AppCenter.Analytics.Channel;
using Com.Microsoft.AppCenter.Ingestion.Models;
using HockeyApp.Android;
using HockeyApp.Android.Utils;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "ACFPuppet", Icon = "@drawable/icon", Theme = "@style/PuppetTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleTop)]
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

        protected override void OnResume()
        {
            base.OnResume();
            HockeyLog.LogLevel = 2;
            CrashManager.Register(this, "2c7e569100194bafa2a30f5c648d44fe");
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
