using System;
using System.Collections.Generic;
using Foundation;
using Microsoft.AppCenter;
using UIKit;

namespace Contoso.iOS.Puppet
{
    public partial class AppCenterController : UITableViewController
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

        public AppCenterController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AppCenterEnabledSwitch.On = AppCenter.IsEnabledAsync().Result;
            LogLevelLabel.Text = LogLevelNames[AppCenter.LogLevel];
            LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
        }

        partial void UpdateUserId(UITextField sender)
        {
            var text = string.IsNullOrEmpty(sender.Text) ? null : sender.Text;
            AppCenter.SetUserId(text);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var logLevelContoller = segue.DestinationViewController as LogLevelController;
            if (logLevelContoller != null)
            {
                switch (segue.Identifier)
                {
                    case "LogLevel":
                        logLevelContoller.LevelSelected += level =>
                        {
                            AppCenter.LogLevel = level;
                        };
                        break;
                    case "LogWriteLevel":
                        logLevelContoller.LevelSelected += level =>
                        {
                            mLogWriteLevel = level;
                            LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
                        };
                        break;
                }
            }
        }

        partial void UpdateEnabled()
        {
            AppCenter.SetEnabledAsync(AppCenterEnabledSwitch.On).Wait();
            AppCenterEnabledSwitch.On = AppCenter.IsEnabledAsync().Result;
        }

        partial void WriteLog()
        {
            string message = LogWriteMessage.Text;
            string tag = LogWriteTag.Text;
            LogFunctions[mLogWriteLevel](tag, message);
        }
    }
}
