using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Channel
{
    public class ChannelGroup : IChannel
    {
        private readonly Dictionary<string, Channel> _channels = new Dictionary<string, Channel>();
        //private const long ShutdownTimeout = 5000;
        private readonly IIngestion _ingestion;
        private readonly IStorage _storage;
        private readonly string _appSecret;
        private readonly Guid _installId = IdHelper.InstallId;
        private bool _enabled;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public event EnqueuingLogEventHandler EnqueuingLog;
        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;

        public ChannelGroup(string appSecret) : this(DefaultIngestion(), DefaultStorage(), appSecret) { }

        protected ChannelGroup(IIngestion ingestion, IStorage storage, string appSecret)
        {
            _ingestion = ingestion;
            _storage = storage;
            _enabled = true;
            _appSecret = appSecret;
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
            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"AddChannel({name})");
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

        private void AnyChannelEnqueuingLog(object sender, EnqueuingLogEventArgs e)
        {
            EnqueuingLog?.Invoke(sender, e);
        }
        private void AnyChannelSendingLog(object sender, SendingLogEventArgs e)
        {
            SendingLog?.Invoke(sender, e);
        }
        private void AnyChannelSentLog(object sender, SentLogEventArgs e)
        {
            SentLog?.Invoke(sender, e);
        }
        private void AnyChannelFailedToSendLog(object sender, FailedToSendLogEventArgs e)
        {
            FailedToSendLog?.Invoke(sender, e);
        }
    }
}
