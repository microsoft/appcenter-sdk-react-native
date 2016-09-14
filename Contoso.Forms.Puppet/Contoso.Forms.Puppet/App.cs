using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Sonoma.Xamarin.Analytics;
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
            Sonoma.Start("6ad16901-9d7d-4135-a3d5-085813b01a4b", typeof(Analytics), typeof(Crashes));
            Analytics.TrackEvent("myEvent", new Dictionary<string, string> { { "someKey", "someValue" } });
            Debug.WriteLine("Sonoma.InstallId=" + Sonoma.InstallId);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            // Make it crash
            // ReSharper disable once ConvertToConstant.Local
            var count = 0;
            Debug.WriteLine("count=" + (count / count));
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}