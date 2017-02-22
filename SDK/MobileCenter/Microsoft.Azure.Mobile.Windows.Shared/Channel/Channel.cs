using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Channel
{
    public class Channel : IChannel
    {
        private const int ClearBatchSize = 100;
        private Ingestion.Models.Device _device;
        private readonly string _appSecret;
        private readonly Guid _installId;

        private readonly IStorage _storage;
        private readonly IIngestion _ingestion;
        private readonly IDeviceInformationHelper _deviceInfoHelper = new DeviceInformationHelper();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly Dictionary<string, List<Log>> _sendingBatches = new Dictionary<string, List<Log>>();

        private readonly int _maxParallelBatches;
        private readonly int _maxLogsPerBatch;

        private long _pendingLogCount;
        private bool _enabled;
        private bool _discardLogs;
        private int _currentState;
        private bool _batchScheduled;
        private TimeSpan _batchTimeInterval;
        //TODO investigate changing use of state snapshots to leverage cancellation tokens
        internal Channel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches, string appSecret, Guid installId, IIngestion ingestion, IStorage storage)
        {
            Name = name;
            _maxParallelBatches = maxParallelBatches;
            _maxLogsPerBatch = maxLogsPerBatch;
            _appSecret = appSecret;
            _installId = installId;
            _ingestion = ingestion;
            _storage = storage;
            _batchTimeInterval = batchTimeInterval;
            _batchScheduled = false;
            _enabled = true;
            _deviceInfoHelper.InformationInvalidated += () => InvalidateDeviceCache();
            Task.Factory.StartNew(()=>CountFromDiskAsync());
        }

        private async Task CountFromDiskAsync()
        {
            await _mutex.WaitAsync();
            var stateSnapshot = _currentState;
            _mutex.Release();

            var logCount = await _storage.CountLogsAsync(Name);
            await _mutex.WaitAsync();
            try
            {
                if (stateSnapshot == _currentState)
                {
                    _pendingLogCount = logCount;
                    CheckPendingLogs();
                }
            }
            finally
            {
                _mutex.Release();
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _mutex.Wait();
                try
                {
                    if (_enabled == value)
                    {
                        return;
                    }
                    if (value)
                    {
                        _enabled = true;
                        _discardLogs = false;
                        _currentState++;
                        CheckPendingLogs();
                    }
                    else
                    {
                        Suspend(true, new CancellationException());
                    }
                }
                finally
                {
                    _mutex.Release();
                }
            }
        }

        public string Name { get; }

        #region Events
        public event EnqueuingLogEventHandler EnqueuingLog;
        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;
        #endregion

        public void Enqueue(Log log)
        {
            _mutex.Wait();
            try
            {
                if (_discardLogs)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Channel is disabled; logs are discarded");
                    SendingLog?.Invoke(this, new SendingLogEventArgs(log));
                    FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
                    return;
                }
                EnqueuingLog?.Invoke(this, new EnqueuingLogEventArgs(log));
                PrepareLog(log);
                Task.Factory.StartNew(()=>PersistLogAsync(log, _currentState));
            }
            finally
            {
                _mutex.Release();
            }
        }

        public void NotifyStateInvalidated()
        {
            _mutex.Wait();
            _currentState++;
            _mutex.Release();
        }

        private void PrepareLog(Log log)
        {
            if (log.Device == null && _device == null)
            {
                _device = _deviceInfoHelper.GetDeviceInformation();
            }
            log.Device = log.Device ?? _device;
            if (log.Toffset == 0L)
            {
                log.Toffset = TimeHelper.CurrentTimeInMilliseconds();
            }
        }

        private async Task PersistLogAsync(Log log, int stateSnapshot)
        {
            try
            {
                await _storage.PutLogAsync(Name, log);
            }
            catch (StorageException e)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Error persisting log", e);
                return;
            }

            await _mutex.WaitAsync();
            try
            {
                if (stateSnapshot != _currentState)
                {
                    return;
                }
                _pendingLogCount++;
                if (_enabled)
                {
                    CheckPendingLogs();
                    return;
                }
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Channel is temporarily disabled; log was saved to disk");
            }
            finally
            {
                _mutex.Release();
            }
        }

        public void InvalidateDeviceCache()
        {
            _mutex.Wait();
            _device = null;
            _mutex.Release();
        }

        public void Clear()
        {
            var stateSnapshot = _currentState;
            _storage.DeleteLogsAsync(Name).ContinueWith(completedTask =>
            {
                _mutex.Wait();
                if (stateSnapshot == _currentState)
                {
                    _pendingLogCount = 0;
                }
                _mutex.Release();
            });
        }

        private void Suspend(bool deleteLogs, Exception exception)
        {
            _enabled = false;
            _batchScheduled = false;
            _discardLogs = deleteLogs;
            _currentState++;
            if (deleteLogs && FailedToSendLog != null)
            {
                foreach (var log in _sendingBatches.Values.SelectMany(batch => batch))
                {
                    FailedToSendLog(this, new FailedToSendLogEventArgs(log, exception));
                }
            }
            try
            {
                _ingestion.Close();
            }
            catch (IngestionException e)
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Failed to close ingestion", e);
            }
            if (deleteLogs)
            {
                _pendingLogCount = 0;
                Task.Run(() => DeleteLogsOnSuspendedAsync());
                return;
            }
            Task.Run(() => _storage.ClearPendingLogStateAsync(Name));
        }

        private async Task DeleteLogsOnSuspendedAsync()
        {
            await _mutex.WaitAsync();
            try
            {
                if (SendingLog != null || FailedToSendLog != null)
                {
                    var stateSnapshot = _currentState;
                    await SignalDeletingLogs(stateSnapshot);
                }
            }
            catch (StorageException)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Failed to invoke events for logs being deleted.");
                return;
            }
            finally
            {
                _mutex.Release();
            }
            await _storage.DeleteLogsAsync(Name);
        }

        private async Task SignalDeletingLogs(int stateSnapshot)
        {
            if (stateSnapshot != _currentState) return;
            var logs = new List<Log>();
            _mutex.Release();
            await _storage.GetLogsAsync(Name, ClearBatchSize, logs);
            await _mutex.WaitAsync();
            if (stateSnapshot != _currentState) return;
            foreach (var log in logs)
            {
                SendingLog?.Invoke(this, new SendingLogEventArgs(log));
                FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
            }
            if (logs.Count >= ClearBatchSize)
            {
                await SignalDeletingLogs(stateSnapshot);
            }
        }

        private async Task TriggerIngestionAsync()
        {
            var needsRelease = true;
            await _mutex.WaitAsync();
            try
            {
                if (!_enabled)
                {
                    return;
                }
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"triggerIngestion({Name}) pendingLogCount={_pendingLogCount}");
                _batchScheduled = false;
                if (_sendingBatches.Count >= _maxParallelBatches)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "Already sending " + _maxParallelBatches + " batches of analytics data to the server");
                    return;
                }

                /* Get a batch from storage */
                var logs = new List<Log>();
                var stateSnapshot = _currentState;
                _mutex.Release();
                needsRelease = false;
                var batchId = await _storage.GetLogsAsync(Name, _maxLogsPerBatch, logs);
                await _mutex.WaitAsync();
                needsRelease = true;
                if (batchId != null && stateSnapshot == _currentState)
                {
                    _sendingBatches.Add(batchId, logs);
                    _pendingLogCount -= logs.Count;
                    TriggerIngestion(logs, stateSnapshot, batchId);
                }
            }
            finally
            {
                if (needsRelease)
                {
                    _mutex.Release();
                }
            }
        }

        private void TriggerIngestion(IList<Log> logs, int stateSnapshot, string batchId)
        {
            /* Before sending logs, trigger the sending event for this channel */
            if (SendingLog != null)
            {
                foreach (var eventArgs in logs.Select(log => new SendingLogEventArgs(log)))
                {
                    SendingLog(this, eventArgs);
                }
            }
            var serviceCall = _ingestion.PrepareServiceCall(_appSecret, _installId, logs);
            serviceCall.Failed += exception => HandleSendingFailure(batchId, exception);
            serviceCall.Succeeded += async () =>
            {
                if (_currentState != stateSnapshot) return;
                try
                {
                    await _storage.DeleteLogsAsync(Name, batchId);
                }
                catch (StorageException e)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, $"Could not delete logs for batch {batchId}", e);
                }
                await _mutex.WaitAsync();
                try
                {

                    if (_currentState != stateSnapshot) return;
                    var removedLogs = _sendingBatches[batchId];
                    _sendingBatches.Remove(batchId);
                    if (SentLog != null)
                    {
                        foreach (var log in removedLogs)
                        {
                            SentLog(this, new SentLogEventArgs(log));
                        }
                    }
                    CheckPendingLogs();
                }
                finally
                {
                    _mutex.Release();
                }
            };
            serviceCall.Execute();
        }

        private void HandleSendingFailure(string batchId, IngestionException e)
        {
            var isRecoverable = e?.IsRecoverable ?? false;
            MobileCenterLog.Error(MobileCenterLog.LogTag, $"Sending logs for channel '{Name}', batch '{batchId}' failed", e);
            var removedLogs = _sendingBatches[batchId];
            _sendingBatches.Remove(batchId);
            if (!isRecoverable && FailedToSendLog != null)
            {
                foreach (var log in removedLogs)
                {
                    FailedToSendLog(this, new FailedToSendLogEventArgs(log, e));
                }
            }
            Suspend(!isRecoverable, e);
            if (isRecoverable)
            {
                _pendingLogCount += removedLogs.Count;
            }
        }
        private void CheckPendingLogs()
        {
            if (!_enabled)
            {
                MobileCenterLog.Info(MobileCenterLog.LogTag, "The service has been disabled. Stop processing logs.");
                return;
            }

            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"CheckPendingLogs({Name}) pending log count: {_pendingLogCount}");
            if (_pendingLogCount >= _maxLogsPerBatch)
            {
                Task.Run(() => TriggerIngestionAsync());
            }
            else if (_pendingLogCount > 0 && !_batchScheduled)
            {
                _batchScheduled = true;
                Task.Delay((int)_batchTimeInterval.TotalMilliseconds).ContinueWith(async completedTask =>
                {
                    if (_batchScheduled)
                    {
                        await TriggerIngestionAsync();
                    }
                });
            }
        }

        public void Shutdown()
        {
            _mutex.Wait();
            try
            {
                Suspend(false, new CancellationException());
            }
            finally
            {
                _mutex.Release();
            }
        }
    }
}
