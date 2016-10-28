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
        private Dictionary<string, Action<string, string>> LogFunctions;
        private Dictionary<LogLevel, string> LogLevelNames;
         
        public CoreContentPage()
        {
            InitializeComponent();

            LogFunctions = new Dictionary<string, Action<string, string>>();
            LogFunctions.Add(Constants.Verbose, SonomaLog.Verbose);
            LogFunctions.Add(Constants.Debug, SonomaLog.Debug);
            LogFunctions.Add(Constants.Info, SonomaLog.Info);
            LogFunctions.Add(Constants.Warning, SonomaLog.Warn);
            LogFunctions.Add(Constants.Error, SonomaLog.Error);

            LogLevelNames = new Dictionary<LogLevel, string>();
            LogLevelNames.Add(LogLevel.Verbose, Constants.Verbose);
            LogLevelNames.Add(LogLevel.Debug, Constants.Debug);
            LogLevelNames.Add(LogLevel.Info, Constants.Info);
            LogLevelNames.Add(LogLevel.Warn, Constants.Warning);
            LogLevelNames.Add(LogLevel.Error, Constants.Error);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LogLevelLabel.Text = LogLevelNames[Sonoma.LogLevel];
        }

        void LogLevelCellTapped(object sender, System.EventArgs e)
        {
            ((NavigationPage)App.Current.MainPage).PushAsync(new LogLevelPage());
        }

        void WriteLog(object sender, System.EventArgs e)
        {
            string level = LogWriteLevelLabel.Text;
            string message = LogMessageEntryCell.Text;
            string tag = LogTagEntryCell.Text;
            LogFunctions[level](tag, message);
        }
    }
}
