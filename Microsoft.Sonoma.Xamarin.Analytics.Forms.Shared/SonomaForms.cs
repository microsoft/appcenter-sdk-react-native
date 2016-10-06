using System;
using Xamarin.Forms;

namespace Microsoft.Sonoma.Xamarin.Analytics.Forms
{
    public static class SonomaForms
    {
        private const string CommonPageClassSuffix = "Page";

        private static Page _pageToTrack;

        private static readonly IPlatformSonomaForms PlatformSonomaForms = new PlatformSonomaForms();

        public static void StartTrackingFormPages()
        {
            Analytics.AutoPageTrackingEnabled = false;
            _pageToTrack = Application.Current.MainPage;
            PlatformSonomaForms.Initialize();
        }

        internal static void NotifyOnResume()
        {
            OnCurrentPageChanged(_pageToTrack);
        }

        private static void OnCurrentPageChanged(Page page)
        {
            var navigationPage = page as NavigationPage;
            _pageToTrack = navigationPage == null ? page : navigationPage.CurrentPage;
            var name = _pageToTrack.GetType().Name;
            if (name.EndsWith(CommonPageClassSuffix) && name.Length > CommonPageClassSuffix.Length)
                name = name.Remove(name.Length - CommonPageClassSuffix.Length);
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

        private static void OnCurrentPageChanged(object sender, EventArgs eventArgs)
        {
            OnCurrentPageChanged((Page)sender);
        }

        private static void OnCurrentPageChanged(object sender, NavigationEventArgs navigationEventArgs)
        {
            OnCurrentPageChanged((Page)sender);
        }
    }
}
