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

        private bool mAutomaticProcessing = true;

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
            return Task.FromResult(false);
        }

        private static Task<ErrorReport> PlatformGetLastSessionCrashReportAsync()
        {
            return Task.FromResult((ErrorReport)null);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
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
            return Task.Run(async () =>
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
                await SendCrashReportsOrAwaitUserConfirmationAsync();
            });
        }

        private Task SendCrashReportsOrAwaitUserConfirmationAsync()
        {
            bool alwaysSend = true; // TODO: change to get from preferences

            if (_unprocessedManagedErrorLogs.Count() > 0)
            {
                // Check for always send: this bypasses user confirmation callback.
                if (alwaysSend)
                {
                    AppCenterLog.Debug(LogTag, "The flag for user confirmation is set to AlwaysSend, will send logs.");
                    return HandleUserConfirmationAsync(UserConfirmation.Send);
                }

                // TODO: mAutomaticProcessing doesn't currently exist on Windows
                // If automatic processing is disabled, don't call the listener.
                //if (!mAutomaticProcessing)
                //{
                //    AppCenterLog.Debug(LogTag, "Automatic processing disabled, will wait for explicit user confirmation.");
                //    return Task.WhenAll(new Task(null)); // TODO: What to do when returning empty
                //}

                if (!ShouldAwaitUserConfirmation())
                {
                    AppCenterLog.Debug(LogTag, "ShouldAwaitUserConfirmation returned false, will send logs.");
                    return HandleUserConfirmationAsync(UserConfirmation.Send);
                }
                else
                {
                    AppCenterLog.Debug(LogTag, "ShouldAwaitUserConfirmation returned false, wait sending logs.");
                }
            }

            return HandleUserConfirmationAsync(UserConfirmation.Send); // TODO: Android returns a boolean (alwaysSend), and HandleUserCOnfirmation may or may not run
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
                }
            }
            else
            {
                if (userConfirmation == UserConfirmation.AlwaysSend)
                {
                    // TODO: Save the AlwaysSend preference
                }

                // Send every pending log.
                foreach (var key in keys)
                {
                    var log = _unprocessedManagedErrorLogs[key];
                    tasks.Add(Channel.EnqueueAsync(log));
                    _unprocessedManagedErrorLogs.Remove(key);
                    ErrorLogHelper.RemoveStoredErrorLogFile(key);
                    if (GetErrorAttachments != null)
                    {
                        var errorReport = new ErrorReport(log, null);

                        // This must never called while a lock is held.
                        var attachments = GetErrorAttachments?.Invoke(errorReport);
                        tasks.Add(SendErrorAttachmentsAsync(log.Id, attachments));
                    }
                }
            }
            return Task.WhenAll(tasks);
        }

        private Task SendErrorAttachmentsAsync(Guid errorId, IEnumerable<ErrorAttachmentLog> attachments)
        {
            if (attachments == null)
            {
                AppCenterLog.Debug(LogTag, $"Crashes.GetErrorAttachments returned null; no additional information will be attached to log: {errorId}.");
                return Task.FromResult(0);
            }
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
