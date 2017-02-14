using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Storage;
using System.Runtime.CompilerServices;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Rest;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Channel
{
    public class Channel : IChannel
    {
        private const int ClearBatchSize = 100;

        private Ingestion.Models.Device _device;
        private string _appSecret;
        private Guid _installId;

        private IStorage _storage;
        private IIngestion _ingestion;
        private IDeviceInformationHelper _deviceInfoHelper = new DeviceInformationHelper();
        private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private Dictionary<string, List<Log>> _sendingBatches = new Dictionary<string, List<Log>>();

        private int _maxParallelBatches;
        private int _maxLogsPerBatch;

        private long _pendingLogCount;
        private bool _enabled;
        private bool _discardLogs;
        private int _currentState;
        private bool _batchScheduled;
        private TimeSpan _batchTimeInterval;
        //NOTE: the constructor should only be called from ChannelGroup
        internal Channel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches, string appSecret, Guid installId, IIngestion ingestion, IStorage storage, DeviceInformationHelper deviceInfoHelper)
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
            _deviceInfoHelper = deviceInfoHelper;
            _deviceInfoHelper.InformationInvalidated += () => InvalidateDeviceCache();
            CountFromDiskAsync();
        }

        


        private async Task CountFromDiskAsync()
        {
            await _mutex.WaitAsync();
            int stateSnapshot = _currentState;
            _mutex.Release();

            int logCount = await _storage.CountLogsAsync(Name);
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

        public string Name { private set; get; }

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
                log = PrepareLog(log);
                PersistLogAsync(log, _currentState);
            }
            catch (Exception e) //TODO make some kind of deviceinformationexception
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Device log cannot be generated", e);
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
            //TODO cancel scheduled batch here? or is it fine?
            _mutex.Release();
        }

        private Log PrepareLog(Log log) //TODO does this really need to return anything?
        {
            //TODO probably more steps
            if (log.Device == null && _device == null)
            {
                _device = _deviceInfoHelper.GetDeviceInformation();
            }
            log.Device = log.Device ?? _device;
            if (log.Toffset == 0L)
            {
                log.Toffset = TimeHelper.CurrentTimeInMilliseconds();
            }
            return log;
        }

        private async Task PersistLogAsync(Log log, int stateSnapshot)
        {
            try
            {
                await _storage.PutLogAsync(Name, log);
            }
            catch (Exception e)
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
            int stateSnapshot = _currentState;

            _storage.DeleteLogsAsync(Name).ContinueWith((completedTask) =>
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
            _mutex.Wait();
            try
            {
                _enabled = false;
                _discardLogs = deleteLogs;
                _currentState++;
                if (deleteLogs && FailedToSendLog != null)
                {
                    foreach (List<Log> batch in _sendingBatches.Values)
                    {
                        foreach (Log log in batch)
                        {
                            FailedToSendLog(this, new FailedToSendLogEventArgs(log, exception));
                        }
                    }
                }
                _ingestion.Close();
                if (deleteLogs)
                {
                    _pendingLogCount = 0;
                    Task.Run(() => DeleteLogsOnSuspendedAsync());
                    return;
                }
                _storage.ClearPendingLogState(Name);
            }
            catch (IngestionException e) //TODO change this exception type
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Failed to close ingestion", e);
            }
            finally
            {
                _mutex.Release();
            }
        }

        private async Task DeleteLogsOnSuspendedAsync()
        {
            await _mutex.WaitAsync();
            try
            {
                if (SendingLog != null || FailedToSendLog != null)
                {
                    int stateSnapshot = _currentState;
                    await SignalDeletingLogs(stateSnapshot);
                }
                _mutex.Release();
                await _storage.DeleteLogsAsync(Name); //TODO if this throws an exception we will catch it and then try to release the mutex that we don't own
            }
            catch (Exception)
            {
                _mutex.Release();
                throw;
            }
        }

        private async Task SignalDeletingLogs(int stateSnapshot)
        {
            if (stateSnapshot != _currentState)
            {
                return;
            }
            var logs = new List<Log>();
            _mutex.Release();
            //TODO put a try-catch?
            string batchId = await _storage.GetLogsAsync(Name, ClearBatchSize, logs);
            await _mutex.WaitAsync();

            if (stateSnapshot != _currentState) //TODO what if batchid == null?
            {
                return;
            }

            foreach (Log log in logs)
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
        {//TODO there was a case where ingestion was triggered but there were no pending logs. investigate.
            await _mutex.WaitAsync();
            try
            {
                if (!_enabled)
                {
                    return;
                }
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"triggerIngestion({Name}) pendingLogCount={_pendingLogCount}");
                _batchScheduled = false;
                //prepares a new batch and starts seding them to ingestion
                if (_sendingBatches.Count >= _maxParallelBatches)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "Already sending " + _maxParallelBatches + " batches of analytics data to the server");
                    return;
                }

                /* Get a batch from storage */
                var logs = new List<Log>();
                int stateSnapshot = _currentState;
                _mutex.Release();
                string batchId = await _storage.GetLogsAsync(Name, _maxLogsPerBatch, logs); //TODO if this throws we will delete an unowned mutex
                await _mutex.WaitAsync();
                if (batchId != null && stateSnapshot == _currentState)
                {
                    _sendingBatches.Add(batchId, logs);
                    _pendingLogCount -= logs.Count;
                    await TriggerIngestionAsync(logs, stateSnapshot, batchId);
                }
            }
            finally
            {
                _mutex.Release();
            }
        }

        private async Task TriggerIngestionAsync(List<Log> logs, int stateSnapshot, string batchId)
        {
            /* Before sending logs, trigger the sending event for this channel */
            if (SendingLog != null)
            {
                foreach (Log log in logs)
                {
                    var eventArgs = new SendingLogEventArgs(log);
                    SendingLog(this, eventArgs);
                }
            }
            _mutex.Release();
            try
            {
                await _ingestion.SendLogsAsync(_appSecret, _installId, logs);
                await _mutex.WaitAsync();
            }
            catch (Exception e) //TODO this is not the right type to catch; should be more broad. Probably need to wrap expected exceptions into a type for ingestion
            {
                await _mutex.WaitAsync();
                HandleSendingFailure(batchId, e);
            }
            if (_currentState != stateSnapshot)
            {
                return;
            }

            _storage.DeleteLogsAsync(Name, batchId);
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

        private void HandleSendingFailure(string batchId, Exception e)
        {
            MobileCenterLog.Error(MobileCenterLog.LogTag, "Sending logs for channel '" + Name + "', batch '" + batchId + "' failed", e);
            var removedLogs = _sendingBatches[batchId];
            _sendingBatches.Remove(batchId);
            bool recoverableError = HttpUtils.IsRecoverableError(e);
            if (!recoverableError && FailedToSendLog != null)
            {
                foreach (var log in removedLogs)
                {
                    FailedToSendLog(this, new FailedToSendLogEventArgs(log, e));
                }
            }
            Suspend(!recoverableError, e);
            if (recoverableError)
            {
                _pendingLogCount += removedLogs.Count;
            }
            return;
        }
        private void CheckPendingLogs()
        {
            if (!_enabled)
            {
                MobileCenterLog.Info(MobileCenterLog.LogTag, "The service has been disabled. Stop processing logs.");
                return;
            }

            MobileCenterLog.Debug(MobileCenterLog.LogTag, "CheckPendingLogs(" + Name + ") pending log count: " + _pendingLogCount);
            if (_pendingLogCount >= _maxLogsPerBatch)
            {
                Task.Run(() => TriggerIngestionAsync());
            }
            else if (_pendingLogCount > 0 && !_batchScheduled)
            {
                _batchScheduled = true;
                Task.Delay((int)_batchTimeInterval.TotalMilliseconds).ContinueWith(async (completedTask) =>
                {

                    _mutex.Wait();
                    bool stillSchedued = _batchScheduled;
                    _mutex.Release();
                    if (stillSchedued) //TODO consider VERY carefully whether there is a race condition if batchscheduled changes right before this check
                    {
                        await TriggerIngestionAsync();
                    }
                });
            }
        }



        public void Shutdown()
        {
            Suspend(false, new CancellationException());
        }
    }
}
