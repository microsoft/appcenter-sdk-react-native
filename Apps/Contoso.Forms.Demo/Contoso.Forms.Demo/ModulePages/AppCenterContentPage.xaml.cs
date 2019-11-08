// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
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

            // Setup start type dropdown choices
            foreach (var startType in StartTypeUtils.GetStartTypeChoiceStrings())
            {
                this.StartTypePicker.Items.Add(startType);
            }
            this.StartTypePicker.SelectedIndex = (int)(StartTypeUtils.GetPersistedStartType());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LogLevelLabel.Text = LogLevelNames[AppCenter.LogLevel];
            AppCenterEnabledSwitchCell.On = await AppCenter.IsEnabledAsync();
            if (Application.Current.Properties.ContainsKey(Constants.UserId) && Application.Current.Properties[Constants.UserId] is string id)
            {
                UserIdEntry.Text = id;
            }
            UserIdEntry.Unfocused += (sender, args) =>
            {
                var inputText = UserIdEntry.Text;
                var text = string.IsNullOrEmpty(inputText) ? null : inputText;
                AppCenter.SetUserId(text);
                Application.Current.Properties[Constants.UserId] = text;
            };
        }

        async void ChangeStartType(object sender, PropertyChangedEventArgs e)
        {
            // IOS sends an event every time user rests their selection on an item without hitting "done", and the only event they send when hitting "done" is that the control is no longer focused.
            // So we'll process the change at that time. This works for android as well.
            if (e.PropertyName == "IsFocused" && !this.StartTypePicker.IsFocused)
            {
                var newSelectionCandidate = this.StartTypePicker.SelectedIndex;
                var persistedStartType = StartTypeUtils.GetPersistedStartType();
                if (newSelectionCandidate != (int)persistedStartType)
                {
                    StartTypeUtils.SetPersistedStartType((StartType)newSelectionCandidate);
                    await Application.Current.SavePropertiesAsync();
                    await DisplayAlert("Start Type Changed", "Start type has changed, which alters the app secret. Please close and re-run the app for the new app secret to take effect.", "OK");
                }
            }
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
            if (tag != null && message != null)
            {
                LogFunctions[LogWriteLevel](tag, message);
            }
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
}
