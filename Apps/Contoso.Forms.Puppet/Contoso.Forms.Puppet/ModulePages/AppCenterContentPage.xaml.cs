// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AppCenterContentPage : ContentPage
    {

        // E.g., calling LogFunctions["Verbose"](tag, msg) will be
        // equivalent to calling Verbose(tag, msg)
        Dictionary<LogLevel, Action<string, string>> LogFunctions;
        Dictionary<LogLevel, string> LogLevelNames;
        LogLevel LogWriteLevel;

        public AppCenterContentPage()
        {
            InitializeComponent();

            LogFunctions = new Dictionary<LogLevel, Action<string, string>>();
            LogFunctions.Add(LogLevel.Verbose, AppCenterLog.Verbose);
            LogFunctions.Add(LogLevel.Debug, AppCenterLog.Debug);
            LogFunctions.Add(LogLevel.Info, AppCenterLog.Info);
            LogFunctions.Add(LogLevel.Warn, AppCenterLog.Warn);
            LogFunctions.Add(LogLevel.Error, AppCenterLog.Error);

            LogLevelNames = new Dictionary<LogLevel, string>();
            LogLevelNames.Add(LogLevel.Verbose, Constants.Verbose);
            LogLevelNames.Add(LogLevel.Debug, Constants.Debug);
            LogLevelNames.Add(LogLevel.Info, Constants.Info);
            LogLevelNames.Add(LogLevel.Warn, Constants.Warning);
            LogLevelNames.Add(LogLevel.Error, Constants.Error);

            LogWriteLevel = LogLevel.Verbose;
            UpdateLogWriteLevelLabel();


            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "bolt.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LogLevelLabel.Text = LogLevelNames[AppCenter.LogLevel];
            AppCenterEnabledSwitchCell.On = await AppCenter.IsEnabledAsync();

        }

        void LogLevelCellTapped(object sender, EventArgs e)
        {
            var page = new LogLevelPage();
            page.LevelSelected += (LogLevel level) =>
            {
                AppCenter.LogLevel = level;
            };
            ((NavigationPage)Application.Current.MainPage).PushAsync(page);
        }

        void LogWriteLevelCellTapped(object sender, EventArgs e)
        {
            var page = new LogLevelPage();
            page.LevelSelected += (LogLevel level) =>
            {
                LogWriteLevel = level;
                UpdateLogWriteLevelLabel();
            };
            ((NavigationPage)Application.Current.MainPage).PushAsync(page);
        }

        void WriteLog(object sender, EventArgs e)
        {
            string message = LogMessageEntryCell.Text;
            string tag = LogTagEntryCell.Text;
            LogFunctions[LogWriteLevel](tag, message);
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.Value);
        }

        void UpdateLogWriteLevelLabel()
        {
            LogWriteLevelLabel.Text = LogLevelNames[LogWriteLevel];
        }
    }

    [Android.Runtime.Preserve(AllMembers = true)]
    public class EntryCellTextChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string UserIdKey = "userId";

        private string _userId;

        public EntryCellTextChanged()
        {
            if (Application.Current.Properties.ContainsKey(UserIdKey) && Application.Current.Properties[UserIdKey] is string id)
            {
                UserId = id;
            }
        }

        public string UserId
        {
            get { return _userId; }

            set
            {
                _userId = value;
                OnTextChanged(_userId);
            }
        }

        public ICommand TextChanged;

        protected virtual void OnTextChanged(string inputText)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(inputText));
                var text = string.IsNullOrEmpty(inputText) ? null : inputText;
                AppCenter.SetUserId(text);
                Application.Current.Properties[UserIdKey] = text;
            }
        }
    }
}
