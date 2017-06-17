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
            MobileCenter.SetEnabled(true);
            UpdateEnabledStateLabels();
        }

        void EnableCrashes(object sender, System.EventArgs e)
        {
            Crashes.SetEnabled(true);
            UpdateEnabledStateLabels();
        }

        void EnableAnalytics(object sender, System.EventArgs e)
        {
            Analytics.SetEnabled(true);
            UpdateEnabledStateLabels();
        }

        void DisableMobileCenter(object sender, System.EventArgs e)
        {
            MobileCenter.SetEnabled(false);
            UpdateEnabledStateLabels();
        }

        void DisableCrashes(object sender, System.EventArgs e)
        {
            Crashes.SetEnabled(false);
            UpdateEnabledStateLabels();
        }

        void DisableAnalytics(object sender, System.EventArgs e)
        {
            Analytics.SetEnabled(false);
            UpdateEnabledStateLabels();
        }

        void UpdateEnabledStateLabels()
        {
            ForceLayout();
            UpdateMobileCenterLabel();
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

        void UpdateMobileCenterLabel()
        {
            if (MobileCenterEnabledLabel != null)
            {
                MobileCenterEnabledLabel.Text = MobileCenter.IsEnabledAsync().Result ? TestStrings.MobileCenterEnabledText : TestStrings.MobileCenterDisabledText;
            }
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
