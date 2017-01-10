using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Test
{
    public partial class AnalyticsPage : ContentPage
    {
        Dictionary<string, string> EventProperties = new Dictionary<string, string>();

        public AnalyticsPage()
        {
            InitializeComponent();
            ForceLayout();
        }

        void DismissPage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void GoToAnalyticsResultsPage(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AnalyticsResultsPage());
        }

        void SendEvent(object sender, EventArgs e)
        {
            string name = "UITest Event";
            if (EventProperties.Count == 0)
            {
                Analytics.TrackEvent(name);
            }
            else
            {
                Analytics.TrackEvent(name, EventProperties);
            }
        }

        void AddProperty(object sender, EventArgs e)
        {
            string propertyName = "property number " + EventProperties.Count;
            string propertyValue = "value for " + propertyName;
            EventProperties.Add(propertyName, propertyValue);
        }

        void ClearProperties(object sender, EventArgs e)
        {
            EventProperties.Clear();
        }
    }
}
