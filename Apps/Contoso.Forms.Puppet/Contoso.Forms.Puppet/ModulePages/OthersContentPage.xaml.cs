using Microsoft.AAppCenter
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
                FirebaseAnalyticsEnabledSwitchCell.IsEnabled = false;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var mcEnabled = await AppCenter.IsEnabledAsync();
            DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitchCell.IsEnabled = mcEnabled;
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
            PushEnabledSwitchCell.IsEnabled = mcEnabled;
            if (XamarinDevice.RuntimePlatform == XamarinDevice.Android)
            {
                if (!Application.Current.Properties.ContainsKey(FirebaseEnabledKey))
                {
                    Application.Current.Properties[FirebaseEnabledKey] = false;
                }
                FirebaseAnalyticsEnabledSwitchCell.On = (bool)Application.Current.Properties[FirebaseEnabledKey];
            }
            RumEnabledSwitchCell.On = await RealUserMeasurements.IsEnabledAsync();
            RumEnabledSwitchCell.IsEnabled = mcEnabled;
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

        async void UpdateRumEnabled(object sender, ToggledEventArgs e)
        {
            await RealUserMeasurements.SetEnabledAsync(e.Value);
        }
    }
}
