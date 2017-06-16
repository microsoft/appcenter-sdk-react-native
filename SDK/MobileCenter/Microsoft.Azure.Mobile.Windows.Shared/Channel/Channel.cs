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

        public async Task SetEnabledAsync(bool enabled)
        {
            State state;
            using (await _mutex.GetLockAsync().ConfigureAwait(false))
            {
                if (_enabled == enabled)
                {
                    return;
                }
                state = _mutex.State;
            }
            if (enabled)
            {
                await ResumeAsync(state).ConfigureAwait(false);
            }
            else
            {
                await SuspendAsync(state, true, new CancellationException()).ConfigureAwait(false);
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
                await _storage.PutLogAsync(Name, log).ConfigureAwait(false);
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
                    await CheckPendingLogsAsync(state).ConfigureAwait(false);
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
            await _storage.DeleteLogsAsync(Name).ConfigureAwait(false);
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

        private async Task ResumeAsync(State state)
        {
            try
            {
                using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
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
            await CheckPendingLogsAsync(state).ConfigureAwait(false);
        }

        private async Task SuspendAsync(State state, bool deleteLogs, Exception exception)
        {
            try
            {
                IEnumerable<Log> unsentLogs = null;
                using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
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
                    using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
                    {
                        _pendingLogCount = 0;
                    }
                    await DeleteLogsOnSuspendedAsync().ConfigureAwait(false);
                }
                await _storage.ClearPendingLogStateAsync(Name).ConfigureAwait(false);
            }
            catch (StatefulMutexException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "The Suspend operation has been cancelled");
            }
        }

        private async Task DeleteLogsOnSuspendedAsync()
        {
            try
            {
                if (SendingLog != null || FailedToSendLog != null)
                {
                    await SignalDeletingLogsAsync().ConfigureAwait(false);
                }
            }
            catch (StorageException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Failed to invoke events for logs being deleted.");
                return;
            }
            await _storage.DeleteLogsAsync(Name).ConfigureAwait(false);
        }

        private async Task SignalDeletingLogsAsync()
        {
            var logs = new List<Log>();
            await _storage.GetLogsAsync(Name, ClearBatchSize, logs).ConfigureAwait(false);
            foreach (var log in logs)
            {
                SendingLog?.Invoke(this, new SendingLogEventArgs(log));
                FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
            }
            if (logs.Count >= ClearBatchSize)
            {
                await SignalDeletingLogsAsync().ConfigureAwait(false);
            }
        }

        private async Task TriggerIngestionAsync(State state)
        {
            using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
            {
                if (!_enabled)
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
                    await TriggerIngestionAsync(state, logs, batchId).ConfigureAwait(false);
                    await CheckPendingLogsAsync(state).ConfigureAwait(false);
                }
                catch (StorageException)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Something went wrong sending logs to ingestion");
                }
            }
        }

        private async Task TriggerIngestionAsync(State state, IList<Log> logs, string batchId)
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
                try
                {
                    await serviceCall.ExecuteAsync().ConfigureAwait(false);
                }
                catch (IngestionException exception)
                {
                    await HandleSendingFailureAsync(state, batchId, exception).ConfigureAwait(false);
                    return;
                }
            }
            await HandleSendingSuccessAsync(state, batchId).ConfigureAwait(false);
        }

        private async Task HandleSendingSuccessAsync(State state, string batchId)
        {
            if (!_mutex.IsCurrent(state))
            {
                return;
            }
            try
            {
                await _storage.DeleteLogsAsync(Name, batchId).ConfigureAwait(false);
            }
            catch (StorageException e)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Could not delete logs for batch {batchId}", e);
                throw;
            }
            finally
            {
                List<Log> removedLogs;
                using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
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

        private async Task HandleSendingFailureAsync(State state, string batchId, IngestionException e)
        {
            var isRecoverable = e?.IsRecoverable ?? false;
            MobileCenterLog.Error(MobileCenterLog.LogTag, $"Sending logs for channel '{Name}', batch '{batchId}' failed: {e?.Message}");
            List<Log> removedLogs;
            using (await _mutex.GetLockAsync(state).ConfigureAwait(false))
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
            await SuspendAsync(state, !isRecoverable, e).ConfigureAwait(false);
        }

        private async Task CheckPendingLogsAsync(State state)
        {
            if (!_enabled)
            {
                MobileCenterLog.Info(MobileCenterLog.LogTag, "The service has been disabled. Stop processing logs.");
                return;
            }

            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"CheckPendingLogs({Name}) pending log count: {_pendingLogCount}");
            if (_pendingLogCount >= _maxLogsPerBatch)
            {
                await TriggerIngestionAsync(state).ConfigureAwait(false);
            }
            else if (_pendingLogCount > 0 && !_batchScheduled)
            {
                _batchScheduled = true;

                // No need wait _batchTimeInterval here.
                var _ = Task.Run(async () =>
                {
                    await Task.Delay((int)_batchTimeInterval.TotalMilliseconds).ConfigureAwait(false);
                    if (_batchScheduled)
                    {
                        await TriggerIngestionAsync(_mutex.State).ConfigureAwait(false);
                    }
                });
            }
        }

        public Task ShutdownAsync()
        {
            return SuspendAsync(_mutex.State, false, new CancellationException());
        }

        public void Dispose()
        {
            _mutex.Dispose();
        }
    }
}
