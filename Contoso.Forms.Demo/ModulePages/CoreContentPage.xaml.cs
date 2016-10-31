using System;
using System.Collections.Generic;
using Microsoft.Sonoma.Core;

using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    public partial class CoreContentPage : ContentPage
    {
        // E.g., calling LogFunctions["Verbose"](tag, msg) will be
        // equivalent to calling Verbose(tag, msg)
        private Dictionary<LogLevel, Action<string, string>> LogFunctions;
        private Dictionary<LogLevel, string> LogLevelNames;
        private LogLevel LogWriteLevel;

        public CoreContentPage()
        {
            InitializeComponent();

            LogFunctions = new Dictionary<LogLevel, Action<string, string>>();
            LogFunctions.Add(LogLevel.Verbose, SonomaLog.Verbose);
            LogFunctions.Add(LogLevel.Debug, SonomaLog.Debug);
            LogFunctions.Add(LogLevel.Info, SonomaLog.Info);
            LogFunctions.Add(LogLevel.Warn, SonomaLog.Warn);
            LogFunctions.Add(LogLevel.Error, SonomaLog.Error);

            LogLevelNames = new Dictionary<LogLevel, string>();
            LogLevelNames.Add(LogLevel.Verbose, Constants.Verbose);
            LogLevelNames.Add(LogLevel.Debug, Constants.Debug);
            LogLevelNames.Add(LogLevel.Info, Constants.Info);
            LogLevelNames.Add(LogLevel.Warn, Constants.Warning);
            LogLevelNames.Add(LogLevel.Error, Constants.Error);

            LogWriteLevel = LogLevel.Verbose;
            UpdateLogWriteLevelLabel();

            if (Device.OS == TargetPlatform.iOS)
            {
                Icon = "bolt.png";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LogLevelLabel.Text = LogLevelNames[Sonoma.LogLevel];
        }

        void LogLevelCellTapped(object sender, System.EventArgs e)
        {
            var page = new LogLevelPage();
            page.LevelSelected += (LogLevel level) => { 
                Sonoma.LogLevel = level;
            };
            ((NavigationPage)App.Current.MainPage).PushAsync(page);
        }

        void LogWriteLevelCellTapped(object sender, System.EventArgs e)
        {
            var page = new LogLevelPage();
            page.LevelSelected += (LogLevel level) =>
            {
                LogWriteLevel = level;
                UpdateLogWriteLevelLabel();
            };
            ((NavigationPage)App.Current.MainPage).PushAsync(page);
        }

        void WriteLog(object sender, System.EventArgs e)
        {
            string message = LogMessageEntryCell.Text;
            string tag = LogTagEntryCell.Text;
            LogFunctions[LogWriteLevel](tag, message);
        }

        void UpdateEnabled(object sender, System.EventArgs e)
        {
            if (SonomaEnabledSwitchCell != null)
            {
                Sonoma.Enabled = SonomaEnabledSwitchCell.On;
            }
        }

        void UpdateLogWriteLevelLabel()
        {
            LogWriteLevelLabel.Text = LogLevelNames[LogWriteLevel];
        }
    }
}
