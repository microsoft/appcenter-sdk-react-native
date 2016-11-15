using Xamarin.Forms;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System.Collections.Generic;

namespace Contoso.Forms.Puppet
{
    public partial class App : Application
    {
        private const string LogTag = "MobileCenterXamarinPuppet";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPuppetPage());
        }

        protected override void OnStart()
        {
            MobileCenterLog.Assert(LogTag, "MobileCenter.LogLevel=" + MobileCenter.LogLevel);
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenterLog.Info(LogTag, "MobileCenter.LogLevel=" + MobileCenter.LogLevel);

            Crashes.SendingErrorReport += (sender, e) =>
            {
                MobileCenterLog.Info(LogTag, "*************************\n");

                MobileCenterLog.Info(LogTag, "Sending error report\n");
                SendingErrorReportEventArgs args = e as SendingErrorReportEventArgs;
                ErrorReport report = args.Report;

                if (report.SystemException != null)
                {
                    MobileCenterLog.Info(LogTag, report.SystemException.ToString());
                }
                else if (report.AndroidDetails != null)
                {
                    MobileCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
                }
            };
            MobileCenter.SetServerUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Start(typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("myEvent");
            Analytics.TrackEvent("myEvent2", new Dictionary<string, string> { { "someKey", "someValue" } });
            MobileCenterLog.Info(LogTag, "MobileCenter.InstallId=" + MobileCenter.InstallId);
            MobileCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + Crashes.HasCrashedInLastSession);

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
