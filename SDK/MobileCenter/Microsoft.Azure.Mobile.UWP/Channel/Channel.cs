using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using Microsoft.Azure.Mobile.UWP.Ingestion;
using Microsoft.Azure.Mobile.UWP.Storage;
using Windows.UI.Xaml;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public class Channel : IChannel
    {
        private IStorage _storage;
        private ISender _sender;
        private bool _enabled;
        private int _maxParallelBatches;
        private int _maxLogsPerBatch;
        private Dictionary<string, List<ILog>> _sendingBatches;
        private Device _device;
        private int _currentState;
        private ChannelTimer _timer;

        public Channel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
        {
            Name = name;
            _timer = new ChannelTimer(maxLogsPerBatch, batchTimeInterval);
            _maxParallelBatches = maxParallelBatches;
            _maxLogsPerBatch = maxLogsPerBatch;
            //TODO need to properly create storage and sender
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
                    InvalidateCurrentState();
                    //TODO more?
                }
                else
                {
                    Suspend(true, null); //TODO exception?
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
            //TODO what if we're disabled?

            // enqueue log
            EnqueuingLog?.Invoke(this, new EnqueuingLogEventArgs(log));

            if (log.Device == null)
            {
                //TODO set the device
            }

            if (log.TOffset == 0L)
            {
                //TODO set the time offset in milliseconds
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
                    _timer.IncrementLogCount();
                }
                else
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "Error persisting log", completedTask.Exception);
                }
            });
        }

        public void Clear()
        {
            _storage.DeleteLogsAsync(Name).Start(); //TODO continue with something instead?
        }

        private void Suspend(bool deleteLogs, Exception exception)
        {
            _enabled = false; //do we need to invalidate batches here?
            _timer.Stop();

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
            catch (Exception) //TODO don't catch just any exception?
            {
                //TODO log error
            }

            if (deleteLogs)
            {
                Clear(); /* TODO there's probably more to it than this */
            }
            else
            {
                /* TODO tell storage to clear pending log state */
            }
        }

        //when the enabled state is changed OR this group is removed, the current batches must stop what they are doing. This signals that to them
        private void InvalidateCurrentState()
        {
            _currentState++;
        }
        private void TriggerIngestion()
        {
            if (!_enabled)
            {
                return;
            }

            //prepares a new batch and starts seding them to ingestion
            if (_sendingBatches.Count >= _maxParallelBatches)
            {
                /*TODO log message */
                return;
            }
            /* Get a batch from storage */
            var logs = new List<ILog>();
            int stateSnapshot = _currentState;
            _storage.GetLogsAsync(Name, _maxLogsPerBatch, out logs).ContinueWith((completedTask) =>
            {
                if (completedTask.Result != null && stateSnapshot == _currentState)
                {
                    TriggerIngestion(logs, stateSnapshot, completedTask.Result);
                }
            });
        }

        private void TriggerIngestion(List<ILog> logs, int stateSnapshot, string batchId)
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
            //TODO why does this need to be async if we are already executing in a background thread?
            _sender.SendLogsAsync(logs).ContinueWith((completedTask) =>
            {
                if (_currentState != stateSnapshot)
                {
                    return;
                }
                if (completedTask.Result.Success)
                {
                    HandleSendingSuccess(batchId);
                }
                else
                {
                    HandleSendingFailure(batchId, completedTask.Result.Exception);
                }
            });
        }

        private void HandleSendingSuccess(string batchId)
        {
            _storage.DeleteLogsAsync(Name, batchId);//TODO should this call actually be async?
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
        
        private void HandleSendingFailure(string batchId, Exception exception)
        {
            //TODO log an error
            var removedLogs = _sendingBatches[batchId];
            _sendingBatches.Remove(batchId);
            bool recoverableError = true; //TODO use an actual way of determining this involving exception
            if (recoverableError)
            {
                //TODO do something here
            }
            else if (FailedToSendLog != null)
            {
                foreach (var log in removedLogs)
                {
                    FailedToSendLog(this, new FailedToSendLogEventArgs(log, exception));
                }
            }

        }
        private class ChannelTimer
        {
            private int _maxLogsPerBatch;
            private int _numLogs = 0;
            private DispatcherTimer _timer = new DispatcherTimer();

            public ChannelTimer(int maxLogsPerBatch, TimeSpan batchTimeInterval)
            {
                _maxLogsPerBatch = maxLogsPerBatch;
                _timer.Interval = batchTimeInterval;
                _timer.Tick += TimeElapsed;
            }

            public event Action Elapsed;

            private void TimeElapsed(object sender, object e)
            {
                Stop();
                Elapsed?.Invoke();
            }

            public void IncrementLogCount()
            {
                ++_numLogs;
                if (_numLogs == 1)
                {
                    _timer.Start();
                }
                if (_numLogs >= _maxLogsPerBatch)
                {
                    TimeElapsed(null, null);
                }
            }

            public void Stop()
            {
                _timer.Stop();
                _numLogs = 0;
            }
        }
    }
}
