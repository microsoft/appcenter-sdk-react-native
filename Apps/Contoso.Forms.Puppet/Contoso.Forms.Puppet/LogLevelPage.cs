using System;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class LogLevelPage
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

        void UpdateLogLevel(object sender, EventArgs e)
        {
            string level = ((Button)sender).Text;
            LevelSelected?.Invoke(LogLevels[level]);
            ((NavigationPage)Application.Current.MainPage).PopAsync();
        }
    }
}
