using System;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Com.Microsoft.Azure.Mobile.Distribute;

namespace Microsoft.Azure.Mobile.Distribute
{
    public static partial class Distribute
    {
        [Preserve]
        public static Type BindingType => typeof(AndroidDistribute);

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.Run(() => (bool)AndroidDistribute.IsEnabled().Get());
        }

        static void PlatformSetEnabled(bool enabled)
        {
            AndroidDistribute.SetEnabled(enabled);
        }

        static void PlatformSetInstallUrl(string installUrl)
        {
            AndroidDistribute.SetInstallUrl(installUrl);
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
            AndroidDistribute.SetApiUrl(apiUrl);
        }

        static void HandleUpdateAction(UpdateAction updateAction)
        {
            /* Xamarin does not bind interface integers, cannot use AndroidUpdateAction */
            switch (updateAction)
            {
                case UpdateAction.Update:
                    AndroidDistribute.NotifyUpdateAction(-1);
                    break;

                case UpdateAction.Postpone:
                    AndroidDistribute.NotifyUpdateAction(-2);
                    break;
            }
        }

        static Listener _listener;

        static ReleaseAvailableCallback _releaseAvailableCallback;

        static void SetReleaseAvailableCallback(ReleaseAvailableCallback releaseAvailableCallback)
        {
            lock (typeof(Distribute))
            {
                _releaseAvailableCallback = releaseAvailableCallback;
                if (_listener == null && _releaseAvailableCallback != null)
                {
                    _listener = new Listener();
                    AndroidDistribute.SetListener(_listener);
                }
            }
        }

        class Listener : Java.Lang.Object, IDistributeListener
        {
            ReleaseAvailableCallback releaseAvailableCallback;

            public bool OnReleaseAvailable(Activity activity, AndroidReleaseDetails androidReleaseDetails)
            {
                if (_releaseAvailableCallback != null)
                {
                    Uri releaseNotesUrl = null;
                    if (androidReleaseDetails.ReleaseNotesUrl != null)
                    {
                        releaseNotesUrl = new Uri(androidReleaseDetails.ReleaseNotesUrl.ToString());
                    }
                    var releaseDetails = new ReleaseDetails
                    {
                        Id = androidReleaseDetails.Id,
                        ShortVersion = androidReleaseDetails.ShortVersion,
                        Version = androidReleaseDetails.Version.ToString(),
                        ReleaseNotes = androidReleaseDetails.ReleaseNotes,
                        ReleaseNotesUrl = releaseNotesUrl,
                        MandatoryUpdate = androidReleaseDetails.IsMandatoryUpdate
                    };
                    return _releaseAvailableCallback(releaseDetails);
                }
                return false;
            }
        }
    }
}
