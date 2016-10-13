using System;
using Microsoft.Sonoma.Xamarin.Crashes.Shared;

namespace Microsoft.Sonoma.Xamarin.Crashes
{
    class PlatformCrashes : PlatformCrashesBase
    {
        public override Type BindingType
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool HasCrashedInLastSession
        {
            get { throw new NotImplementedException(); }
        }

        public override void TrackException(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
