// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Contoso.WinForms.Demo.Properties;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Contoso.WinForms.Demo
{
    public partial class MainForm : Form
    {
        private string fileAttachments;
        private string textAttachments;

        public MainForm()
        {
            InitializeComponent();
            UpdateState();
            fileAttachments = Settings.Default.FileErrorAttachments;
            textAttachments = Settings.Default.TextErrorAttachments;
            TextAttachmentTextBox.Text = textAttachments;
            FileAttachmentPathLabel.Text = fileAttachments;
        }

        private void UpdateState()
        {
            AppCenterEnabled.Checked = AppCenter.IsEnabledAsync().Result;
            AnalyticsEnabled.Checked = Analytics.IsEnabledAsync().Result;
            CrashesEnabled.Checked = Crashes.IsEnabledAsync().Result;
            AnalyticsEnabled.Enabled = AppCenterEnabled.Checked;
            CrashesEnabled.Enabled = AppCenterEnabled.Checked;
        }

        private void AppCenterEnabled_CheckedChanged(object sender, EventArgs e)
        {
            AppCenter.SetEnabledAsync(AppCenterEnabled.Checked).Wait();
        }

        private void AnalyticsEnabled_CheckedChanged(object sender, EventArgs e)
        {
            AnalyticsEnabled.Enabled = AppCenterEnabled.Checked;
            Analytics.SetEnabledAsync(AnalyticsEnabled.Checked).Wait();
        }

        private void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateState();
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
            CrashesEnabled.Enabled = AppCenterEnabled.Checked;
            await Crashes.SetEnabledAsync(CrashesEnabled.Checked);
        }

        private void CrashWithTestException_Click(object sender, EventArgs e)
        {
            HandleOrThrow(() => Crashes.GenerateTestCrash());
        }

        private void CrashWithNonSerializableException_Click(object sender, EventArgs e)
        {
            HandleOrThrow(() => throw new NonSerializableException());
        }

        private void CrashWithDivisionByZero_Click(object sender, EventArgs e)
        {
            HandleOrThrow(() => { _ = 42 / int.Parse("0"); });
        }

        private void CrashWithAggregateException_Click(object sender, EventArgs e)
        {
            HandleOrThrow(() => throw GenerateAggregateException());
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
            HandleOrThrow(() =>
            {
                string[] values = { "a", null, "c" };
                var b = values[1].Trim();
                System.Diagnostics.Debug.WriteLine(b);
            });
        }

        private async void CrashInsideAsyncTask_Click(object sender, EventArgs e)
        {
            try
            {
                await FakeService.DoStuffInBackground();
            }
            catch (Exception ex) when (HandleExceptions.Checked)
            {
                TrackException(ex);
            }
        }

        private static class FakeService
        {
            public static async Task DoStuffInBackground()
            {
                await Task.Run(() => throw new IOException("Server did not respond"));
            }
        }

        #endregion

        private void SelectFileAttachmentButton_ClickListener(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true
            };
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                FileAttachmentPathLabel.Text = filePath;
            }
            else
            {
                FileAttachmentPathLabel.Text = "The file isn't selected";
            }
            Settings.Default.FileErrorAttachments = filePath;
            Settings.Default.Save();
        }

        private void TextAttachmentTextBox_TextChanged(object sender, EventArgs e)
        {
            textAttachments = TextAttachmentTextBox.Text;
            Settings.Default.TextErrorAttachments = textAttachments;
            Settings.Default.Save();
        }

        private void TrackException(Exception e)
        {
            Dictionary<string, string> properties = null;
            Crashes.TrackError(e, properties, Program.GetErrorAttachments().ToArray());
        }

        private void HandleOrThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e) when (HandleExceptions.Checked)
            {
                TrackException(e);
            }
        }
    }
}
