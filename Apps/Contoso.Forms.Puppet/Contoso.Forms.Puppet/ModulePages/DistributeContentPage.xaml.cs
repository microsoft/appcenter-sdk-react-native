using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Distribute;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class DistributeContentPage
    {
        public DistributeContentPage()
        {
            InitializeComponent();
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "socket.png";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DistributeEnabledSwitchCell.On = Distribute.Enabled;
            DistributeEnabledSwitchCell.IsEnabled = MobileCenter.Enabled;
        }

        void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            Distribute.Enabled = e.Value;
        }
    }
}
