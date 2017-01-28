using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using Microsoft.Azure.Mobile.UWP.Ingestion;
using Microsoft.Azure.Mobile.UWP.Storage;
using Windows.UI.Xaml;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public class Channel : IChannel
    {
        private IStorage _storage;
        private Sender _sender;
        private bool _enabled;
        private int _maxParallelBatches;
        private int _maxLogsPerBatch;
        private Dictionary<string, Batch> _sendingBatches;
        private ChannelTimer _timer;

        internal bool Enabled { get; set; }
        public Channel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
        {
            //construct
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
            // enqueue log
            
        }

        public void Clear()
        {
            _storage.DeleteLogs(Name);
        }

        private void Suspend(bool deleteLogs, Exception exception)
        {
            _enabled = false;//do we need to invalidate batches here?
            _timer.Stop();

            foreach (Batch batch in _sendingBatches.Values)
            {
                if (deleteLogs)
                {
                    foreach (ILog log in batch.Logs)
                    {
                        if (FailedToSendLog != null)
                        {
                            var eventArgs = new FailedToSendLogEventArgs(log, exception);
                            FailedToSendLog(this, eventArgs);
                        }
                    }
                }
            }

            /* TODO Close ingestion */

            if (deleteLogs)
            {
                Clear(); /* TODO there's probably more to it than this */
            }
            else
            {
                /* TODO tell storage to clear pending log state */
            }
        }

        private void InvalidateCurrentBatches()
        {
            // the runtime complexity of this can obviously be improved but starting with it this way for clarity
            foreach (Batch batch in _sendingBatches.Values)
            {
                batch.HasValidState = false;
            }
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
            string batchId = _storage.GetLogs(Name, _maxLogsPerBatch, out logs);
            if (batchId != null)
            {
                var batch = new Batch(batchId, logs);

                /* Before sending logs, trigger the sending event for this channel */
                if (SendingLog != null)
                {
                    foreach (ILog log in batch.Logs)
                    {
                        var eventArgs = new SendingLogEventArgs(log);
                        SendingLog(this, eventArgs);
                    }
                }
                _sender.SendLogs(batch.Logs);
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



        private class Batch
        {
            public Batch(string batchId, List<ILog> logs)
            {
                HasValidState = true;
                Logs = logs;
                Id = batchId;
            }
            public bool HasValidState { get; set; }
            public List<ILog> Logs { get; set; }
            public string Id { get; set; }
        }
    }
}
