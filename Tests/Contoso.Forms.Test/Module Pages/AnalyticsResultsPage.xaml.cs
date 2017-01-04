using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class AnalyticsResultsPage : ContentPage
    {
        public AnalyticsResultsPage()
        {
            InitializeComponent();
            EventData.UpdatedEvent += () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (EventNameLabel != null)
                    {
                        EventNameLabel.Text = EventData.Name;
                    }

                    if (EventPropertiesLabel != null)
                    {
                        EventPropertiesLabel.Text = EventData.Properties == null ? "0" : EventData.Properties.Values.Count.ToString();
                    }
                });
            };
        }

        void DismissPage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

    }
}
