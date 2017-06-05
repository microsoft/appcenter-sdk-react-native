using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;

namespace Contoso.WinForms.Demo
{
    public partial class MainForm : Form
    {
        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions = new Dictionary<LogLevel, Action<string, string>> {
            { LogLevel.Verbose, MobileCenterLog.Verbose },
            { LogLevel.Debug, MobileCenterLog.Debug },
            { LogLevel.Info, MobileCenterLog.Info },
            { LogLevel.Warn, MobileCenterLog.Warn },
            { LogLevel.Error, MobileCenterLog.Error }
        };

        public MainForm()
        {
            InitializeComponent();
            UpdateState();
            mobileCenterLogLevel.SelectedIndex = (int) MobileCenter.LogLevel;
        }

        private void UpdateState()
        {
            mobileCenterEnabled.Checked = MobileCenter.Enabled;
            analyticsEnabled.Checked = Analytics.Enabled;
        }

        private void mobileCenterEnabled_CheckedChanged(object sender, EventArgs e)
        {
            MobileCenter.Enabled = mobileCenterEnabled.Checked;
        }

        private void analyticsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Analytics.Enabled = analyticsEnabled.Checked;
        }

        private void mobileCenterLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            MobileCenter.LogLevel = (LogLevel)mobileCenterLogLevel.SelectedIndex;
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void writeLog_Click(object sender, EventArgs e)
        {
            if (logLevel.SelectedIndex == -1)
            {
                return;
            }
            var level = (LogLevel)logLevel.SelectedIndex;
            var tag = logTag.Text;
            var message = logMessage.Text;
            LogFunctions[level](tag, message);
        }

        private void trackEvent_Click(object sender, EventArgs e)
        {
            var name = eventName.Text;
            var properties = eventProperties.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Cells["Key"].Value != null && row.Cells["Value"].Value != null)
                .ToDictionary(
                    row => row.Cells["Key"].Value?.ToString(),
                    row => row.Cells["Value"].Value?.ToString());
            Analytics.TrackEvent(name, properties);
        }

        private void CrashWithNullReferenceException_Click(object sender, EventArgs e)
        {
            string[] values = { "a", null, "c" };
            var b = values[1].Trim();
            System.Diagnostics.Debug.WriteLine(b);
        }
    }
}
