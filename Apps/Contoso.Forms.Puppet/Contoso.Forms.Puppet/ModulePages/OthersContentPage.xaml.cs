using Microsoft.AppCenter;
using Microsoft.AppCenter.Distribute;
using Microsoft.AppCenter.Push;
using Microsoft.AppCenter.Rum;
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

                // Note: Doesn't work on UWP, see https://bugzilla.xamarin.com/show_bug.cgi?id=42189
                FirebaseAnalyticsEnabledSwitchCell.IsEnabled = false;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var acEnabled = await AppCenter.IsEnabledAsync();
            DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitchCell.IsEnabled = acEnabled;
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
            PushEnabledSwitchCell.IsEnabled = acEnabled;
            if (XamarinDevice.RuntimePlatform == XamarinDevice.Android)
            {
                if (!Application.Current.Properties.ContainsKey(FirebaseEnabledKey))
                {
                    Application.Current.Properties[FirebaseEnabledKey] = false;
                }
                FirebaseAnalyticsEnabledSwitchCell.On = (bool)Application.Current.Properties[FirebaseEnabledKey];
            }
            RumEnabledSwitchCell.On = await RealUserMeasurements.IsEnabledAsync();
            RumEnabledSwitchCell.IsEnabled = acEnabled;
        }

        async void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.Value);
        }

        async void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
            await Push.SetEnabledAsync(e.Value);
        }

        async void UpdateFirebaseAnalyticsEnabled(object sender, ToggledEventArgs e)
        {
            Application.Current.Properties[FirebaseEnabledKey] = e.Value;
            await Application.Current.SavePropertiesAsync();
        }

        async void UpdateRumEnabled(object sender, ToggledEventArgs e)
        {
            await RealUserMeasurements.SetEnabledAsync(e.Value);
        }
    }
}
