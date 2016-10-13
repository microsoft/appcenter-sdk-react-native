using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Sonoma.Xamarin.Analytics;
using Microsoft.Sonoma.Xamarin.Analytics.Forms;
using Microsoft.Sonoma.Xamarin.Core;
using Microsoft.Sonoma.Xamarin.Crashes;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //Sonoma.SetServerUrl("http://in-integration.dev.avalanch.es:8081");
            Debug.WriteLine("Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.LogLevel = LogLevel.Verbose;
            Debug.WriteLine("Sonoma.LogLevel=" + Sonoma.LogLevel);           
            SonomaForms.StartTrackingFormPages();
            Sonoma.Start(typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("myEvent");
            Analytics.TrackEvent("myEvent2", new Dictionary<string, string> { { "someKey", "someValue" } });
            Debug.WriteLine("Sonoma.InstallId=" + Sonoma.InstallId);
            Debug.WriteLine("Crashes.HasCrashedInLastSession=" + Crashes.HasCrashedInLastSession);
        }
    }
}