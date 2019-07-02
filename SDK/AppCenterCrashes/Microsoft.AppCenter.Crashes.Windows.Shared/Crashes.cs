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
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter.Crashes
{
    public partial class Crashes : AppCenterService
    {
        private static readonly object CrashesLock = new object();

        private static Crashes _instanceField;

        private const int MaxAttachmentsPerCrash = 2;

        internal const string PrefKeyAlwaysSend = Constants.KeyPrefix + "CrashesAlwaysSend";

        static Crashes()
        {
            LogSerializer.AddLogType(ManagedErrorLog.JsonIdentifier, typeof(ManagedErrorLog));
            LogSerializer.AddLogType(ErrorAttachmentLog.JsonIdentifier, typeof(ErrorAttachmentLog));
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
            return Instance.InstanceHasCrashedInLastSessionAsync();
        }

        private static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            return Instance.InstanceGetLastSessionCrashReportAsync();
        }

        private static void PlatformNotifyUserConfirmation(UserConfirmation userConfirmation)
        {
            Instance.HandleUserConfirmationAsync(userConfirmation);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void PlatformTrackError(System.Exception exception, IDictionary<string, string> properties)
        {
        }

        /// <summary>
        /// A dictionary that contains unprocessed managed error logs before getting a user confirmation.
        /// </summary>
        private Dictionary<Guid, ManagedErrorLog> _unprocessedManagedErrorLogs;

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

        // Task to get the last session error report, if one is found.
        private TaskCompletionSource<ErrorReport> _lastSessionErrorReportTaskSource;

        internal Crashes()
        {
            _unprocessedManagedErrorLogs = new Dictionary<Guid, ManagedErrorLog>();
        }

        /// <summary>
        /// Method that is called to signal start of Crashes service.
        /// </summary>
        /// <param name="channelGroup">Channel group</param>
        /// <param name="appSecret">App secret</param>
        public override void OnChannelGroupReady(IChannelGroup channelGroup, string appSecret)
        {
            lock (_serviceLock)
            {
                base.OnChannelGroupReady(channelGroup, appSecret);
                ApplyEnabledState(InstanceEnabled);
                if (InstanceEnabled)
                {
                    _lastSessionErrorReportTaskSource = new TaskCompletionSource<ErrorReport>();
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
                    _lastSessionErrorReportTaskSource = null;
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

        private async Task<bool> InstanceHasCrashedInLastSessionAsync()
        {
            return (await InstanceGetLastSessionCrashReportAsync()) != null;
        }

        private Task<ErrorReport> InstanceGetLastSessionCrashReportAsync()
        {
            return _lastSessionErrorReportTaskSource?.Task ?? Task.FromResult<ErrorReport>(null);
        }

        private Task ProcessPendingErrorsAsync()
        {
            return Task.Run(async () =>
            {
                var lastSessionErrorLogTimestamp = DateTime.MinValue;
                ManagedErrorLog lastSessionErrorLog = null;
                foreach (var file in ErrorLogHelper.GetErrorLogFiles())
                {
                    AppCenterLog.Debug(LogTag, $"Process pending error file {file.Name}");
                    var log = ErrorLogHelper.ReadErrorLogFile(file);

                    // Process the file for last session crash report. It doesn't matter if the log is null.
                    try
                    {
                        var otherFileTimestamp = file.LastWriteTime;
                        if (lastSessionErrorLogTimestamp < otherFileTimestamp)
                        {
                            lastSessionErrorLogTimestamp = otherFileTimestamp;
                            lastSessionErrorLog = log;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        AppCenterLog.Warn(LogTag, $"Failed to get the last write time for an error file.", ex);
                    }

                    // Finish processing the file.
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
                ErrorReport lastSessionErrorReport = null;
                if (lastSessionErrorLog != null)
                {
                    AppCenterLog.Debug(LogTag, "Setting last session error report to an actual report.");

                    // TODO: Build error report from cache.
                    lastSessionErrorReport = new ErrorReport(lastSessionErrorLog, null);
                }
                else
                {
                    AppCenterLog.Debug(LogTag, "Setting last session error report to null.");
                }
                _lastSessionErrorReportTaskSource.SetResult(lastSessionErrorReport);
                await SendCrashReportsOrAwaitUserConfirmationAsync().ConfigureAwait(false);
            });
        }

        private async Task SendCrashReportsOrAwaitUserConfirmationAsync()
        {
            bool alwaysSend = ApplicationSettings.GetValue(PrefKeyAlwaysSend, false);
            if (_unprocessedManagedErrorLogs.Count() > 0)
            {
                // Check for always send: this bypasses user confirmation callback.
                if (alwaysSend)
                {
                    AppCenterLog.Debug(LogTag, "The flag for user confirmation is set to AlwaysSend, will send logs.");
                    await HandleUserConfirmationAsync(UserConfirmation.Send);
                    return;
                }

                var shouldAwaitUserConfirmation = ShouldAwaitUserConfirmation?.Invoke();
                if (shouldAwaitUserConfirmation.HasValue && shouldAwaitUserConfirmation.Value)
                {
                    AppCenterLog.Debug(LogTag, "ShouldAwaitUserConfirmation returned true, wait sending logs.");
                }
                else
                {
                    AppCenterLog.Debug(LogTag, "ShouldAwaitUserConfirmation returned false or is not implemented, will send logs.");
                    await HandleUserConfirmationAsync(UserConfirmation.Send);
                }
            }
        }

        private Task HandleUserConfirmationAsync(UserConfirmation userConfirmation)
        {
            var keys = _unprocessedManagedErrorLogs.Keys.ToList();
            var tasks = new List<Task>();

            if (userConfirmation == UserConfirmation.DontSend)
            {
                foreach (var key in keys)
                {
                    _unprocessedManagedErrorLogs.Remove(key);
                    ErrorLogHelper.RemoveStoredErrorLogFile(key);
                    // TODO: Remove exception files
                }
            }
            else
            {
                if (userConfirmation == UserConfirmation.AlwaysSend)
                {
                    ApplicationSettings.SetValue(PrefKeyAlwaysSend, true);
                }

                // Send every pending log.
                foreach (var key in keys)
                {
                    var log = _unprocessedManagedErrorLogs[key];
                    tasks.Add(Channel.EnqueueAsync(log));
                    _unprocessedManagedErrorLogs.Remove(key);
                    ErrorLogHelper.RemoveStoredErrorLogFile(key);

                    // TODO: Build error report from cache.
                    var errorReport = new ErrorReport(log, null);

                    // This must never be called while a lock is held.
                    var attachments = GetErrorAttachments?.Invoke(errorReport);
                    if (attachments == null)
                    {
                        AppCenterLog.Debug(LogTag, $"Crashes.GetErrorAttachments returned null; no additional information will be attached to log: {log.Id}.");
                    }
                    else
                    {
                        tasks.Add(SendErrorAttachmentsAsync(log.Id, attachments));
                    }
                }
            }
            return Task.WhenAll(tasks);
        }

        private Task SendErrorAttachmentsAsync(Guid errorId, IEnumerable<ErrorAttachmentLog> attachments)
        {
            var totalErrorAttachments = 0;
            var tasks = new List<Task>();
            foreach (var attachment in attachments)
            {
                if (attachment != null)
                {
                    attachment.Id = Guid.NewGuid();
                    attachment.ErrorId = errorId;
                    try
                    {
                        attachment.Validate();
                        ++totalErrorAttachments;
                        tasks.Add(Channel.EnqueueAsync(attachment));
                    }
                    catch (ValidationException e)
                    {
                        AppCenterLog.Error(LogTag, "Not all required fields are present in ErrorAttachmentLog.", e);
                    }
                }
                else
                {
                    AppCenterLog.Warn(LogTag, "Skipping null ErrorAttachmentLog in Crashes.GetErrorAttachments.");
                }
            }
            if (totalErrorAttachments > MaxAttachmentsPerCrash)
            {
                AppCenterLog.Warn(LogTag, $"A limit of {MaxAttachmentsPerCrash} attachments per error report might be enforced by server.");
            }
            return Task.WhenAll(tasks);
        }
    }
}
