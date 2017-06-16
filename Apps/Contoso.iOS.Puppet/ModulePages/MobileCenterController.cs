using System;
using System.Collections.Generic;
using Foundation;
using Microsoft.Azure.Mobile;
using UIKit;

namespace Contoso.iOS.Puppet
{
    public partial class MobileCenterController : UITableViewController
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

        public MobileCenterController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            MobileCenterEnabledSwitch.On = MobileCenter.IsEnabledAsync().Result;
            LogLevelLabel.Text = LogLevelNames[MobileCenter.LogLevel];
            LogWriteLevelLabel.Text = LogLevelNames[mLogWriteLevel];
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
                            MobileCenter.LogLevel = level;
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
            MobileCenter.SetEnabled(MobileCenterEnabledSwitch.On);
            MobileCenterEnabledSwitch.On = MobileCenter.IsEnabledAsync().Result;
        }

        partial void WriteLog()
        {
            string message = LogWriteMessage.Text;
            string tag = LogWriteTag.Text;
            LogFunctions[mLogWriteLevel](tag, message);
        }
    }
}
