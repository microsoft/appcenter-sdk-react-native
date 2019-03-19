// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Foundation;
using Microsoft.AppCenter.Distribute.iOS.Bindings;

namespace Microsoft.AppCenter.Distribute
{
    public static partial class Distribute
    {
        static Type _internalBindingType = typeof(MSDistribute);

        [Preserve]
        public static Type BindingType
        {
            get
            {
                return _internalBindingType;
            }
        }

        static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(MSDistribute.IsEnabled());
        }

        static Task PlatformSetEnabledAsync(bool enabled)
        {
            MSDistribute.SetEnabled(enabled);
            return Task.FromResult(default(object));
        }

        static void PlatformSetInstallUrl(string installUrl)
        {
            MSDistribute.SetInstallUrl(installUrl);
        }

        static void PlatformSetApiUrl(string apiUrl)
        {
            MSDistribute.SetApiUrl(apiUrl);
        }

        /// <summary>
        /// Process URL request for the Distribute service.
        /// Place this method call into app delegate openUrl method.
        /// </summary>
        /// <param name="url">The url with parameters.</param>
        public static void OpenUrl(NSUrl url)
        {
            MSDistribute.OpenUrl(url);
        }

        /// <summary>
        /// Do not check for updates in case the app is launched with a debug configuration.
        /// In case you want to use in-app updated, place this method call into your
        /// app delegate's FinishedLaunching method BEFORE you call AppCenter.Start(...)
        /// or before you init the forms application object if you use Xamarin Forms.
        /// </summary>
        /// <remarks>
        /// This method is required because the SDK cannot detect an attached debugger, nor can it detect
        /// a release configuration at runtime. If this method is not called, the browser will appear and try to
        /// setup in-app updates.
        /// </remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DontCheckForUpdatesInDebug()
        {
            _internalBindingType = null;
        }

        static Delegate _delegate;

        static ReleaseAvailableCallback _releaseAvailableCallback;

        static void SetReleaseAvailableCallback(ReleaseAvailableCallback releaseAvailableCallback)
        {
            lock (typeof(Distribute))
            {
                _releaseAvailableCallback = releaseAvailableCallback;
                if (_delegate == null && _releaseAvailableCallback != null)
                {
                    _delegate = new Delegate();
                    MSDistribute.SetDelegate(_delegate);
                }
            }
        }

        static void HandleUpdateAction(UpdateAction updateAction)
        {
            switch (updateAction)
            {
                case UpdateAction.Update:
                    MSDistribute.NotifyUpdateAction(MSUpdateAction.Update);
                    break;

                case UpdateAction.Postpone:
                    MSDistribute.NotifyUpdateAction(MSUpdateAction.Postpone);
                    break;
            }
        }

        public class Delegate : MSDistributeDelegate
        {
            public override bool OnReleaseAvailable(MSDistribute distribute, MSReleaseDetails details)
            {
                if (_releaseAvailableCallback != null)
                {
                    Uri releaseNotesUrl = null;
                    if (details.ReleaseNotesUrl != null)
                    {
                        releaseNotesUrl = new Uri(details.ReleaseNotesUrl.ToString());
                    }
                    var releaseDetails = new ReleaseDetails
                    {
                        Id = details.Id,
                        ShortVersion = details.ShortVersion,
                        Version = details.Version,
                        ReleaseNotes = details.ReleaseNotes,
                        ReleaseNotesUrl = releaseNotesUrl,
                        MandatoryUpdate = details.MandatoryUpdate
                    };
                    return _releaseAvailableCallback(releaseDetails);
                }
                return false;
            }
        }
    }
}
