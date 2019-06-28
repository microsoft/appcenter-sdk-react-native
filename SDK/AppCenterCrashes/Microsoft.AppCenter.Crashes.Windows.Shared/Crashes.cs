// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter.Crashes
{
    public partial class Crashes : AppCenterService
    {
        private static readonly object CrashesLock = new object();

        private static Crashes _instanceField;

        static Crashes()
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
                    _instanceField = value; //for testing
                }
            }
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
                return Task.FromResult(default(object));
            }
        }

        private static void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionOccurredEventArgs args)
        {
            var errorLog = ErrorLogHelper.CreateErrorLog(args.Exception);
            ErrorLogHelper.SaveErrorLogFile(errorLog);
        }

        private static Task<bool> PlatformHasCrashedInLastSessionAsync()
        {
            return Task.FromResult(Instance.LastSessionErrorReport != null);
        }

        private static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            return Task.FromResult(Instance.LastSessionErrorReport);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void PlatformNotifyUserConfirmation(UserConfirmation confirmation)
        {
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void PlatformTrackError(System.Exception exception, IDictionary<string, string> properties)
        {
        }

        /// <summary>
        /// A dictionary that contains unprocessed managed error logs before getting a user confirmation.
        /// </summary>
        private Dictionary<Guid, ManagedErrorLog> _unprocessedManagedErrorLogs;

        internal Crashes()
        {
            _unprocessedManagedErrorLogs = new Dictionary<Guid, ManagedErrorLog>();
        }

        /// <inheritdoc />
        protected override string ChannelName => "crashes";

        /// <inheritdoc />
        protected override int TriggerCount => 1;

        /// <inheritdoc />
        protected override TimeSpan TriggerInterval => TimeSpan.FromSeconds(1);

        /// <inheritdoc />
        public override string ServiceName => "Crashes";

        /// <summary>
        /// A task of processing pending error log files.
        /// </summary>
        internal Task ProcessPendingErrorsTask { get; set; }

        /// <summary>
        /// Method that is called to signal start of Crashes service.
        /// </summary>
        /// <param name="channelGroup">Channel group</param>
        /// <param name="appSecret">App secret</param>
        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            lock (CrashesLock)
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(InstanceEnabled);
                if (InstanceEnabled)
                {
                    ProcessPendingErrorsTask = ProcessPendingErrorsAsync();
                }
            }
        }

        private void ApplyEnabledState(bool enabled)
        {
            lock (_serviceLock)
            {
                if (enabled && ChannelGroup != null)
                {
                    ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred += OnUnhandledExceptionOccurred;
                }
                else if (!enabled)
                {
                    ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred -= OnUnhandledExceptionOccurred;
                    ErrorLogHelper.RemoveAllStoredErrorLogFiles();
                }
            }
        }

        public override bool InstanceEnabled
        {
            get => base.InstanceEnabled;

            set
            {
                lock (_serviceLock)
                {
                    var prevValue = InstanceEnabled;
                    base.InstanceEnabled = value;
                    if (value != prevValue)
                    {
                        ApplyEnabledState(value);
                    }
                }
            }
        }

        private Task ProcessPendingErrorsAsync()
        {
            return Task.Run(() =>
            {
                foreach (var file in ErrorLogHelper.GetErrorLogFiles())
                {
                    AppCenterLog.Debug(LogTag, $"Process pending error file {file.Name}");
                    var log = ErrorLogHelper.ReadErrorLogFile(file);
                    if (log == null)
                    {
                        AppCenterLog.Error(LogTag, $"Error parsing error log. Deleting invalid file: {file.Name}");
                        try
                        {
                            file.Delete();
                        }
                        catch (System.Exception ex)
                        {
                            AppCenterLog.Warn(LogTag, $"Failed to delete error log file {file.Name}.", ex);
                        }
                    }
                    else
                    {
                        _unprocessedManagedErrorLogs.Add(log.Id, log);
                    }
                }
                SendCrashReportsOrAwaitUserConfirmation();
            }).ContinueWith((_) => ProcessPendingErrorsTask = null);
        }

        private void SendCrashReportsOrAwaitUserConfirmation()
        {
            HandleUserConfirmation();
        }

        private void HandleUserConfirmation()
        {
            // Send every pending log.
            var keys = _unprocessedManagedErrorLogs.Keys.ToList();
            foreach (var key in keys)
            {
                Channel.EnqueueAsync(_unprocessedManagedErrorLogs[key]);
                _unprocessedManagedErrorLogs.Remove(key);
                ErrorLogHelper.RemoveStoredErrorLogFile(key);
            }
        }
    }
}
