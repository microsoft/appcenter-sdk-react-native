// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Contoso.WPF.Puppet.DotNetCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class MainWindow
    {
        private string fileAttachments;
        private string textAttachments;

        public ObservableCollection<Property> EventPropertiesSource = new ObservableCollection<Property>();
        public ObservableCollection<Property> ErrorPropertiesSource = new ObservableCollection<Property>();

        public MainWindow()
        {
            InitializeComponent();
            UpdateState();
            EventProperties.ItemsSource = EventPropertiesSource;
            ErrorProperties.ItemsSource = ErrorPropertiesSource;
            fileAttachments = Settings.Default.FileErrorAttachments;
            textAttachments = Settings.Default.TextErrorAttachments;
            TextAttachmentTextBox.Text = textAttachments;
            FileAttachmentLabel.Content = fileAttachments;
            if (!string.IsNullOrEmpty(Settings.Default.CountryCode))
            {
                CountryCodeEnableCheckbox.IsChecked = true;
                CountryCodeText.Text = Settings.Default.CountryCode;
            }
            if (!string.IsNullOrEmpty(Settings.Default.UserId))
            {
                UserId.Text = Settings.Default.UserId;
            }
        }

        private void UpdateState()
        {
            AppCenterEnabled.IsChecked = AppCenter.IsEnabledAsync().Result;
            CrashesEnabled.IsChecked = Crashes.IsEnabledAsync().Result;
            AnalyticsEnabled.IsChecked = Analytics.IsEnabledAsync().Result;
            AnalyticsEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
            CrashesEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
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
            AnalyticsEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
            Analytics.SetEnabledAsync(AnalyticsEnabled.IsChecked.Value).Wait();
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateState();
        }

        private void TrackEvent_Click(object sender, RoutedEventArgs e)
        {
            var name = EventName.Text;
            var propertiesDictionary = EventPropertiesSource.Where(property => property.Key != null && property.Value != null)
                .ToDictionary(property => property.Key, property => property.Value);
            Analytics.TrackEvent(name, propertiesDictionary);
        }

        private void CountryCodeEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (!CountryCodeEnableCheckbox.IsChecked.HasValue)
            {
                return;
            }
            if (!CountryCodeEnableCheckbox.IsChecked.Value)
            {
                CountryCodeText.Text = "";
                SaveCountryCode();
            }
            else
            {
                if (string.IsNullOrEmpty(Settings.Default.CountryCode))
                {
                    CountryCodeText.Text = RegionInfo.CurrentRegion.TwoLetterISORegionName;
                }
                else
                {
                    CountryCodeText.Text = Settings.Default.CountryCode;
                }
            }
            CountryCodePanel.IsEnabled = CountryCodeEnableCheckbox.IsChecked.Value;
        }

        private void CountryCodeSave_ClickListener(object sender, RoutedEventArgs e)
        {
            SaveCountryCode();
        }

        private void SaveCountryCode()
        {
            CountryCodeNotice.Visibility = Visibility.Visible;
            Settings.Default.CountryCode = CountryCodeText.Text;
            Settings.Default.Save();
            AppCenter.SetCountryCode(CountryCodeText.Text.Length > 0 ? CountryCodeText.Text : null);
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

        private void TextAttachmentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textAttachments = TextAttachmentTextBox.Text;
            Settings.Default.TextErrorAttachments = textAttachments;
            Settings.Default.Save();
        }

        private void UserId_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                HandleUserIdChange();
            }
        }

        private void UserId_LostFocus(object sender, RoutedEventArgs e)
        {
            HandleUserIdChange();
        }

        private void HandleUserIdChange()
        {
            var userId = UserId.Text;
            var text = string.IsNullOrEmpty(userId) ? null : userId;
            AppCenter.SetUserId(text);
            Settings.Default.UserId = text;
            Settings.Default.Save();
        }

        #region Crash

        private void CrashesEnabled_Checked(object sender, RoutedEventArgs e)
        {
            CrashesEnabled.IsEnabled = AppCenterEnabled.IsChecked.Value;
            Crashes.SetEnabledAsync(CrashesEnabled.IsChecked.Value).Wait();
        }

        public class NonSerializableException : Exception
        {
        }

        private void CrashWithTestException_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() => Crashes.GenerateTestCrash());
        }

        private void CrashWithNonSerializableException_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() => throw new NonSerializableException());
        }

        private void CrashWithDivisionByZero_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() => { _ = 42 / int.Parse("0"); });
        }

        private void CrashWithAggregateException_Click(object sender, RoutedEventArgs e)
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

        private void CrashWithNullReference_Click(object sender, RoutedEventArgs e)
        {
            HandleOrThrow(() =>
            {
                string[] values = { "a", null, "c" };
                var b = values[1].Trim();
                System.Diagnostics.Debug.WriteLine(b);
            });
        }

        private async void CrashInsideAsyncTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await FakeService.DoStuffInBackground();
            }
            catch (Exception ex) when (HandleExceptions.IsChecked.Value)
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

        private void TrackException(Exception e)
        {
            var properties = ErrorPropertiesSource.Where(property => property.Key != null && property.Value != null).ToDictionary(property => property.Key, property => property.Value);
            if (properties.Count == 0)
            {
                properties = null;
            }
            Crashes.TrackError(e, properties, App.GetErrorAttachments().ToArray());
        }

        void HandleOrThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e) when (HandleExceptions.IsChecked.Value)
            {
                TrackException(e);
            }
        }

        #endregion
    }
}
