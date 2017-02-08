using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Mobile.UWP.Ingestion;
using Microsoft.Azure.Mobile.UWP.Ingestion.Http;
using Microsoft.Azure.Mobile.UWP.Storage;

namespace Microsoft.Azure.Mobile.UWP.Channel
{
    public class ChannelGroup : IChannel
    {
        private Dictionary<string, Channel> _channels = new Dictionary<string, Channel>();
        private const long ShutdownTimeout = 5000;
        private IIngestion _ingestion;
        private IStorage _storage;
        private string _appSecret;
        private Guid _installId;
        private bool _enabled;
        private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        #region Events
        public event EnqueuingLogEventHandler EnqueuingLog;
        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;
        #endregion

        public ChannelGroup(IIngestion ingestion, IStorage storage, string appSecret)
        {
            _ingestion = ingestion;
            _storage = storage;
            _enabled = true;
            _appSecret = appSecret;
        }

        public ChannelGroup(string appSecret) : this(DefaultIngestion(), DefaultStorage(), appSecret)
        {
        }

        public void SetServerUrl(string serverUrl)
        {
            _mutex.Wait();
            _ingestion.SetServerUrl(serverUrl);
            _mutex.Release();
        }

        public void AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
        {
            _mutex.Wait();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "AddChannel(" + name + ")");
            //TODO error handling
            var newChannel = new Channel(name, maxLogsPerBatch, batchTimeInterval, maxParallelBatches, _appSecret, _installId, _ingestion, _storage);
            newChannel.EnqueuingLog += AnyChannelEnqueuingLog;
            newChannel.SendingLog += AnyChannelSendingLog;
            newChannel.SentLog += AnyChannelSentLog;
            newChannel.FailedToSendLog += AnyChannelFailedToSendLog;

            _channels.Add(name, newChannel);
            _mutex.Release();
        }

        public void RemoveChannel(Channel channel)
        {
            _mutex.Wait();
            //TODO error handling
            channel.NotifyStateInvalidated();
            channel.EnqueuingLog -= AnyChannelEnqueuingLog;
            channel.SendingLog -= AnyChannelSendingLog;
            channel.SentLog -= AnyChannelSentLog;
            channel.FailedToSendLog -= AnyChannelFailedToSendLog;
            _channels.Remove(channel.Name);
            _mutex.Release();
        }

        public Channel GetChannel(string channelName)
        {
            _mutex.Wait();
            var channel = _channels[channelName];
            _mutex.Release();
            return channel;
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
                foreach (var channel in _channels.Values)
                {
                    channel.Enabled = value;
                }
                _enabled = value;
                _mutex.Release();
            }
        }

        public void Shutdown()
        {
            _mutex.Wait();
            foreach (var channel in _channels.Values)
            {
                channel.Shutdown();
            }

            //TODO need some kind of waiting/timeout?

            _mutex.Release();
        }

        private static IIngestion DefaultIngestion()
        {
            return new IngestionNetworkStateHandler(new IngestionHttp());
        }

        private static IStorage DefaultStorage()
        {
            return new Storage.Storage();
        }

        //TODO should these be wrapped in mutex? don't think so, but give some thought later
        private void AnyChannelEnqueuingLog(object sender, EnqueuingLogEventArgs e)
        {
            EnqueuingLog?.Invoke(sender, e); //TODO should we pass "this" as sender or "sender"?
        }
        private void AnyChannelSendingLog(object sender, SendingLogEventArgs e)
        {
            SendingLog?.Invoke(sender, e); //TODO should we pass "this" as sender or "sender"?
        }
        private void AnyChannelSentLog(object sender, SentLogEventArgs e)
        {
            SentLog?.Invoke(sender, e); //TODO should we pass "this" as sender or "sender"?
        }
        private void AnyChannelFailedToSendLog(object sender, FailedToSendLogEventArgs e)
        {
            FailedToSendLog?.Invoke(sender, e); //TODO should we pass "this" as sender or "sender"?
        }
    }
}
