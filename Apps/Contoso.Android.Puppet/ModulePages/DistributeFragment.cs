using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Distribute;

namespace Contoso.Android.Puppet
{
    public class DistributeFragment : Fragment
    {
        private Switch DistributeEnabledSwitch;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Distribute, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            DistributeEnabledSwitch = view.FindViewById(Resource.Id.enabled) as Switch;

            DistributeEnabledSwitch.CheckedChange += UpdateEnabled;

            UpdateState();
        }

        public override bool UserVisibleHint
        {
            get { return base.UserVisibleHint; }
            set
            {
                base.UserVisibleHint = value;
                if (value && IsResumed) UpdateState();
            }
        }

        private void UpdateState()
		{
			DistributeEnabledSwitch.Enabled = MobileCenter.Enabled;
			DistributeEnabledSwitch.Checked = Distribute.Enabled;
        }

        private void UpdateEnabled(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
            Distribute.Enabled = e.IsChecked;
            DistributeEnabledSwitch.Checked = Distribute.Enabled;
        }
    }
}
