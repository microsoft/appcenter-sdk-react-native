using Android.App;
using Android.OS;
using Android.Widget;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using System.Collections.Generic;

namespace Contoso.Android.Puppet
{
    [Activity(Label = "SXPuppet", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string LogTag = "SonomaXamarinPuppet";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += delegate
            {
                // Crash
                button.Text = button.Text.Substring(42);
            };

            // Sonoma integration
            SonomaLog.Assert(LogTag, "Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.LogLevel = LogLevel.Verbose;
            SonomaLog.Info(LogTag, "Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.Start("44cd8722-bfe0-4748-ac14-7692e031a8a5", typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("myEvent", new Dictionary<string, string> { { "someKey", "someValue" } });
            SonomaLog.Info(LogTag, "Sonoma.InstallId=" + Sonoma.InstallId);
        }
    }
}