using Xamarin.Forms;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace Contoso.Forms.Test
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            TestPage testPage = new TestPage();

            /* Set crash callbacks and events */
            Crashes.ShouldProcessErrorReport = testPage.ResultsPage.ShouldProcessErrorReport;
            Crashes.ShouldAwaitUserConfirmation = testPage.ResultsPage.ShouldAwaitUserConfirmation;
            Crashes.SendingErrorReport += testPage.ResultsPage.SendingErrorReport;
            Crashes.SentErrorReport += testPage.ResultsPage.SentErrorReport;
            Crashes.FailedToSendErrorReport += testPage.ResultsPage.FailedToSendErrorReport;

            /* Start Mobile Center */
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenter.Start(typeof(Analytics), typeof(Crashes));
            MainPage = testPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
