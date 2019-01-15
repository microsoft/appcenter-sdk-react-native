using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter;

namespace Contoso.Android.Puppet
{
    using Result = global::Android.App.Result;

    public class AppCenterFragment : PageFragment
    {
        private static readonly IDictionary<LogLevel, Action<string, string>> LogFunctions = new Dictionary<LogLevel, Action<string, string>> {
            { LogLevel.Verbose, AppCenterLog.Verbose },
            { LogLevel.Debug, AppCenterLog.Debug },
            { LogLevel.Info, AppCenterLog.Info },
            { LogLevel.Warn, AppCenterLog.Warn },
            { LogLevel.Error, AppCenterLog.Error }
        };
        private static readonly IDictionary<LogLevel, string> LogLevelNames = new Dictionary<LogLevel, string> {
            { LogLevel.Verbose, Constants.Verbose },
            { LogLevel.Debug, Constants.Debug },
            { LogLevel.Info, Constants.Info },
            { LogLevel.Warn, Constants.Warning },
            { LogLevel.Error, Constants.Error }
        };
        private LogLevel mLogWriteLevel = LogLevel.Verbose;

        private Switch AppCenterEnabledSwitch;
        private TextView LogLevelLabel;
        private EditText LogWriteMessageText;
        private EditText LogWriteTagText;
        private TextView LogWriteLevelLabel;
        private Button LogWriteButton;
        private EditText UserIdText;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.AppCenter, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Find views.
            AppCenterEnabledSwitch = view.FindViewById(Resource.Id.enabled_app_center) as Switch;
            LogLevelLabel = view.FindViewById(Resource.Id.log_level) as TextView;
            LogWriteMessageText = view.FindViewById(Resource.Id.write_log_message) as EditText;
            LogWriteTagText = view.FindViewById(Resource.Id.write_log_tag) as EditText;
            LogWriteLevelLabel = view.FindViewById(Resource.Id.write_log_level) as TextView;
            LogWriteButton = view.FindViewById(Resource.Id.write_log) as Button;
            UserIdText = view.FindViewById(Resource.Id.write_user_id) as EditText;

            // Subscribe to events.
            AppCenterEnabledSwitch.CheckedChange += UpdateEnabled;
            ((View)LogLevelLabel.Parent).Click += LogLevelClicked;
            ((View)LogWriteLevelLabel.Parent).Click += LogWriteLevelClicked;
            LogWriteButton.Click += WriteLog;
            UserIdText.KeyPress += UserIdTextKeyPressedHandler;

            UpdateState();
        }

        protected override async void UpdateState()
        {
            AppCenterEnabledSwitch.CheckedChange -= UpdateEnabled;
            AppCenterEnabledSwitch.Checked = await AppCenter.IsEnabledAsync();
            AppCenterEnabledSwitch.CheckedChange += UpdateEnabled;
            LogLevelLabel.Text = LogLevelNames[AppCenter.LogLevel];
            LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != (int)Result.Ok || data == null)
            {
                return;
            }
            var logLevel = (LogLevel)data.GetIntExtra("log_level", (int)LogLevel.Verbose);
            switch (requestCode)
            {
                case 0:
                    AppCenter.LogLevel = logLevel;
                    LogLevelLabel.Text = LogLevelNames[AppCenter.LogLevel];
                    break;
                case 1:
                    mLogWriteLevel = logLevel;
                    LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
                    break;
            }
        }

        private async void UpdateEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.IsChecked);
            AppCenterEnabledSwitch.Checked = await AppCenter.IsEnabledAsync();
        }

        private void LogLevelClicked(object sender, EventArgs e)
        {
            var intent = new Intent(Activity.ApplicationContext, typeof(LogLevelActivity));
            StartActivityForResult(intent, 0);
        }

        private void LogWriteLevelClicked(object sender, EventArgs e)
        {
            var intent = new Intent(Activity.ApplicationContext, typeof(LogLevelActivity));
            StartActivityForResult(intent, 1);
        }

        private void UserIdTextKeyPressedHandler(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                AppCenter.SetUserId(UserIdText.Text);
            }
        }

        private void WriteLog(object sender, EventArgs e)
        {
            string message = LogWriteMessageText.Text;
            string tag = LogWriteTagText.Text;
            LogFunctions[mLogWriteLevel](tag, message);
        }
    }
}
