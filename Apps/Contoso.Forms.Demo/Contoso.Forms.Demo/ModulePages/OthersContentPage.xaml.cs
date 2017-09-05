using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Distribute;
using Microsoft.Azure.Mobile.Push;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    using XamarinDevice = Xamarin.Forms.Device;

    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class OthersContentPage
    {
        public OthersContentPage()
        {
            InitializeComponent();
            if (XamarinDevice.RuntimePlatform == XamarinDevice.iOS)
            {
                Icon = "handbag.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            DistributeEnabledSwitchCell.On = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitchCell.IsEnabled = await MobileCenter.IsEnabledAsync();
            PushEnabledSwitchCell.On = await Push.IsEnabledAsync();
        }

        async void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            await Distribute.SetEnabledAsync(e.Value);
        }

        async void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
            await Push.SetEnabledAsync(e.Value);
        }
    }
}
