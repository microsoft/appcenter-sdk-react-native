using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile;

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

        void EnableMobileCenter(object sender, System.EventArgs e)
        {
            MobileCenter.Enabled = true;
            UpdateEnabledStateLabels();
        }

        void EnableCrashes(object sender, System.EventArgs e)
        {
            Crashes.Enabled = true;
            UpdateEnabledStateLabels();
        }

        void EnableAnalytics(object sender, System.EventArgs e)
        {
            Analytics.Enabled = true;
            UpdateEnabledStateLabels();
        }

        void DisableMobileCenter(object sender, System.EventArgs e)
        {
            MobileCenter.Enabled = false;
            UpdateEnabledStateLabels();
        }

        void DisableCrashes(object sender, System.EventArgs e)
        {
            Crashes.Enabled = false;
            UpdateEnabledStateLabels();
        }

        void DisableAnalytics(object sender, System.EventArgs e)
        {
            Analytics.Enabled = false;
            UpdateEnabledStateLabels();
        }

        void UpdateEnabledStateLabels()
        {
            UpdateMobileCenterLabel();
            UpdateCrashesLabel();
            UpdateAnalyticsLabel();
        }

        void UpdateCrashesLabel()
        {
            if (CrashesEnabledLabel != null)
            {
                CrashesEnabledLabel.Text = Crashes.Enabled ? TestStrings.CrashesEnabledText : TestStrings.CrashesDisabledText;
            }
        }

        void UpdateAnalyticsLabel()
        {
            if (AnalyticsEnabledLabel != null)
            {
                AnalyticsEnabledLabel.Text = Analytics.Enabled ? TestStrings.AnalyticsEnabledText : TestStrings.AnalyticsDisabledText;
            }
        }

        void UpdateMobileCenterLabel()
        {
            if (MobileCenterEnabledLabel != null)
            {
                MobileCenterEnabledLabel.Text = MobileCenter.Enabled ? TestStrings.MobileCenterEnabledText : TestStrings.MobileCenterDisabledText;
            }
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
