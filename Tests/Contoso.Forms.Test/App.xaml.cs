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

            MobileCenter.Start(typeof(Analytics), typeof(Crashes));

            MainPage = new TestPage();
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
