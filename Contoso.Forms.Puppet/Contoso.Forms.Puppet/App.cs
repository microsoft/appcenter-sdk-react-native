using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Sonoma.Xamarin.Analytics;
using Microsoft.Sonoma.Xamarin.Core;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        }
                    }
                }
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            Debug.WriteLine("Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.LogLevel = LogLevel.Verbose;
            Debug.WriteLine("Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.Start("61e19ad3-5b43-4fc5-84aa-0bed649d70db", typeof(Analytics));
            Analytics.TrackEvent("myEvent", new Dictionary<string, string> { { "someKey", "someValue" } });
            Debug.WriteLine("Sonoma.InstallId=" + Sonoma.InstallId);
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