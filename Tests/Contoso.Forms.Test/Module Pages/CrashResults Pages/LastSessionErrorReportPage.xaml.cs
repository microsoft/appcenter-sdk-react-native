using Microsoft.AAppCenterCrashes;
using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class LastSessionErrorReportPage : ContentPage
    {
        readonly string _nullText;

        public LastSessionErrorReportPage()
        {
            InitializeComponent();
            _nullText = ExceptionTypeLabel.Text;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Crashes.GetLastSessionCrashReportAsync().ContinueWith(task =>
            {
                Device.BeginInvokeOnMainThread(() => UpdateLabels(task.Result));
            });
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        void UpdateLabels(ErrorReport errorReport)
        {
            ExceptionTypeLabel.Text = errorReport?.Exception?.GetType().Name ?? _nullText;
            ExceptionMessageLabel.Text = errorReport?.Exception?.Message ?? _nullText;
            AppStartTimeLabel.Text = errorReport?.AppStartTime.ToString() ?? _nullText;
            AppErrorTimeLabel.Text = errorReport?.AppErrorTime.ToString() ?? _nullText;
            IdLabel.Text = errorReport?.Id ?? _nullText;
            DeviceLabel.Text = errorReport?.Device != null ? TestStrings.DeviceReportedText : _nullText;
            iOSDetailsLabel.Text = errorReport?.iOSDetails != null ? TestStrings.HasiOSDetailsText : _nullText;
            AndroidDetailsLabel.Text = errorReport?.AndroidDetails != null ? TestStrings.HasAndroidDetailsText : _nullText;
        }
    }
}
