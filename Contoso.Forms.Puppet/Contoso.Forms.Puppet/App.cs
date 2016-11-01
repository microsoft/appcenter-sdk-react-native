using Microsoft.Sonoma.Analytics;
using Microsoft.Sonoma.Analytics.Forms;
using Microsoft.Sonoma.Core;
using Microsoft.Sonoma.Crashes;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public class App : Application
    {
        private const string LogTag = "SonomaXamarinPuppet";

        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //Sonoma.SetServerUrl("http://in-integration.dev.avalanch.es:8081");
            SonomaLog.Assert(LogTag, "Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.LogLevel = LogLevel.Verbose;
            SonomaLog.Info(LogTag, "Sonoma.LogLevel=" + Sonoma.LogLevel);
            //SonomaForms.StartTrackingFormPages();
            Sonoma.Start(typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("myEvent");
            Analytics.TrackEvent("myEvent2", new Dictionary<string, string> { { "someKey", "someValue" } });
            SonomaLog.Info(LogTag, "Sonoma.InstallId=" + Sonoma.InstallId);
            SonomaLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + Crashes.HasCrashedInLastSession);
        }
    }
}