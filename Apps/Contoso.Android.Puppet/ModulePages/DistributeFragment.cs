using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Distribute;

namespace Contoso.Android.Puppet
{
    public class DistributeFragment : PageFragment
    {
        private Switch DistributeEnabledSwitch;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Distribute, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            // Find views.
            DistributeEnabledSwitch = view.FindViewById(Resource.Id.enabled_distribute) as Switch;

            // Subscribe to events.
            DistributeEnabledSwitch.CheckedChange += UpdateEnabled;

            UpdateState();
        }

        protected override async void UpdateState()
        {
            DistributeEnabledSwitch.CheckedChange -= UpdateEnabled;
            DistributeEnabledSwitch.Enabled = true;
            DistributeEnabledSwitch.Checked = await Distribute.IsEnabledAsync();
            DistributeEnabledSwitch.Enabled = await MobileCenter.IsEnabledAsync();
            DistributeEnabledSwitch.CheckedChange += UpdateEnabled;
        }

        private async void UpdateEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Distribute.SetEnabled(e.IsChecked);
            DistributeEnabledSwitch.Checked = await Distribute.IsEnabledAsync();
        }
    }
}
