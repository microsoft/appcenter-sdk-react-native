using Microsoft.AppCenterCrashes;
using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class CrashResultsPage : ContentPage
    {
        LastSessionErrorReportPage ErrorReportPage;

        public CrashResultsPage()
        {
            InitializeComponent();
            ErrorReportPage = new LastSessionErrorReportPage();
            ForceLayout();
            InitializeText();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (HasCrashedInLastSessionLabel != null)
            {
                if (Crashes.HasCrashedInLastSessionAsync().Result)
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
            Navigation.PushModalAsync(ErrorReportPage);
        }

        public void SendingErrorReport(object sender, SendingErrorReportEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ForceLayout();

                if (SendingErrorReportLabel != null)
                {
                    SendingErrorReportLabel.Text = TestStrings.DidSendingErrorReportText;
                }
            });

        }

        public void SentErrorReport(object sender, SentErrorReportEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ForceLayout();

                if (SentErrorReportLabel != null)
                {
                    SentErrorReportLabel.Text = TestStrings.DidSentErrorReportText;
                }
            });
        }

        public void FailedToSendErrorReport(object sender, FailedToSendErrorReportEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ForceLayout();

                if (FailedToSendErrorReportLabel != null)
                {
                    FailedToSendErrorReportLabel.Text = TestStrings.DidFailedToSendErrorReportText;
                }
            });
        }

        public bool ShouldProcessErrorReport(ErrorReport report)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                ForceLayout();

                if (ShouldProcessErrorReportLabel != null)
                {
                    ShouldProcessErrorReportLabel.Text = TestStrings.DidShouldProcessErrorReportText;
                }
            });

            return true;
        }

        public bool ShouldAwaitUserConfirmation()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ForceLayout();

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
                if (Crashes.HasCrashedInLastSessionAsync().Result)
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
