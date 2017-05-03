using Android.Support.V4.App;

namespace Contoso.Android.Puppet
{
    public class PagerAdapter : FragmentStatePagerAdapter
    {
        public PagerAdapter(FragmentManager fragmentManager) : base(fragmentManager)
        {
        }

        public override int Count
        {
            get { return 4; }
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return new MobileCenterFragment();
                case 1: return new AnalyticsFragment();
                case 2: return new CrashesFragment();
                case 3: return new DistributeFragment();
                default: return null;
            }
        }
    }
}
