using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using UIKit;

namespace Contoso.iOS.Puppet
{

    public partial class AnalyticsController : UITableViewController
    {
        public class PropertiesTableSource : UITableViewSource
        {
            private readonly IDictionary<string, string> mEventProperties;
            private readonly string[] mKeys;

            public PropertiesTableSource(IDictionary<string, string> eventProperties)
            {
                mEventProperties = eventProperties;
                mKeys = mEventProperties.Keys.ToArray();
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return mKeys.Length;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell("PropertiesCell");
                var key = mKeys[indexPath.Row];
                var value = mEventProperties[key];

                cell.TextLabel.Text = key;
                cell.DetailTextLabel.Text = value;

                return cell;
            }
        }

        private readonly IDictionary<string, string> mEventProperties = new Dictionary<string, string>();

        public AnalyticsController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            AnalyticsEnabledSwitch.On = Analytics.IsEnabledAsync().Result;
            AnalyticsEnabledSwitch.Enabled = AppCenter.IsEnabledAsync().Result;
            NumPropertiesLabel.Text = mEventProperties.Count.ToString();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var tableViewContoller = segue.DestinationViewController as UITableViewController;
            if (tableViewContoller != null)
            {
                tableViewContoller.TableView.Source = new PropertiesTableSource(mEventProperties);
            }
        }

        partial void UpdateEnabled()
        {
            Analytics.SetEnabledAsync(AnalyticsEnabledSwitch.On).Wait();
            AnalyticsEnabledSwitch.On = Analytics.IsEnabledAsync().Result;
        }

        partial void AddProperty()
        {
            var alert = UIAlertController.Create("Add Property", "Please enter a property values to add to the event", UIAlertControllerStyle.Alert);
            alert.AddTextField(textField => textField.Placeholder = "Property Name");
            alert.AddTextField(textField => textField.Placeholder = "Property Value");
            alert.AddAction(UIAlertAction.Create("Add Property", UIAlertActionStyle.Default, Action =>
            {
                mEventProperties.Add(alert.TextFields[0].Text, alert.TextFields[1].Text);
                NumPropertiesLabel.Text = mEventProperties.Count.ToString();
            }));
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        partial void TrackEvent()
        {
            Analytics.TrackEvent(EventName.Text, mEventProperties.Count > 0 ? mEventProperties : null);
            mEventProperties.Clear();
            NumPropertiesLabel.Text = mEventProperties.Count.ToString();
        }
    }
}
