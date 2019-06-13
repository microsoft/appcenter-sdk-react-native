// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Contoso.WPF.Puppet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions =
            new Dictionary<LogLevel, Action<string, string>>
            {
                {LogLevel.Verbose, AppCenterLog.Verbose},
                {LogLevel.Debug, AppCenterLog.Debug},
                {LogLevel.Info, AppCenterLog.Info},
                {LogLevel.Warn, AppCenterLog.Warn},
                {LogLevel.Error, AppCenterLog.Error}
            };

        public ObservableCollection<Property> Properties = new ObservableCollection<Property>();

        public MainWindow()
        {
            InitializeComponent();
            UpdateState();
            appCenterLogLevel.SelectedIndex = (int)AppCenter.LogLevel;
            eventProperties.ItemsSource = Properties;
        }


        private void UpdateState()
        {
            appCenterEnabled.IsChecked = AppCenter.IsEnabledAsync().Result;
            analyticsEnabled.IsChecked = Analytics.IsEnabledAsync().Result;
        }

        private void appCenterEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (appCenterEnabled.IsChecked.HasValue)
            {
                AppCenter.SetEnabledAsync(appCenterEnabled.IsChecked.Value).Wait();
            }
        }

        private void analyticsEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (analyticsEnabled.IsChecked.HasValue)
            {
                Analytics.SetEnabledAsync(analyticsEnabled.IsChecked.Value).Wait();
            }
        }


        private void appCenterLogLevel_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AppCenter.LogLevel = (LogLevel)appCenterLogLevel.SelectedIndex;
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateState();
        }

        private void writeLog_Click(object sender, RoutedEventArgs e)
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

        private void trackEvent_Click(object sender, RoutedEventArgs e)
        {
            var name = eventName.Text;
            var propertiesDictionary = Properties.Where(property => property.Key != null && property.Value != null)
                .ToDictionary(property => property.Key, property => property.Value);
            Analytics.TrackEvent(name, propertiesDictionary);
        }

        #region Crash

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
    }
}
