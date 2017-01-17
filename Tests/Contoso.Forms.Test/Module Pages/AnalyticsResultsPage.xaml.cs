using System;
using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class AnalyticsResultsPage : ContentPage
    {
        public AnalyticsResultsPage()
        {
            InitializeComponent();
            ForceLayout();
            EventSharer.SendingEvent += (EventData data) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ForceLayout();

                    if (EventNameLabel != null)
                    {
                        EventNameLabel.Text = data.Name;
                    }

                    if (EventPropertiesLabel != null)
                    {
                        EventPropertiesLabel.Text = data.Properties == null ? "0" : data.Properties.Values.Count.ToString();
                    }

                    if (DidSendingEventLabel != null)
                    {
                        DidSendingEventLabel.Text = TestStrings.DidSendingEventText;
                    }
                });
            };

            EventSharer.SentEvent += (EventData data) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ForceLayout();

                    if (DidSentEventLabel != null)
                    {
                        DidSentEventLabel.Text = TestStrings.DidSentEventText;
                    }
                });
            };

            EventSharer.FailedToSendEvent += (EventData data) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ForceLayout();

                    if (DidFailedToSendEventLabel != null)
                    {
                        DidFailedToSendEventLabel.Text = TestStrings.DidFailedToSendEventText;
                    }
                });
            };
        }

        void DismissPage(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void ResetPage(object sender, EventArgs e)
        {
            EventNameLabel.Text = "";
            EventPropertiesLabel.Text = "";
            DidSentEventLabel.Text = "";
            DidSendingEventLabel.Text = "";
            DidFailedToSendEventLabel.Text = "";
        } 
    }
}
