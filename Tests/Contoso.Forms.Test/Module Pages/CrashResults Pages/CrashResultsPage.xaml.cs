using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class CrashResultsPage : ContentPage
    {
        public CrashResultsPage()
        {
            InitializeComponent();
            InitializeText();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (HasCrashedInLastSessionLabel != null)
            {
                if (Crashes.HasCrashedInLastSession)
                {
                    HasCrashedInLastSessionLabel.Text = TestStrings.HasCrashedInLastSessionText;
                }
                else
                {
                    HasCrashedInLastSessionLabel.Text = TestStrings.HasNotCrashedInLastSessionText;
                }
            }
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void ViewLastSessionErrorReport(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new LastSessionErrorReportPage());
        }

        public void SendingErrorReport(object sender, SendingErrorReportEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (SendingErrorReportLabel != null)
                {
                    SendingErrorReportLabel.Text = TestStrings.DidSendingErrorReportText;
                }
            });

        }

        public void SentErrorReport(object sender, SentErrorReportEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (SentErrorReportLabel != null)
                {
                    SentErrorReportLabel.Text = TestStrings.DidSentErrorReportText;
                }
            });
        }

        public void FailedToSendErrorReport(object sender, FailedToSendErrorReportEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (FailedToSendErrorReportLabel != null)
                {
                    FailedToSendErrorReportLabel.Text = TestStrings.DidFailedToSendErrorReportText;
                }
            });
        }

        public ErrorAttachment GetErrorAttachment(ErrorReport report)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (GetErrorAttachmentLabel != null)
                {
                    GetErrorAttachmentLabel.Text = TestStrings.DidGetErrorAttachmentText;
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
                    ShouldProcessErrorReportLabel.Text = TestStrings.DidShouldProcessErrorReportText;
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
                    ShouldAwaitUserConfirmationLabel.Text = TestStrings.DidShouldAwaitUserConfirmationText;
                }
            });
            return false;
        }

        public void InitializeText()
        {
            if (HasCrashedInLastSessionLabel != null)
            {
                if (Crashes.HasCrashedInLastSession)
                {
                    HasCrashedInLastSessionLabel.Text = TestStrings.HasCrashedInLastSessionText;
                }
                else
                {
                    HasCrashedInLastSessionLabel.Text = TestStrings.HasNotCrashedInLastSessionText;
                }
            }

            if (SendingErrorReportLabel != null)
            {
                SendingErrorReportLabel.Text = TestStrings.DidNotSendingErrorReportText;
            }

            if (SentErrorReportLabel != null)
            {
                SentErrorReportLabel.Text = TestStrings.DidNotSentErrorReportText;
            }

            if (FailedToSendErrorReportLabel != null)
            {
                FailedToSendErrorReportLabel.Text = TestStrings.DidNotFailedToSendErrorReportText;
            }

            if (GetErrorAttachmentLabel != null)
            {
                GetErrorAttachmentLabel.Text = TestStrings.DidNotGetErrorAttachmentText;
            }

            if (ShouldProcessErrorReportLabel != null)
            {
                ShouldProcessErrorReportLabel.Text = TestStrings.DidNotShouldProcessErrorReportText;
            }

            if (ShouldAwaitUserConfirmationLabel != null)
            {
                ShouldAwaitUserConfirmationLabel.Text = TestStrings.DidNotShouldAwaitUserConfirmationText;
            }
        }                                         
    }
}
