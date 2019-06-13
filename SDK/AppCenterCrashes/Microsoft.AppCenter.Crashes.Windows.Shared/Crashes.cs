// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;

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

        private static readonly object CrashesLock = new object();

        private static Crashes _instanceField;

        private Crashes()
        {
            LogSerializer.AddLogType(ManagedErrorLog.JsonIdentifier, typeof(ManagedErrorLog));
        }

        public static Crashes Instance
        {
            get
            {
                lock (CrashesLock)
                {
                    return _instanceField ?? (_instanceField = new Crashes());
                }
            }
            set
            {
                lock (CrashesLock)
                {
                    _instanceField = value;
                }
            }
        }

        public void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
        }

        private static Task<bool> PlatformIsEnabledAsync()
        {
            lock (CrashesLock)
            {
                return Task.FromResult(Instance.InstanceEnabled);
            }
        }
        
        private static Task PlatformSetEnabledAsync(bool enabled)
        {
            lock (CrashesLock)
            {
                Instance.InstanceEnabled = enabled;
                if (enabled)
                {
                    ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred += OnUnhandledExceptionOccurred;
                }
                return Task.FromResult(default(object));
            }
        }

        private static void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionOccurredEventArgs args)
        {
            // TODO: errorLog should not wait for screen size
            var errorLog = ErrorLogHelper.CreateErrorLogAsync(args.Exception).Result;
            ErrorLogHelper.SaveErrorLogFiles(args.Exception, errorLog);
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
        private static void PlatformTrackError(System.Exception exception, IDictionary<string, string> properties)
        {
        }
    }
}
