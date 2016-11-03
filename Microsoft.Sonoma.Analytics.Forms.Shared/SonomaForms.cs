using System;
using Xamarin.Forms;

namespace Microsoft.Azure.Mobile.Analytics.Forms
{
	public static class SonomaForms
	{
		const string CommonPageClassSuffix = "Page";

		static Page _pageToTrack;

		static readonly IPlatformSonomaForms PlatformSonomaForms = new PlatformSonomaForms();

		public static void StartTrackingFormPages()
		{
			//Analytics.AutoPageTrackingEnabled = false;
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
			if (name.EndsWith(CommonPageClassSuffix, StringComparison.OrdinalIgnoreCase) && name.Length > CommonPageClassSuffix.Length)
				name = name.Remove(name.Length - CommonPageClassSuffix.Length);
			//Analytics.TrackPage(name);
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
