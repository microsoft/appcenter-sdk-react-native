using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile;
using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class CrashResultsPage : ContentPage
    {
        public CrashResultsPage()
        {
            InitializeComponent();

            if (HasCrashedInLastSessionLabel != null && Crashes.HasCrashedInLastSession)
            {
                HasCrashedInLastSessionLabel.Text = "HasCrashedInLastSession == true";
            }
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        public void SendingErrorReport(object sender, SendingErrorReportEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (SendingErrorReportLabel != null)
                {
                    SendingErrorReportLabel.Text = "SendingErrorReport has occured";
                }
            });
        }

        public void SentErrorReport(object sender, SentErrorReportEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (SentErrorReportLabel != null)
                {
                    SentErrorReportLabel.Text = "SentErrorReport has occured";
                }
            });
        }

        public void FailedToSendErrorReport(object sender, FailedToSendErrorReportEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (FailedToSendErrorReportLabel != null)
                {
                    FailedToSendErrorReportLabel.Text = "FailedToSendErrorReport has occured";
                }
            });
        }

        public ErrorAttachment GetErrorAttachment(ErrorReport report)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (GetErrorAttachmentLabel != null)
                {
                    GetErrorAttachmentLabel.Text = "GetErrorAttachment has occured";
                }
            });
            return ErrorAttachment.AttachmentWithText("hello");
        }

        public bool ShouldProcessErrorReport(ErrorReport report)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (ShouldProcessErrorReportLabel != null)
                {
                    ShouldProcessErrorReportLabel.Text = "ShouldProcessErrorReport has occured";
                }
            });
            return true;
        }

        public bool ShouldAwaitUserConfirmation()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (ShouldAwaitUserConfirmationLabel != null)
                {
                    ShouldAwaitUserConfirmationLabel.Text = "ShouldAwaitUserConfirmation has occured";
                }
            });
            return false;
        }
                                                         
    }
}
