using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace Contoso.Android.Puppet
{
    public class AnalyticsFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Analytics, container, false);
        }
    }
}
