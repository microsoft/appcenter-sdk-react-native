using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Test
{
    public partial class TestPage : ContentPage
    {
        public TestPage()
        {
            InitializeComponent();
            UpdateEnabledStateLabels();
        }

        void ToggleMobileCenterEnabled(object sender, System.EventArgs e)
        {
            MobileCenter.Enabled = !MobileCenter.Enabled;
            UpdateEnabledStateLabels();
        }

        void ToggleCrashesEnabled(object sender, System.EventArgs e)
        {
            Crashes.Enabled = !Crashes.Enabled;
            UpdateEnabledStateLabels();
        }

        void ToggleAnalyticsEnabled(object sender, System.EventArgs e)
        {
            Analytics.Enabled = !Analytics.Enabled;
            UpdateEnabledStateLabels();
        }

        Dictionary<string, string> EventProperties = new Dictionary<string, string>();

        void SendEvent(object sender, System.EventArgs e)
        {
            string name = "UITest Event";
            if (EventProperties.Count == 0)
            {
                Analytics.TrackEvent(name);
            }
            else
            {
                Analytics.TrackEvent(name, EventProperties);
            }
        }

        void AddProperty(object sender, System.EventArgs e)
        {
            string propertyName = "property number " + EventProperties.Count;
            string propertyValue = "value for " + propertyName;
            EventProperties.Add(propertyName, propertyValue);
        }

        void ClearProperties(object sender, System.EventArgs e)
        {
            EventProperties.Clear();
        }

        void DivideByZeroCrash(object sender, System.EventArgs e)
        {
            int x = 42 / int.Parse("0");
        }

        void GenerateTestCrash(object sender, System.EventArgs e)
        {
            Crashes.GenerateTestCrash();
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
                CrashesEnabledLabel.Text = Crashes.Enabled ? "Crashes enabled" : "Crashes disabled";
            }
        }

        void UpdateAnalyticsLabel()
        {
            if (AnalyticsEnabledLabel != null)
            {
                AnalyticsEnabledLabel.Text = Analytics.Enabled ? "Analytics enabled" : "Analytics disabled";
            }
        }

        void UpdateMobileCenterLabel()
        {
            if (MobileCenterEnabledLabel != null)
            {
                MobileCenterEnabledLabel.Text = MobileCenter.Enabled ? "Mobile Center enabled" : "Mobile Center disabled";
            }
        }
    }
}
