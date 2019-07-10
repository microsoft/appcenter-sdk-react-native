// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Channel;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Crashes.Utils;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Windows.Shared.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

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
            LogSerializer.AddLogType(HandledErrorLog.JsonIdentifier, typeof(HandledErrorLog));
        }

        /// <summary>
        /// Unique instance.
        /// </summary>
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
            ErrorLogHelper.SaveErrorLogFiles(args.Exception, errorLog);
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

        private static void PlatformTrackError(System.Exception exception, IDictionary<string, string> properties)
        {
            Instance.InstanceTrackError(exception, properties);
        }

        /// <summary>
        /// A dictionary that contains unprocessed managed error logs before getting a user confirmation.
        /// </summary>
        internal readonly IDictionary<Guid, ManagedErrorLog> _unprocessedManagedErrorLogs;

        /// <summary>
        /// A dictionary for a cache that contains error report.
        /// </summary>
        private readonly IDictionary<Guid, ErrorReport> _errorReportCache;

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
            _errorReportCache = new ConcurrentDictionary<Guid, ErrorReport>();
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
                    ChannelGroup.SendingLog += ChannelSendingLog;
                    ChannelGroup.SentLog += ChannelSentLog;
                    ChannelGroup.FailedToSendLog += ChannelFailedToSendLog;
                }
                else if (!enabled)
                {
                    ApplicationLifecycleHelper.Instance.UnhandledExceptionOccurred -= OnUnhandledExceptionOccurred;
                    if (ChannelGroup != null)
                    {
                        ChannelGroup.SendingLog -= ChannelSendingLog;
                        ChannelGroup.SentLog -= ChannelSentLog;
                        ChannelGroup.FailedToSendLog -= ChannelFailedToSendLog;
                    }
                    ErrorLogHelper.RemoveAllStoredErrorLogFiles();
                    _unprocessedManagedErrorLogs.Clear();
                    _lastSessionErrorReportTaskSource = null;
                }
            }
        }

        /// <inheritdoc />
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
                ErrorReport lastSessionErrorReport = null;
                foreach (var file in ErrorLogHelper.GetErrorLogFiles())
                {
                    AppCenterLog.Debug(LogTag, $"Process pending error file {file.Name}");
                    var log = ErrorLogHelper.ReadErrorLogFile(file);

                    // Delete file if corrupted.
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

                        // Also try to delete paired exception file.
                        if (Guid.TryParse(System.IO.Path.GetFileNameWithoutExtension(file.Name), out var id))
                        {
                            ErrorLogHelper.RemoveStoredExceptionFile(id);
                        }
                        continue;
                    }

                    // The error report cannot be built if any of those fields are null.
                    if (log.Device == null || log.AppLaunchTimestamp == null || log.Timestamp == null)
                    {
                        AppCenterLog.Error(LogTag, $"Error parsing error log. Deleting invalid file: {file.Name}");
                        RemoveAllStoredErrorLogFiles(log.Id);
                        continue;
                    }

                    // Build and record error report.
                    var report = BuildErrorReport(log);
                    if (lastSessionErrorReport == null || lastSessionErrorReport.AppErrorTime < report.AppErrorTime)
                    {
                        lastSessionErrorReport = report;
                    }
                    if (ShouldProcessErrorReport?.Invoke(report) ?? true)
                    {
                        _unprocessedManagedErrorLogs.Add(log.Id, log);
                    }
                    else
                    {
                        AppCenterLog.Debug(LogTag, $"ShouldProcessErrorReport returned false, clean up and ignore log: {log.Id}");
                        RemoveAllStoredErrorLogFiles(log.Id);
                    }
                }
                _lastSessionErrorReportTaskSource.SetResult(lastSessionErrorReport);
                await SendCrashReportsOrAwaitUserConfirmationAsync().ConfigureAwait(false);
            });
        }

        private void RemoveAllStoredErrorLogFiles(Guid errorId)
        {
            // ReSharper disable once InconsistentlySynchronizedField this is a concurrent dictionary.
            _errorReportCache.Remove(errorId);
            ErrorLogHelper.RemoveStoredErrorLogFile(errorId);
            ErrorLogHelper.RemoveStoredExceptionFile(errorId);
        }

        private async Task SendCrashReportsOrAwaitUserConfirmationAsync()
        {
            var alwaysSend = ApplicationSettings.GetValue(PrefKeyAlwaysSend, false);
            if (_unprocessedManagedErrorLogs.Any())
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
            lock (_serviceLock)
            {
                if (IsInactive)
                {
                    return Task.FromResult(default(object));
                }

                var keys = _unprocessedManagedErrorLogs.Keys.ToList();
                var tasks = new List<Task>();

                if (userConfirmation == UserConfirmation.DontSend)
                {
                    foreach (var key in keys)
                    {
                        _unprocessedManagedErrorLogs.Remove(key);
                        RemoveAllStoredErrorLogFiles(key);
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

                        // Get error report (will be in cache).
                        var errorReport = BuildErrorReport(log);

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
        }

        private void InstanceTrackError(System.Exception exception, IDictionary<string, string> properties)
        {
            lock (_serviceLock)
            {
                if (IsInactive)
                {
                    return;
                }
                properties = PropertyValidator.ValidateProperties(properties, "HandledError");
                var log = new HandledErrorLog(exception: ErrorLogHelper.CreateModelException(exception), properties: properties, id: Guid.NewGuid(), device: null);
                Channel.EnqueueAsync(log);
            }
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
                    if (attachment.ValidatePropertiesForAttachment())
                    {
                        ++totalErrorAttachments;
                        tasks.Add(Channel.EnqueueAsync(attachment));
                    }
                    else
                    {
                        AppCenterLog.Error(LogTag, "Not all required fields are present in ErrorAttachmentLog.");
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

        private ErrorReport BuildErrorReport(ManagedErrorLog log)
        {
            if (_errorReportCache.ContainsKey(log.Id))
            {
                return _errorReportCache[log.Id];
            }
            var file = ErrorLogHelper.GetStoredExceptionFile(log.Id);
            var exception = file == null ? null : ErrorLogHelper.ReadExceptionFile(file);
            var report = new ErrorReport(log, exception);
            _errorReportCache.Add(log.Id, report);
            return report;
        }

        private void ChannelSendingLog(object sender, SendingLogEventArgs e)
        {
            var report = MapLogEventToReportAndDeleteOnLastEvent(e);
            if (report != null)
            {
                SendingErrorReport?.Invoke(sender, new SendingErrorReportEventArgs { Report = report });
            }
        }

        private void ChannelSentLog(object sender, SentLogEventArgs e)
        {
            var report = MapLogEventToReportAndDeleteOnLastEvent(e);
            if (report != null)
            {
                SentErrorReport?.Invoke(sender, new SentErrorReportEventArgs { Report = report });
            }
        }

        private void ChannelFailedToSendLog(object sender, FailedToSendLogEventArgs e)
        {
            var report = MapLogEventToReportAndDeleteOnLastEvent(e);
            if (report != null)
            {
                FailedToSendErrorReport?.Invoke(sender, new FailedToSendErrorReportEventArgs { Report = report, Exception = e.Exception });
            }
        }

        private ErrorReport MapLogEventToReportAndDeleteOnLastEvent(ChannelEventArgs channelEventArgs)
        {
            if (channelEventArgs.Log is ManagedErrorLog log)
            {
                var report = BuildErrorReport(log);
                if (channelEventArgs is SentLogEventArgs || channelEventArgs is FailedToSendLogEventArgs)
                {
                    ErrorLogHelper.RemoveStoredExceptionFile(log.Id);
                }
                return report;
            }
            return null;
        }
    }
}
