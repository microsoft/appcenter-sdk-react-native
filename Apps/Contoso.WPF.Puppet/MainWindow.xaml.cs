using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;


namespace Contoso.WPF.Puppet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions = new Dictionary<LogLevel, Action<string, string>> {
            { LogLevel.Verbose, MobileCenterLog.Verbose },
            { LogLevel.Debug, MobileCenterLog.Debug },
            { LogLevel.Info, MobileCenterLog.Info },
            { LogLevel.Warn, MobileCenterLog.Warn },
            { LogLevel.Error, MobileCenterLog.Error }
        };
        private static readonly IDictionary<LogLevel, string> LogLevelNames = new Dictionary<LogLevel, string> {
            { LogLevel.Verbose, Constants.Verbose },
            { LogLevel.Debug, Constants.Debug },
            { LogLevel.Info, Constants.Info },
            { LogLevel.Warn, Constants.Warning },
            { LogLevel.Error, Constants.Error }
        };

        public ObservableCollection<Property> properties = new ObservableCollection<Property>();

        public MainWindow()
        {
            InitializeComponent();
            UpdateState();
            mobileCenterLogLevel.SelectedIndex = (int) MobileCenter.LogLevel;
            eventProperties.ItemsSource = properties;
        }

        private void UpdateState()
        {
            mobileCenterEnabled.IsChecked = MobileCenter.Enabled;
            analyticsEnabled.IsChecked = Analytics.Enabled;
            analyticsEnabled.IsEnabled = MobileCenter.Enabled;
        }

        private void mobileCenterEnabled_Checked(object sender, RoutedEventArgs e)
        {
            MobileCenter.Enabled = (bool) mobileCenterEnabled.IsChecked;
        }

        private void analyticsEnabled_Checked(object sender, RoutedEventArgs e)
        {
            Analytics.Enabled = (bool) analyticsEnabled.IsChecked;
        }

        private void mobileCenterLogLevel_SelectionChanged(object sender, RoutedEventArgs e)
        {
            MobileCenter.LogLevel = (LogLevel) mobileCenterLogLevel.SelectedIndex;
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
            Analytics.TrackEvent(name, properties.Where(property => property.Key != null && property.Value != null).ToDictionary(property => property.Key, property => property.Value));
        }
    }
}
