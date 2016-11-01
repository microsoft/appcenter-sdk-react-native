using Xamarin.Forms;

using Microsoft.Sonoma.Core;
using Microsoft.Sonoma.Analytics;
using Microsoft.Sonoma.Crashes;


namespace Contoso.Forms.Puppet
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPuppetPage());
            Sonoma.LogLevel = LogLevel.Verbose;
            Sonoma.Start(typeof(Analytics), typeof(Crashes));
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
