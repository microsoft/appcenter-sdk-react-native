using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using Microsoft.Azure.Mobile.UWP.Ingestion;
using Microsoft.Azure.Mobile.UWP.Storage;
using Windows.UI.Xaml;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public class Channel : IChannel
    {
        //TODO suffix async methods with async
        //TODO research issues with awaiting inside of a lock and find solutions...probably will need to use a monitor
        private IStorage _storage;
        private ISender _sender;
        private bool _enabled;
        private bool _discardLogs;
        private int _maxParallelBatches;
        private int _maxLogsPerBatch;
        private long _pendingLogCount;
        private Dictionary<string, List<ILog>> _sendingBatches = new Dictionary<string, List<ILog>>();
        private Ingestion.Models.Device _device;
        private int _currentState;
        private DispatcherTimer _timer = new DispatcherTimer();
        private const int ClearBatchSize = 100;

        //TODO maybe the solution to the countfromdisk problem is using a factory and hiding constructors? this seems best. then a user can't ever use a channel that hasn't checked disk for logs yet
        public Channel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches, ISender sender, IStorage storage)
        {
            Name = name;
            _timer.Interval = batchTimeInterval;
            _timer.Tick += TimerElapsed;
            _maxParallelBatches = maxParallelBatches;
            _maxLogsPerBatch = maxLogsPerBatch;
            _sender = sender;
            _storage = storage;
        }

        public async void CountFromDisk()
        {
            _pendingLogCount = await _storage.CountLogs(Name); //TODO do we need the state thing here? I feel like we might actually not
            CheckPendingLogs();
        }

        private void TimerElapsed(object o, object a)
        {
            _timer.Stop();
            TriggerIngestion().Start();
        }

        internal bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled == value)
                {
                    return;
                }
                if (value)
                {
                    _enabled = true;
                    _discardLogs = false;
                    InvalidateCurrentState();
                    CheckPendingLogs();
                    //TODO more?
                }
                else
                {
                    Suspend(true, new CancellationException());
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

        public void Enqueue(ILog log)
        {
            if (!_enabled && _discardLogs)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Channel is disabled; logs are discarded");
                SendingLog?.Invoke(this, new SendingLogEventArgs(log));
                FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
                return;
            }

            // enqueue log
            EnqueuingLog?.Invoke(this, new EnqueuingLogEventArgs(log));

            if (log.Device == null)
            {
                if (_device == null)
                {
                    try
                    {
                        _device = MiscStubs.GetDeviceInfo();
                    }
                    catch (Exception e) //TODO in android we catch a specific DeviceInfoException
                    {
                        MobileCenterLog.Error(MobileCenterLog.LogTag, "Device log cannot be generated", e);
                        return;
                    }
                }
                log.Device = _device;
            }

            if (log.TOffset == 0L)
            {
                log.TOffset = MiscStubs.CurrentTimeInMilliseconds();
            }

            //TODO add data to logs as needed
            int stateSnapshot = _currentState;
            _storage.PutLogAsync(Name, log).ContinueWith((completedTask) =>
            {
                if (stateSnapshot != _currentState)
                {
                    return;
                }
                if (completedTask.Result)
                {
                    _pendingLogCount++;
                    if (_enabled)
                    {
                        CheckPendingLogs();
                    }
                    else
                    {
                        MobileCenterLog.Warn(MobileCenterLog.LogTag, "Channel is temporarily disabled; log was saved to disk");
                    }
                }
                else
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "Error persisting log", completedTask.Exception);
                }
            });
        }

        public void Clear()
        {
            _storage.DeleteLogsAsync(Name).ContinueWith((completedTask) =>
            {
                _pendingLogCount = 0; //TODO is that correct?
            });
        }

        private void Suspend(bool deleteLogs, Exception exception)
        {
            _enabled = false;
            _discardLogs = deleteLogs;
            InvalidateCurrentState();
            foreach (List<ILog> batch in _sendingBatches.Values)
            {
                if (deleteLogs)
                {
                    foreach (ILog log in batch)
                    {
                        if (FailedToSendLog != null)
                        {
                            var eventArgs = new FailedToSendLogEventArgs(log, exception);
                            FailedToSendLog(this, eventArgs);
                        }
                    }
                }
            }
            try
            {
                _sender.Close();
            }
            catch (Exception e) //TODO don't catch just any exception?
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Failed to close ingestion", e);
            }

            if (deleteLogs)
            {
                _pendingLogCount = 0;
                DeleteLogsOnSuspended().Start();
            }
            else
            {
                _storage.ClearPendingLogState(); //TODO need to actually only do this for current channel, so pass name?
            }
        }

        private async Task DeleteLogsOnSuspended()
        {
            int stateSnapshot = _currentState;
            if (SendingLog == null && FailedToSendLog == null)
            {
                _storage.DeleteLogsAsync(Name).Start();
                return;
            }

            await DeleteLogsOnSuspended(_currentState);
        }

        private async Task DeleteLogsOnSuspended(int stateSnapshot)
        {
            if (stateSnapshot != _currentState)
            {
                return;
            }
            List<ILog> logs;
            //TODO put a try-catch?
            string batchId = await _storage.GetLogsAsync(Name, ClearBatchSize, out logs);

            if (stateSnapshot != _currentState) //TODO what if batchid == null?
            {
                return;
            }

            foreach (ILog log in logs)
            {
                SendingLog?.Invoke(this, new SendingLogEventArgs(log));
                FailedToSendLog?.Invoke(this, new FailedToSendLogEventArgs(log, new CancellationException()));
            }
            if (logs.Count >= ClearBatchSize)
            {
                await DeleteLogsOnSuspended(stateSnapshot);
                return;
            }
            _storage.DeleteLogsAsync(Name).Start();
        }

        private void InvalidateCurrentState()
        {
            _currentState++;
        }

        private async Task TriggerIngestion()
        {
            if (!_enabled)
            {
                return;
            }

            //prepares a new batch and starts seding them to ingestion
            if (_sendingBatches.Count >= _maxParallelBatches)
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Already sending " + _maxParallelBatches + " batches of analytics data to the server");
                return;
            }
            /* Get a batch from storage */
            var logs = new List<ILog>();
            int stateSnapshot = _currentState;
            string batchId = await _storage.GetLogsAsync(Name, _maxLogsPerBatch, out logs);
            if (batchId != null && stateSnapshot == _currentState)
            {
                _sendingBatches.Add(batchId, logs);
                _pendingLogCount -= logs.Count;
                await TriggerIngestion(logs, stateSnapshot, batchId);
            }
        }

        private async Task TriggerIngestion(List<ILog> logs, int stateSnapshot, string batchId)
        {
            /* Before sending logs, trigger the sending event for this channel */
            if (SendingLog != null)
            {
                foreach (ILog log in logs)
                {
                    var eventArgs = new SendingLogEventArgs(log);
                    SendingLog(this, eventArgs);
                }
            }
            var result = await _sender.SendLogsAsync(logs);
            if (_currentState != stateSnapshot)
            {
                return;
            }
            if (result.Success)
            {
                _storage.DeleteLogsAsync(Name, batchId).Start();
                var removedLogs = _sendingBatches[batchId];
                _sendingBatches.Remove(batchId);
                if (SentLog != null)
                {
                    foreach (var log in removedLogs)
                    {
                        SentLog(this, new SentLogEventArgs(log));
                    }
                }
            }
            else
            {
                MobileCenterLog.Error(MobileCenterLog.LogTag, "Sending logs for channel '" + Name + "', batch '" + batchId + "' failed", result.Exception);
                var removedLogs = _sendingBatches[batchId];
                _sendingBatches.Remove(batchId);
                bool recoverableError = MiscStubs.IsRecoverableHttpError(result.Exception);
                if (!recoverableError && FailedToSendLog != null)
                {
                    foreach (var log in removedLogs)
                    {
                        FailedToSendLog(this, new FailedToSendLogEventArgs(log, result.Exception));
                    }
                }
                Suspend(!recoverableError, result.Exception);
                if (recoverableError)
                {
                    _pendingLogCount += removedLogs.Count;
                }
            }

            CheckPendingLogs();
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
                TriggerIngestion().Start();
            }
            else if (_pendingLogCount > 0 && !_timer.IsEnabled)
            {
                _timer.Start();
            }
        }
    }
}
