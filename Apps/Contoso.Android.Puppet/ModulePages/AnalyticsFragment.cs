using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;

namespace Contoso.Android.Puppet
{
    public class AnalyticsFragment : Fragment
    {
        private readonly IDictionary<string, string> mEventProperties = new Dictionary<string, string>();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Analytics, container, false);
        }

        private Switch AnalyticsEnabledSwitch;
        private EditText EventNameText;
        private TextView PropertiesCountLabel;
        private Button AddPropertyButton;
        private Button TrackEventButton;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            AnalyticsEnabledSwitch = view.FindViewById(Resource.Id.enabled) as Switch;
            EventNameText = view.FindViewById(Resource.Id.event_name) as EditText;
            PropertiesCountLabel = view.FindViewById(Resource.Id.properties_count) as TextView;
            AddPropertyButton = view.FindViewById(Resource.Id.add_property) as Button;
            TrackEventButton = view.FindViewById(Resource.Id.track_event) as Button;

            AnalyticsEnabledSwitch.CheckedChange += UpdateEnabled;
            ((View)PropertiesCountLabel.Parent).Click += Properties;
            AddPropertyButton.Click += AddProperty;
            TrackEventButton.Click += TrackEvent;

            UpdateState();
        }

        public override bool UserVisibleHint
        {
            get { return base.UserVisibleHint; }
            set
            {
                base.UserVisibleHint = value;
                if (value && IsResumed)
                    UpdateState();
            }
        }

        private void UpdateState()
        {
            AnalyticsEnabledSwitch.Checked = Analytics.Enabled;
            AnalyticsEnabledSwitch.Enabled = MobileCenter.Enabled;
            PropertiesCountLabel.Text = mEventProperties.Count.ToString();
        }

        private void UpdateEnabled(object sender, EventArgs e)
        {
            Analytics.Enabled = AnalyticsEnabledSwitch.Checked;
            AnalyticsEnabledSwitch.Checked = Analytics.Enabled;
        }

        private void Properties(object sender, EventArgs e)
        {
            var intent = new Intent(Activity, typeof(PropertiesActivity));
            intent.PutExtra("properties", mEventProperties.Select(i => i.Key + ": " + i.Value).ToArray());
            StartActivity(intent);
        }

        private void AddProperty(object sender, EventArgs e)
        {
        }

        private void TrackEvent(object sender, EventArgs e)
        {
            Analytics.TrackEvent(EventNameText.Text, mEventProperties.Count > 0 ? mEventProperties : null);
            mEventProperties.Clear();
            PropertiesCountLabel.Text = mEventProperties.Count.ToString();
        }
    }
}
