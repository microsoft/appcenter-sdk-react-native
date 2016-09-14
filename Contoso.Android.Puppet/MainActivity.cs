using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using Microsoft.Sonoma.Xamarin.Analytics;
using Microsoft.Sonoma.Xamarin.Core;
using Microsoft.Sonoma.Xamarin.Crashes;

namespace Contoso.Android.Puppet
{
    [Activity(Label = "SXPuppet", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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
            Log.Info("SonomaXamarin", "Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.LogLevel = LogLevel.Verbose;
            Log.Info("SonomaXamarin", "Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.Start("6ad16901-9d7d-4135-a3d5-085813b01a4b", typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("myEvent", new Dictionary<string, string> { { "someKey", "someValue" } });
            Log.Info("SonomaXamarin", "Sonoma.InstallId=" + Sonoma.InstallId);
        }
    }
}