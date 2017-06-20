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
            MobileCenter.SetEnabledAsync(true).Wait();
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

        void DisableMobileCenter(object sender, System.EventArgs e)
        {
            MobileCenter.SetEnabledAsync(false).Wait();
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
