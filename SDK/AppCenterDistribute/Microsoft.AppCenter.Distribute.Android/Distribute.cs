// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Com.Microsoft.Appcenter.Distribute;

namespace Microsoft.AppCenter.Distribute
{
    public static partial class Distribute
    {
        [Preserve]
        public static Type BindingType => typeof(AndroidDistribute);

        static Task<bool> PlatformIsEnabledAsync()
        {
            var future = AndroidDistribute.IsEnabled();
            return Task.Run(() => (bool)future.Get());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            var future = AndroidDistribute.SetEnabled(enabled);
            return Task.Run(() => future.Get());
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

        /// <summary>
        /// Set whether the distribute service can be used within a debuggable build.
        /// </summary>
        /// <param name="enabled"><c>true</c> to enable, <c>false</c> to disable (the initial default value is <c>false</c>).</param>
        public static void SetEnabledForDebuggableBuild(bool enabled)
        {
            AndroidDistribute.SetEnabledForDebuggableBuild(enabled);
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
