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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MobileCenterEnabledSwitchCell.On = await MobileCenter.IsEnabledAsync();
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await MobileCenter.SetEnabledAsync(e.Value);
        }
    }
}
