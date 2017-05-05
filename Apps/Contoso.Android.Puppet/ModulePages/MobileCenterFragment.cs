using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Microsoft.Azure.Mobile;

namespace Contoso.Android.Puppet
{
    using Result = global::Android.App.Result;

    public class MobileCenterFragment : Fragment
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
        private LogLevel mLogWriteLevel = LogLevel.Verbose;

        private Switch MobileCenterEnabledSwitch;
        private TextView LogLevelLabel;
        private EditText LogWriteMessageText;
        private EditText LogWriteTagText;
        private TextView LogWriteLevelLabel;
        private Button LogWriteButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.MobileCenter, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Find views.
            MobileCenterEnabledSwitch = view.FindViewById(Resource.Id.enabled_mobile_center) as Switch;
            LogLevelLabel = view.FindViewById(Resource.Id.log_level) as TextView;
            LogWriteMessageText = view.FindViewById(Resource.Id.write_log_message) as EditText;
            LogWriteTagText = view.FindViewById(Resource.Id.write_log_tag) as EditText;
            LogWriteLevelLabel = view.FindViewById(Resource.Id.write_log_level) as TextView;
            LogWriteButton = view.FindViewById(Resource.Id.write_log) as Button;

            // Subscribe to events.
            MobileCenterEnabledSwitch.CheckedChange += UpdateEnabled;
            ((View)LogLevelLabel.Parent).Click += LogLevelClicked;
            ((View)LogWriteLevelLabel.Parent).Click += LogWriteLevelClicked;
            LogWriteButton.Click += WriteLog;

            UpdateState();
        }

        private void UpdateState()
        {
            MobileCenterEnabledSwitch.Checked = MobileCenter.Enabled;
            LogLevelLabel.Text = LogLevelNames[MobileCenter.LogLevel];
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
                    MobileCenter.LogLevel = logLevel;
                    LogLevelLabel.Text = LogLevelNames[MobileCenter.LogLevel];
                    break;
                case 1:
                    mLogWriteLevel = logLevel;
                    LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
                    break;
            }
        }

        private void UpdateEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            MobileCenter.Enabled = e.IsChecked;
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

        private void WriteLog(object sender, EventArgs e)
        {
            string message = LogWriteMessageText.Text;
            string tag = LogWriteTagText.Text;
            LogFunctions[mLogWriteLevel](tag, message);
        }
    }
}
