using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class ToggleStatesPage : ContentPage
    {
        public ToggleStatesPage()
        {
            InitializeComponent();
            UpdateEnabledStateLabels();
        }

        void EnableAppCenter(object sender, System.EventArgs e)
        {
            AppCenter.SetEnabledAsync(true).Wait();
            UpdateEnabledStateLabels();
        }

        void EnableCrashes(object sender, System.EventArgs e)
        {
            Crashes.SetEnabledAsync(true).Wait();
            UpdateEnabledStateLabels();
        }

        void EnableAnalytics(object sender, System.EventArgs e)
        {
            Analytics.SetEnabledAsync(true).Wait();
            UpdateEnabledStateLabels();
        }

        void DisableAppCenter(object sender, System.EventArgs e)
        {
            AppCenter.SetEnabledAsync(false).Wait();
            UpdateEnabledStateLabels();
        }

        void DisableCrashes(object sender, System.EventArgs e)
        {
            Crashes.SetEnabledAsync(false).Wait();
            UpdateEnabledStateLabels();
        }

        void DisableAnalytics(object sender, System.EventArgs e)
        {
            Analytics.SetEnabledAsync(false).Wait();
            UpdateEnabledStateLabels();
        }

        void UpdateEnabledStateLabels()
        {
            ForceLayout();
            UpdateAppCenterLabel();
            UpdateCrashesLabel();
            UpdateAnalyticsLabel();
            ForceLayout();
        }

        void UpdateCrashesLabel()
        {
            if (CrashesEnabledLabel != null)
            {
                CrashesEnabledLabel.Text = Crashes.IsEnabledAsync().Result ? TestStrings.CrashesEnabledText : TestStrings.CrashesDisabledText;
            }
        }

        void UpdateAnalyticsLabel()
        {
            if (AnalyticsEnabledLabel != null)
            {
                AnalyticsEnabledLabel.Text = Analytics.IsEnabledAsync().Result ? TestStrings.AnalyticsEnabledText : TestStrings.AnalyticsDisabledText;
            }
        }

        void UpdateAppCenterLabel()
        {
            if (AppCenterEnabledLabel != null)
            {
                AppCenterEnabledLabel.Text = AppCenter.IsEnabledAsync().Result ? TestStrings.AppCenterEnabledText : TestStrings.AppCenterDisabledText;
            }
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
