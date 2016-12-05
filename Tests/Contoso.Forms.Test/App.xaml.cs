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
            TestPage theMainPage = new TestPage();

            Crashes.SentErrorReport += theMainPage.SentErrorReport;
            Crashes.SendingErrorReport += theMainPage.SendingErrorReport;
            Crashes.FailedToSendErrorReport += theMainPage.FailedToSendErrorReport;
            Crashes.GetErrorAttachment = theMainPage.GetErrorAttachment;
            Crashes.ShouldAwaitUserConfirmation = theMainPage.ShouldAwaitUserConfirmation;
            Crashes.ShouldProcessErrorReport = theMainPage.ShouldProcessErrorReport;
            
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenter.Start(typeof(Analytics), typeof(Crashes));

            MainPage = theMainPage;
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
