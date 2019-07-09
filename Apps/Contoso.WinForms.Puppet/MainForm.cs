// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Contoso.WinForms.Demo
{
    public partial class MainForm : Form
    {
        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions = new Dictionary<LogLevel, Action<string, string>> {
            { LogLevel.Verbose, AppCenterLog.Verbose },
            { LogLevel.Debug, AppCenterLog.Debug },
            { LogLevel.Info, AppCenterLog.Info },
            { LogLevel.Warn, AppCenterLog.Warn },
            { LogLevel.Error, AppCenterLog.Error }
        };

        public MainForm()
        {
            InitializeComponent();
            UpdateState();
            AppCenterLogLevel.SelectedIndex = (int)AppCenter.LogLevel;
        }

        private void UpdateState()
        {
            AppCenterEnabled.Checked = AppCenter.IsEnabledAsync().Result;
            AnalyticsEnabled.Checked = Analytics.IsEnabledAsync().Result;
            CrashesEnabled.Checked = Crashes.IsEnabledAsync().Result;
        }

        private void AppCenterEnabled_CheckedChanged(object sender, EventArgs e)
        {
            AppCenter.SetEnabledAsync(AppCenterEnabled.Checked).Wait();
        }

        private void AnalyticsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Analytics.SetEnabledAsync(AnalyticsEnabled.Checked).Wait();
        }

        private void AppCenterLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppCenter.LogLevel = (LogLevel)AppCenterLogLevel.SelectedIndex;
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void WriteLog_Click(object sender, EventArgs e)
        {
            if (LogLevelValue.SelectedIndex == -1)
            {
                return;
            }
            var level = (LogLevel)LogLevelValue.SelectedIndex;
            var tag = LogTag.Text;
            var message = LogMessage.Text;
            LogFunctions[level](tag, message);
        }

        private void TrackEvent_Click(object sender, EventArgs e)
        {
            var name = EventName.Text;
            var properties = EventProperties.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Cells["Key"].Value != null && row.Cells["Value"].Value != null)
                .ToDictionary(
                    row => row.Cells["Key"].Value?.ToString(),
                    row => row.Cells["Value"].Value?.ToString());
            Analytics.TrackEvent(name, properties);
        }

        #region Crash

        public class NonSerializableException : Exception
        {
        }

        private async void CrashesEnabled_CheckedChanged(object sender, EventArgs e)
        {
            await Crashes.SetEnabledAsync(CrashesEnabled.Checked);
        }

        private void CrashWithTestException_Click(object sender, EventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        private void CrashWithNonSerializableException_Click(object sender, EventArgs e)
        {
            throw new NonSerializableException();
        }

        private void CrashWithDivisionByZero_Click(object sender, EventArgs e)
        {
            _ = 42 / int.Parse("0");
        }

        private void CrashWithAggregateException_Click(object sender, EventArgs e)
        {
            throw GenerateAggregateException();
        }

        private static Exception GenerateAggregateException()
        {
            try
            {
                throw new AggregateException(SendHttp(), new ArgumentException("Invalid parameter", ValidateLength()));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception SendHttp()
        {
            try
            {
                throw new IOException("Network down");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception ValidateLength()
        {
            try
            {
                throw new ArgumentOutOfRangeException(null, "It's over 9000!");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private void CrashWithNullReference_Click(object sender, EventArgs e)
        {
            string[] values = { "a", null, "c" };
            var b = values[1].Trim();
            System.Diagnostics.Debug.WriteLine(b);
        }

        private async void CrashInsideAsyncTask_Click(object sender, EventArgs e)
        {
            await FakeService.DoStuffInBackground();
        }

        private static class FakeService
        {
            public static async Task DoStuffInBackground()
            {
                await Task.Run(() => throw new IOException("Server did not respond"));
            }
        }

        #endregion

        private void CountryCodeEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (!CountryCodeEnableCheckbox.Checked)
            {
                CountryCodeText.Text = "";
                AppCenter.SetCountryCode(null);
            }
            else
            {
                CountryCodeText.Text = RegionInfo.CurrentRegion.TwoLetterISORegionName;
                AppCenter.SetCountryCode(CountryCodeText.Text);
            }
            CountryCodeGroup.Enabled = CountryCodeEnableCheckbox.Checked;
        }

        private void BtnSave_ClickListener(object sender, EventArgs e)
        {
            AppCenter.SetCountryCode(CountryCodeText.Text.Length > 0 ? CountryCodeText.Text : null);
        }
    }
}
