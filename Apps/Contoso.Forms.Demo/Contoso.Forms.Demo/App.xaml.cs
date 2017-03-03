using Xamarin.Forms;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;


namespace Contoso.Forms.Demo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainDemoPage());
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenter.Start("android=987b5941-4fac-4968-933e-98a7ff29237c;ios=fe2bf05d-f4f9-48a6-83d9-ea8033fbb644",
                               typeof(Analytics), typeof(Crashes));
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
