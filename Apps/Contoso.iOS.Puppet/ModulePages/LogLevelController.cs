using System;
using System.Collections.Generic;
using Foundation;
using Microsoft.AppCenter;
using UIKit;

namespace Contoso.iOS.Puppet
{
    public partial class LogLevelController : UITableViewController
    {
        private readonly IDictionary<string, LogLevel> mLogLevels = new Dictionary<string, LogLevel> {
            { Constants.Verbose, LogLevel.Verbose },
            { Constants.Debug, LogLevel.Debug },
            { Constants.Info, LogLevel.Info },
            { Constants.Warning, LogLevel.Warn },
            { Constants.Error, LogLevel.Error }
        };
        public event Action<LogLevel> LevelSelected;

        public LogLevelController(IntPtr handle) : base(handle)
        {
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath);
            LevelSelected?.Invoke(mLogLevels[cell.TextLabel.Text]);
            tableView.DeselectRow(indexPath, true);
            NavigationController.PopViewController(true);
        }
    }
}
