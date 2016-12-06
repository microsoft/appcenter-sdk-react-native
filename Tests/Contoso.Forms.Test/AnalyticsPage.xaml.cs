using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Test
{
    public partial class AnalyticsPage : ContentPage
    {
        Dictionary<string, string> EventProperties = new Dictionary<string, string>();

        public AnalyticsPage()
        {
            InitializeComponent();
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }


        void SendEvent(object sender, System.EventArgs e)
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

        void AddProperty(object sender, System.EventArgs e)
        {
            string propertyName = "property number " + EventProperties.Count;
            string propertyValue = "value for " + propertyName;
            EventProperties.Add(propertyName, propertyValue);
        }

        void ClearProperties(object sender, System.EventArgs e)
        {
            EventProperties.Clear();
        }
    }
}
