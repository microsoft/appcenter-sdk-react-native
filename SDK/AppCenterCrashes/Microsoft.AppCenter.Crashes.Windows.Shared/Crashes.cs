using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
#if REFERENCE
#else
using WatsonRegistrationUtility;
#endif

namespace Microsoft.AppCenter.Crashes
{
    public partial class Crashes : IAppCenterService
    {
        public string ServiceName => "Crashes";

        /// <inheritdoc />
        /// <summary>
        /// This property does not return a meaningful value on Windows.
        /// </summary>
        public bool InstanceEnabled { get; set; }

        private static Crashes _instanceField;

        // ReSharper disable once UnusedMember.Global
        public static Crashes Instance => _instanceField ?? (_instanceField = new Crashes());

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            try
            {
#if REFERENCE
#else
                WatsonRegistrationManager.Start(appSecret);
#pragma warning disable CS0612 // Type or member is obsolete
                AppCenter.CorrelationIdChanged += (s, id) =>
                {
                    WatsonRegistrationManager.SetCorrelationId(id.ToString());
                };

                // Checking for null and setting id needs to be atomic to avoid
                // overwriting
                var newId = Guid.NewGuid();
                AppCenter.TestAndSetCorrelationId(Guid.Empty, ref newId);
#pragma warning restore CS0612 // Type or member is obsolete
#endif
            }
            catch (Exception e)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "Failed to register crashes with Watson", e);
            }
        }

        private static Task<bool> PlatformIsEnabledAsync()
        {
            return Task.FromResult(false);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            return Task.FromResult(default(object));
        }

        private static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            return Task.FromResult(false);
        }

        private static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            return Task.FromResult((ErrorReport)null);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void PlatformTrackError(Exception exception, IDictionary<string, string> properties)
        {
        }
    }
}
