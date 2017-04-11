using Microsoft.Azure.Mobile;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class MobileCenterContentPage : ContentPage
    {
        public MobileCenterContentPage()
        {
            InitializeComponent();
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "bolt.png";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MobileCenterEnabledSwitchCell.On = MobileCenter.Enabled;
        }

        void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            MobileCenter.Enabled = e.Value;
        }
    }
}
