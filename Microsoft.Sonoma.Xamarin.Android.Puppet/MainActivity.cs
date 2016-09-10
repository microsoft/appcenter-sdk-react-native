using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;

namespace Microsoft.Sonoma.Xamarin.Android.Puppet
{
    using Com.Microsoft.Sonoma.Core;
    using Com.Microsoft.Sonoma.Analytics;
    using Com.Microsoft.Sonoma.Errors;

    [Activity(Label = "Puppet", MainLauncher = true, Icon = "@mipmap/icon")]
    // ReSharper disable once UnusedMember.Global
    public class MainActivity : AppCompatActivity
    {
        private int _count;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            {
                button.Text = $"{_count / _count}";
                _count++;
            };

            Sonoma.LogLevel = 2;
            Sonoma.Start(Application, "9f6eb5e9-e0e6-4a24-b030-c6c1631b54fe", Class.FromType(typeof(Analytics)), Class.FromType(typeof(ErrorReporting)));
            Analytics.TrackEvent("myEvent", new Dictionary<string, string> { { "someKey", "someValue" } });
        }
    }
}