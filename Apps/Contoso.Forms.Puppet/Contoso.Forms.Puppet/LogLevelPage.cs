using Microsoft.AAppCenter
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class LogLevelPage : ContentPage
    {
        private Dictionary<string, LogLevel> LogLevels;
        public event Action<LogLevel> LevelSelected;

        public LogLevelPage()
        {
            InitializeComponent();
            Title = "Log Level";

            LogLevels = new Dictionary<string, LogLevel>();
            LogLevels.Add(Constants.Verbose, LogLevel.Verbose);
            LogLevels.Add(Constants.Debug, LogLevel.Debug);
            LogLevels.Add(Constants.Info, LogLevel.Info);
            LogLevels.Add(Constants.Warning, LogLevel.Warn);
            LogLevels.Add(Constants.Error, LogLevel.Error);
        }

        void UpdateLogLevel(object sender, System.EventArgs e)
        {
            string level = ((TextCell)sender).Text;
            LevelSelected.Invoke(LogLevels[level]);
            ((NavigationPage)App.Current.MainPage).PopAsync();
        }
    }
}
