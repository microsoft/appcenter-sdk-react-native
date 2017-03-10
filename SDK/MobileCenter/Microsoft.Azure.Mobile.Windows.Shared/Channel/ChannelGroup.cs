using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;
using Microsoft.Azure.Mobile.Utils;

namespace Microsoft.Azure.Mobile.Channel
{
    public class ChannelGroup : IChannel
    {
        /* While ChannelGroup is technically capable of deep nesting, note that this behavior is not tested */
        private readonly HashSet<IChannel> _channels = new HashSet<IChannel>();
        //private const long ShutdownTimeout = 5000;
        private readonly IIngestion _ingestion;
        private readonly IStorage _storage;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        /* This must be visible to crashes */
        public string AppSecret { get; set; }

        public event EnqueuingLogEventHandler EnqueuingLog;
        public event SendingLogEventHandler SendingLog;
        public event SentLogEventHandler SentLog;
        public event FailedToSendLogEventHandler FailedToSendLog;

        public ChannelGroup(string appSecret) : this(DefaultIngestion(), DefaultStorage(), appSecret) { }

        protected ChannelGroup(IIngestion ingestion, IStorage storage, string appSecret)
        {
            _ingestion = ingestion;
            _storage = storage;
            AppSecret = appSecret;
        }

        public void SetServerUrl(string serverUrl)
        {
            _mutex.Wait();
            _ingestion.SetServerUrl(serverUrl);
            _mutex.Release();
        }

        /// <exception cref="MobileCenterException">Attempted to add duplicate channel to group</exception>
        public IChannel AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"AddChannel({name})");
            var newChannel = new Channel(name, maxLogsPerBatch, batchTimeInterval, maxParallelBatches, AppSecret,
                     _ingestion, _storage);
            AddChannel(newChannel);
            return newChannel;
        }

        /// <exception cref="MobileCenterException">Attempted to add duplicate channel to group</exception>
        public void AddChannel(IChannel channel)
        {
            _mutex.Wait();
            try
            {
                if (channel == null)
                {
                    throw new MobileCenterException("Attempted to add null channel to group");
                }
                var added = _channels.Add(channel);
                if (!added)
                {
                    /* The benefit of throwing an exception in this case is debatable. Might make sense to allow this. */
                    throw new MobileCenterException("Attempted to add duplicate channel to group");
                }
                channel.EnqueuingLog += AnyChannelEnqueuingLog;
                channel.SendingLog += AnyChannelSendingLog;
                channel.SentLog += AnyChannelSentLog;
                channel.FailedToSendLog += AnyChannelFailedToSendLog;
            }
            finally
            {
                _mutex.Release();
            }
        }

        public void SetEnabled(bool enabled)
        {
            _mutex.Wait();
            foreach (var channel in _channels)
            {
                channel.SetEnabled(enabled);
            }
            _mutex.Release();
        }

        public void Shutdown()
        {
            _mutex.Wait();
            foreach (var channel in _channels)
            {
                channel.Shutdown();
            }

            //TODO need some kind of waiting/timeout?

            _mutex.Release();
        }

        public void Enqueue(Log log)
        {
            /* No-op; inherited fat interface */
        }

        private static IIngestion DefaultIngestion()
        {
            return new NetworkStateIngestion(new RetryableIngestion(new IngestionHttp()));
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
