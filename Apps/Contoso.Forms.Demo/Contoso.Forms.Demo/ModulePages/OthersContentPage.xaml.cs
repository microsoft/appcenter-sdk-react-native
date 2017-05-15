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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DistributeEnabledSwitchCell.On = Distribute.Enabled;
            DistributeEnabledSwitchCell.IsEnabled = MobileCenter.Enabled;
            PushEnabledSwitchCell.On = Push.Enabled;
        }

        void UpdateDistributeEnabled(object sender, ToggledEventArgs e)
        {
            Distribute.Enabled = e.Value;
        }

        void UpdatePushEnabled(object sender, ToggledEventArgs e)
        {
	        Push.Enabled = e.Value;
        }
    }
}
