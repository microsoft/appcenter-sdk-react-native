using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;


namespace Contoso.WPF.Puppet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions = new Dictionary<LogLevel, Action<string, string>> {
            { LogLevel.Verbose, AppCenterLog.Verbose },
            { LogLevel.Debug, AppCenterLog.Debug },
            { LogLevel.Info, AppCenterLog.Info },
            { LogLevel.Warn, AppCenterLog.Warn },
            { LogLevel.Error, AppCenterLog.Error }
        };

        public ObservableCollection<Property> Properties = new ObservableCollection<Property>();

        public MainWindow()
        {
            InitializeComponent();
            UpdateState();
            mobileCenterLogLevel.SelectedIndex = (int) AppCenter.LogLevel;
            eventProperties.ItemsSource = Properties;
        }


        private void UpdateState()
        {
            mobileCenterEnabled.IsChecked = AppCenter.IsEnabledAsync().Result;
            analyticsEnabled.IsChecked = Analytics.IsEnabledAsync().Result;
        }

        private void mobileCenterEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (mobileCenterEnabled.IsChecked.HasValue)
            {
                AppCenter.SetEnabledAsync(mobileCenterEnabled.IsChecked.Value).Wait();
            }
        }

        private void analyticsEnabled_Checked(object sender, RoutedEventArgs e)
        {
            if (analyticsEnabled.IsChecked.HasValue)
            {
                Analytics.SetEnabledAsync(analyticsEnabled.IsChecked.Value).Wait();
            }
        }


        private void mobileCenterLogLevel_SelectionChanged(object sender, RoutedEventArgs e)
        {
            AppCenter.LogLevel = (LogLevel) mobileCenterLogLevel.SelectedIndex;
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
            var level = (LogLevel) logLevel.SelectedIndex;
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

        private void CrashWithNullReference_Click(object sender, RoutedEventArgs e)
        {
            string[] values = { "a", null, "c" };
            var b = values[1].Trim();
            System.Diagnostics.Debug.WriteLine(b);
        }
    }
}
