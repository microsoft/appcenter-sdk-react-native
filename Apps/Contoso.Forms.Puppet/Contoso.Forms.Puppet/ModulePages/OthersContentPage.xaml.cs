using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Distribute;
using Microsoft.Azure.Mobile.Push;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    using XamarinDevice = Xamarin.Forms.Device;

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class OthersContentPage
    {

        public const string FirebaseEnabledKey = "FIREBASE_ENABLED";

        public OthersContentPage()
        {
            InitializeComponent();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.iOS)
            {
                Icon = "handbag.png";
            }
            if (XamarinDevice.RuntimePlatform != XamarinDevice.Android)
            {
                FirebaseAnalyticsEnabledSwitchCell.On = false;
                FirebaseAnalyticsEnabledSwitchCell.IsEnabled = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DistributeEnabledSwitchCell.On = Distribute.Enabled;
            DistributeEnabledSwitchCell.IsEnabled = MobileCenter.Enabled;
            PushEnabledSwitchCell.On = Push.Enabled;
            if (XamarinDevice.RuntimePlatform == XamarinDevice.Android)
            {
                if (!Application.Current.Properties.ContainsKey(FirebaseEnabledKey))
                {
                    Application.Current.Properties[FirebaseEnabledKey] = false;
                }
                FirebaseAnalyticsEnabledSwitchCell.On = (bool)Application.Current.Properties[FirebaseEnabledKey];
            }
        }

        void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            Distribute.Enabled = e.Value;
        }

        void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
	        Push.Enabled = e.Value;
        }

        void UpdateFirebaseAnalyticsEnabled(object sender, ToggledEventArgs e)
        {
            Application.Current.Properties[FirebaseEnabledKey] = e.Value;
        }
    }
}
