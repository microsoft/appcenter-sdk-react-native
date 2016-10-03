using System;
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
        private Page _pageToTrack;

        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //Sonoma.SetServerUrl("http://in-integration.dev.avalanch.es:8081");
            Debug.WriteLine("Sonoma.LogLevel=" + Sonoma.LogLevel);
            Sonoma.LogLevel = LogLevel.Verbose;
            Debug.WriteLine("Sonoma.LogLevel=" + Sonoma.LogLevel);
            Analytics.AutoPageTrackingEnabled = false;
            Sonoma.Start(typeof(Analytics), typeof(Crashes));
            //Sonoma.Enabled = false;
            //Sonoma.Enabled = true;
            Analytics.TrackEvent("myEvent");
            Analytics.TrackEvent("myEventWithProperties", new Dictionary<string, string> { { "someKey", "someValue" } });
            Debug.WriteLine("Sonoma.InstallId=" + Sonoma.InstallId);
            OnCurrentPageChanged(MainPage);
        }

        private void OnCurrentPageChanged(Page page)
        {
            var navigationPage = page as NavigationPage;
            _pageToTrack = navigationPage == null ? page : navigationPage.CurrentPage;
            var name = _pageToTrack.GetType().Name;
            if (name.EndsWith("Page") && name.Length > "Page".Length)
                name = name.Remove(name.Length - "Page".Length);
            Analytics.TrackPage(name);
            if (navigationPage != null)
            {
                navigationPage.Pushed -= OnCurrentPageChanged;
                navigationPage.Pushed += OnCurrentPageChanged;
                navigationPage.Popped -= OnCurrentPageChanged;
                navigationPage.Popped += OnCurrentPageChanged;
            }
            else
            {
                page.Appearing -= OnCurrentPageChanged;
                page.Appearing += OnCurrentPageChanged;
            }
        }

        private void OnCurrentPageChanged(object sender, EventArgs eventArgs)
        {
            OnCurrentPageChanged((Page)sender);
        }

        private void OnCurrentPageChanged(object sender, NavigationEventArgs navigationEventArgs)
        {
            OnCurrentPageChanged((Page)sender);
        }

        protected override void OnSleep()
        {
            Debug.WriteLine("OnSleep()");
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Debug.WriteLine("OnResume()");
            OnCurrentPageChanged(_pageToTrack);
        }
    }
}