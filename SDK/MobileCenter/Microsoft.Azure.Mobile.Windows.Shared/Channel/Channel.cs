using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Utils;
using Microsoft.Azure.Mobile.Utils.Synchronization;

namespace Microsoft.Azure.Mobile.Channel
{
    public sealed class Channel : IChannelUnit
    {
        private const int ClearBatchSize = 100;
        private Ingestion.Models.Device _device;
        private readonly string _appSecret;
        private readonly IStorage _storage;
        private readonly IIngestion _ingestion;
        private readonly IDeviceInformationHelper _deviceInfoHelper = new DeviceInformationHelper();
        private readonly Dictionary<string, List<Log>> _sendingBatches = new Dictionary<string, List<Log>>();
        private readonly int _maxParallelBatches;
        private readonly int _maxLogsPerBatch;
        private long _pendingLogCount;
        private bool _enabled;
        private bool _discardLogs;
        private bool _batchScheduled;
        private TimeSpan _batchTimeInterval;
        private readonly StatefulMutex _mutex = new StatefulMutex();

        internal Channel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches,
            string appSecret, IIngestion ingestion, IStorage storage)
        {
            Name = name;
            _maxParallelBatches = maxParallelBatches;
            _maxLogsPerBatch = maxLogsPerBatch;
            _appSecret = appSecret;
            _ingestion = ingestion;
            _storage = storage;
            _batchTimeInterval = batchTimeInterval;
            _batchScheduled = false;
            _enabled = true;
            DeviceInformationHelper.InformationInvalidated += (sender, e) => InvalidateDeviceCache();
            var lockHolder = _mutex.GetLock();
            Task.Run(() => _storage.CountLogsAsync(Name)).ContinueWith(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    _pendingLogCount = task.Result;
                }
                lockHolder.Dispose();
            });
        }

        /// <summary>
        /// Gets value indicating whether the Channel is enabled
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                using (_mutex.GetLock())
                {
                    return _enabled;
                }
            }
        }

        /// <summary>
        /// The channel's name
        /// </summary>
        public string Name { get; }

        #region Events
        public event EventHandler<EnqueuingLogEventArgs> EnqueuingLog;
        public event EventHandler<SendingLogEventArgs> SendingLog;
        public event EventHandler<SentLogEventArgs> SentLog;
        public event EventHandler<FailedToSendLogEventArgs> FailedToSendLog;
        #endregion

        public void SetEnabled(bool enabled)
        {
            State state;
            using (_mutex.GetLock())
            {
                if (_enabled == enabled)
                {
                    return;
                }
                state = _mutex.State;
            }
            if (enabled)
            {
                Resume(state);
            }
            else
            {
                Suspend(state, true, new CancellationException());
            }
        }

        public async Task EnqueueAsync(Log log)
        {
            try
            {
                State state;
                bool discardLogs;
                using (await _mutex.GetLockAsync().ConfigureAwait(false))
                {
                    state = _mutex.State;
                    discardLogs = _discardLogs;
                }
                if (discardLogs)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Channel is disabled; logs are discarded");
                    SendingLog?.Invoke(this, new SendingLogEventArgs(log));
                    FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
                }
                EnqueuingLog?.Invoke(this, new EnqueuingLogEventArgs(log));
                await PrepareLogAsync(log, state).ConfigureAwait(false);
                await PersistLogAsync(log, state).ConfigureAwait(false);
            }
            catch (StatefulMutexException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "The Enqueue operation has been cancelled");
            }
        }

        private async Task PrepareLogAsync(Log log, State state)
        {
            if (log.Device == null && _device == null)
            {
                var device = await _deviceInfoHelper.GetDeviceInformationAsync().ConfigureAwait(false);
                using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
                {
                    _device = device;
                }
            }
            log.Device = log.Device ?? _device;
            log.Timestamp = log.Timestamp ?? DateTime.Now;
        }

        private async Task PersistLogAsync(Log log, State state)
        {
            try
            {
                await _storage.PutLog(Name, log).ConfigureAwait(false);
            }
            catch (StorageException e)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Error persisting log", e);
                return;
            }
            try
            {
                bool enabled;
                using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
                {
                    _pendingLogCount++;
                    enabled = _enabled;
                }
                if (enabled)
                {
                    CheckPendingLogs(state);
                    return;
                }
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Channel is temporarily disabled; log was saved to disk");
            }
            catch (StatefulMutexException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "The PersistLog operation has been cancelled");
            }
        }

        public void InvalidateDeviceCache()
        {
            using (_mutex.GetLock())
            {
                _device = null;
            }
        }
        
        public async Task ClearAsync()
        {
            var state = _mutex.State;
            await _storage.DeleteLogs(Name).ConfigureAwait(false);
            try
            {
                using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
                {
                    _pendingLogCount = 0;
                }
            }
            catch (StatefulMutexException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "The Clear operation has been cancelled");
            }
        }

        private void Resume(State state)
        {
            try
            {
                using (_mutex.GetLock(state))
                {
                    _enabled = true;
                    _discardLogs = false;
                    state = _mutex.InvalidateState();
                }
            }
            catch (StatefulMutexException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "The Resume operation has been cancelled");
            }
            CheckPendingLogs(state);
        }

        private void Suspend(State state, bool deleteLogs, Exception exception)
        {
            try
            {
                IEnumerable<Log> unsentLogs = null;
                using (_mutex.GetLock(state))
                {
                    _enabled = false;
                    _batchScheduled = false;
                    _discardLogs = deleteLogs;
                    if (deleteLogs)
                    {
                        unsentLogs = _sendingBatches.Values.SelectMany(batch => batch);
                        _sendingBatches.Clear();
                    }
                    state = _mutex.InvalidateState();
                }
                if (unsentLogs  != null)
                {
                    foreach (var log in unsentLogs)
                    {
                        FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, exception));
                    }
                }
                if (deleteLogs)
                {
                    try
                    {
                        _ingestion.Close();
                    }
                    catch (IngestionException e)
                    {
                        MobileCenterLog.Error(MobileCenterLog.LogTag, "Failed to close ingestion", e);
                    }
                    using (_mutex.GetLock(state))
                    {
                        _pendingLogCount = 0;
                        TriggerDeleteLogsOnSuspending();
                    }
                }
                _storage.ClearPendingLogState(Name);
            }
            catch (StatefulMutexException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "The Suspend operation has been cancelled");
            }
        }

        private void TriggerDeleteLogsOnSuspending()
        {
            if (SendingLog == null && FailedToSendLog == null)
            {
                _storage.DeleteLogs(Name);
                return;
            }
            SignalDeletingLogs().ContinueWith(completedTask => _storage.DeleteLogs(Name));
        }

        private Task SignalDeletingLogs()
        {
            var logs = new List<Log>();
            return _storage.GetLogsAsync(Name, ClearBatchSize, logs)
                .ContinueWith(completedTask =>
                {
                    if (completedTask.IsFaulted)
                    {
                        MobileCenterLog.Warn(MobileCenterLog.LogTag,
                            "Failed to invoke events for logs being deleted.");
                        return;
                    }
                    foreach (var log in logs)
                    {
                        SendingLog?.Invoke(this, new SendingLogEventArgs(log));
                        FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
                    }
                    if (logs.Count >= ClearBatchSize)
                    {
                        SignalDeletingLogs();
                    }
                });
        }

        private async Task TriggerIngestionAsync(State state)
        {
            using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
            {
                if (!_enabled || !_batchScheduled)
                {
                    return;
                }
                MobileCenterLog.Debug(MobileCenterLog.LogTag,
                    $"triggerIngestion({Name}) pendingLogCount={_pendingLogCount}");
                _batchScheduled = false;
                if (_sendingBatches.Count >= _maxParallelBatches)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag,
                        "Already sending " + _maxParallelBatches + " batches of analytics data to the server");
                    return;
                }
            }

            // Get a batch from storage
            var logs = new List<Log>();
            var batchId = await _storage.GetLogsAsync(Name, _maxLogsPerBatch, logs).ConfigureAwait(false);
            if (batchId != null)
            {
                using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
                {
                    _sendingBatches.Add(batchId, logs);
                    _pendingLogCount -= logs.Count;
                }
                try
                {
                    TriggerIngestion(state, logs, batchId);
                    CheckPendingLogs(state);
                }
                catch (StorageException)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Something went wrong sending logs to ingestion");
                }
            }
        }

        private void TriggerIngestion(State state, IList<Log> logs, string batchId)
        {
            // Before sending logs, trigger the sending event for this channel
            if (SendingLog != null)
            {
                foreach (var eventArgs in logs.Select(log => new SendingLogEventArgs(log)))
                {
                    SendingLog?.Invoke(this, eventArgs);
                }
            }

            // If the optional Install ID has no value, default to using empty GUID
            var rawInstallId = await MobileCenter.GetInstallIdAsync().ConfigureAwait(false);
            var installId = rawInstallId.HasValue ? rawInstallId.Value : Guid.Empty;
            using (var serviceCall = _ingestion.PrepareServiceCall(_appSecret, installId, logs))
            {
                serviceCall.ExecuteAsync().ContinueWith(completedTask =>
                {
                    var ingestionException = completedTask.Exception?.InnerException as IngestionException;
                    if (ingestionException == null)
                    {
                        HandleSendingSuccess(state, batchId);
                    }
                    else
                    {
                        HandleSendingFailure(state, batchId, ingestionException);
                    }
                });
            }
        }

        private void HandleSendingSuccess(State state, string batchId)
        {
            if (!_mutex.IsCurrent(state))
            {
                return;
            }
            try
            {
                _storage.DeleteLogs(Name, batchId);
            }
            catch (StorageException e)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Could not delete logs for batch {batchId}", e);
                throw;
            }
            finally
            {
                List<Log> removedLogs;
                using (_mutex.GetLock(state))
                {
                    removedLogs = _sendingBatches[batchId];
                    _sendingBatches.Remove(batchId);
                }
                if (SentLog != null)
                {
                    foreach (var log in removedLogs)
                    {
                        SentLog?.Invoke(this, new SentLogEventArgs(log));
                    }
                }
            }
        }

        private void HandleSendingFailure(State state, string batchId, IngestionException e)
        {
            var isRecoverable = e?.IsRecoverable ?? false;
            MobileCenterLog.Error(MobileCenterLog.LogTag, $"Sending logs for channel '{Name}', batch '{batchId}' failed: {e?.Message}");
            List<Log> removedLogs;
            using (_mutex.GetLock(state))
            {
                removedLogs = _sendingBatches[batchId];
                _sendingBatches.Remove(batchId);
                if (isRecoverable)
                {
                    _pendingLogCount += removedLogs.Count;
                }
            }
            if (!isRecoverable && FailedToSendLog != null)
            {
                foreach (var log in removedLogs)
                {
                    FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, e));
                }
            }
            Suspend(state, !isRecoverable, e);
        }

        private void CheckPendingLogs(State state)
        {
            if (!_enabled)
            {
                MobileCenterLog.Info(MobileCenterLog.LogTag, "The service has been disabled. Stop processing logs.");
                return;
            }

            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"CheckPendingLogs({Name}) pending log count: {_pendingLogCount}");
            using (_mutex.GetLock())
            {
                if (_pendingLogCount >= _maxLogsPerBatch)
                {
                    _batchScheduled = true;
                    Task.Run(async () =>
                    {
                        await TriggerIngestionAsync(state).ConfigureAwait(false);
                    });
                }
                else if (_pendingLogCount > 0 && !_batchScheduled)
                {
                    _batchScheduled = true;
                    
                    // No need wait _batchTimeInterval here.
                    Task.Run(async () =>
                    {
                        await Task.Delay((int) _batchTimeInterval.TotalMilliseconds).ConfigureAwait(false);
                        if (_batchScheduled)
                        {
                            await TriggerIngestionAsync(_mutex.State).ConfigureAwait(false);
                        }
                    });
                }
            }
        }

        public Task ShutdownAsync()
        {
            Suspend(_mutex.State, false, new CancellationException());

            // Nothing to wait on; just suspend and return a task
            return Task.FromResult(default(object));
        }

        public void Dispose()
        {
            _mutex.Dispose();
        }
    }
}
