using Microsoft.Azure.Mobile.Crashes;
using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class LastSessionErrorReportPage : ContentPage
    {
        public LastSessionErrorReportPage()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(() =>
            {
                UpdateLabels();
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateLabels();
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void UpdateLabels()
        {
            if (ExceptionTypeLabel != null)
            {
                ExceptionTypeLabel.Text = Crashes.LastSessionCrashReport?.Exception?.GetType().Name;
            }

            if (ExceptionMessageLabel != null)
            {
                ExceptionMessageLabel.Text = Crashes.LastSessionCrashReport?.Exception?.Message;
            }

            if (AppStartTimeLabel != null && Crashes.LastSessionCrashReport?.AppStartTime != null)
            {
                AppStartTimeLabel.Text = Crashes.LastSessionCrashReport.AppStartTime.ToString();
            }

            if (AppErrorTimeLabel != null && Crashes.LastSessionCrashReport?.AppErrorTime != null)
            {
                AppErrorTimeLabel.Text = Crashes.LastSessionCrashReport.AppErrorTime.ToString();
            }

            if (IdLabel != null)
            {
                IdLabel.Text = Crashes.LastSessionCrashReport?.Id;
            }

            if (DeviceLabel != null && Crashes.LastSessionCrashReport?.Device != null)
            {
                DeviceLabel.Text = TestStrings.DeviceReportedText;
            }

            if (iOSDetailsLabel != null && Crashes.LastSessionCrashReport?.iOSDetails != null)
            {
                iOSDetailsLabel.Text = TestStrings.HasiOSDetailsText;
            }

            if (AndroidDetailsLabel != null && Crashes.LastSessionCrashReport?.AndroidDetails != null)
            {
                AndroidDetailsLabel.Text = TestStrings.HasAndroidDetailsText;
            }
        }
    }
}
