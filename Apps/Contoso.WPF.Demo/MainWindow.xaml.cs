// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Contoso.WPF.Demo.Properties;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Win32;

namespace Contoso.WPF.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class MainWindow
    {
        private string fileAttachments;

        private string textAttachments;

        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions =
            new Dictionary<LogLevel, Action<string, string>>
            {
                {Microsoft.AppCenter.LogLevel.Verbose, AppCenterLog.Verbose},
                {Microsoft.AppCenter.LogLevel.Debug, AppCenterLog.Debug},
                {Microsoft.AppCenter.LogLevel.Info, AppCenterLog.Info},
                {Microsoft.AppCenter.LogLevel.Warn, AppCenterLog.Warn},
                {Microsoft.AppCenter.LogLevel.Error, AppCenterLog.Error}
            };

        public ObservableCollection<Property> Properties = new ObservableCollection<Property>();

        public MainWindow()
        {
            InitializeComponent();
            UpdateState();
            AppCenterLogLevel.SelectedIndex = (int) AppCenter.LogLevel;
            EventProperties.ItemsSource = Properties;
            fileAttachments = Settings.Default.FileErrorAttachments;
            textAttachments = Settings.Default.TextErrorAttachments;
            TextAttachmentTextBox.Text = textAttachments;
            FileAttachmentLabel.Content = fileAttachments;
        }

        private void UpdateState()
        {
            AppCenterEnabled.IsChecked = AppCenter.IsEnabledAsync().Result;
            CrashesEnabled.IsChecked = Crashes.IsEnabledAsync().Result;
            AnalyticsEnabled.IsChecked = Analytics.IsEnabledAsync().Result;
        }

        private void AppCenterEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (AppCenterEnabled.IsChecked.HasValue)
            {
                AppCenter.SetEnabledAsync(AppCenterEnabled.IsChecked.Value).Wait();
            }
        }

        private void AnalyticsEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (AnalyticsEnabled.IsChecked.HasValue)
            {
                Analytics.SetEnabledAsync(AnalyticsEnabled.IsChecked.Value).Wait();
            }
        }

        private void AppCenterLogLevel_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AppCenter.LogLevel = (LogLevel) AppCenterLogLevel.SelectedIndex;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateState();
        }

        private void WriteLog_Click(object sender, RoutedEventArgs e)
        {
            if (LogLevel.SelectedIndex == -1)
            {
                return;
            }

            var level = (LogLevel) LogLevel.SelectedIndex;
            var tag = LogTag.Text;
            var message = LogMessage.Text;
            LogFunctions[level](tag, message);
        }

        private void TrackEvent_Click(object sender, RoutedEventArgs e)
        {
            var name = EventName.Text;
            var propertiesDictionary = Properties.Where(property => property.Key != null && property.Value != null)
                .ToDictionary(property => property.Key, property => property.Value);
            Analytics.TrackEvent(name, propertiesDictionary);
        }

        private void FileErrorAttachment_Click(object sender, RoutedEventArgs e)
        {
            var filePath = string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true
            };
            var result = openFileDialog.ShowDialog();
            if (result ?? false)
            {
                filePath = openFileDialog.FileName;
                FileAttachmentLabel.Content = filePath;
            }
            else
            {
                FileAttachmentLabel.Content = "The file isn't selected";
            }
            Settings.Default.FileErrorAttachments = filePath;
            Settings.Default.Save();
        }

        #region Crash

        private void CrashesEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (CrashesEnabled.IsChecked.HasValue)
            {
                Crashes.SetEnabledAsync(CrashesEnabled.IsChecked.Value).Wait();
            }
        }

        public class NonSerializableException : Exception
        {
        }

        private void CrashWithTestException_Click(object sender, RoutedEventArgs e)
        {
            Crashes.GenerateTestCrash();
        }

        private void CrashWithNonSerializableException_Click(object sender, RoutedEventArgs e)
        {
            throw new NonSerializableException();
        }

        private void CrashWithDivisionByZero_Click(object sender, RoutedEventArgs e)
        {
            _ = 42 / int.Parse("0");
        }

        private void CrashWithAggregateException_Click(object sender, RoutedEventArgs e)
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

        private void CrashWithNullReference_Click(object sender, RoutedEventArgs e)
        {
            string[] values = { "a", null, "c" };
            var b = values[1].Trim();
            System.Diagnostics.Debug.WriteLine(b);
        }

        private async void CrashInsideAsyncTask_Click(object sender, RoutedEventArgs e)
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

        private void TextAttachmentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textAttachments = TextAttachmentTextBox.Text;
            Settings.Default.TextErrorAttachments = textAttachments;
            Settings.Default.Save();
        }
    }
}
