using Xamarin.Forms;

using Microsoft.Sonoma.Core;
using Microsoft.Sonoma.Analytics;
using Microsoft.Sonoma.Crashes;
using System.Collections.Generic;

namespace Contoso.Forms.Puppet
{
    public partial class App : Application
    {
        private const string LogTag = "SonomaXamarinPuppet";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPuppetPage());
        }

        protected override void OnStart()
        {
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

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
