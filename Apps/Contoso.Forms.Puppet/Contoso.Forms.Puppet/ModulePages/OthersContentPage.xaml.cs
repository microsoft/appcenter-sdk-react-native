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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitchCell.IsEnabled = await MobileCenter.IsEnabledAsync();
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
            PushEnabledSwitchCell.IsEnabled = await MobileCenter.IsEnabledAsync();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.Android)
            {
                if (!Application.Current.Properties.ContainsKey(FirebaseEnabledKey))
                {
                    Application.Current.Properties[FirebaseEnabledKey] = false;
                }
                FirebaseAnalyticsEnabledSwitchCell.On = (bool)Application.Current.Properties[FirebaseEnabledKey];
            }
        }

        async void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.Value);
        }

        async void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
	        await Push.SetEnabledAsync(e.Value);
        }

        void UpdateFirebaseAnalyticsEnabled(object sender, ToggledEventArgs e)
        {
            Application.Current.Properties[FirebaseEnabledKey] = e.Value;
        }
    }
}
