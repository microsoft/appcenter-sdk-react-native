using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;

namespace Microsoft.Azure.Mobile.Channel
{
    public sealed class ChannelGroup : IChannelGroup, IAppSecretHolder
    {
        private readonly HashSet<IChannelUnit> _channels = new HashSet<IChannelUnit>();
        private readonly TimeSpan _shutdownTimeout = TimeSpan.FromSeconds(5);
        private readonly IIngestion _ingestion;
        private readonly IStorage _storage;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public string AppSecret { get; internal set; }

        public event EventHandler<EnqueuingLogEventArgs> EnqueuingLog;
        public event EventHandler<SendingLogEventArgs> SendingLog;
        public event EventHandler<SentLogEventArgs> SentLog;
        public event EventHandler<FailedToSendLogEventArgs> FailedToSendLog;

        public ChannelGroup(string appSecret) : this(DefaultIngestion(), DefaultStorage(), appSecret) { }

        internal ChannelGroup(IIngestion ingestion, IStorage storage, string appSecret)
        {
            _ingestion = ingestion;
            _storage = storage;
            AppSecret = appSecret;
        }

        public void SetLogUrl(string logUrl)
        {
            _mutex.Wait();
            _ingestion.SetLogUrl(logUrl);
            _mutex.Release();
        }

        /// <exception cref="MobileCenterException">Attempted to add duplicate channel to group</exception>
        public IChannelUnit AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
        {
            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"AddChannel({name})");
            var newChannel = new Channel(name, maxLogsPerBatch, batchTimeInterval, maxParallelBatches, AppSecret,
                     _ingestion, _storage);
            AddChannel(newChannel);
            return newChannel;
        }

        /// <exception cref="MobileCenterException">Attempted to add duplicate channel to group</exception>
        public void AddChannel(IChannelUnit channel)
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
                    // The benefit of throwing an exception in this case is debatable. Might make sense to allow this.
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
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Waiting for storage to finish operations");
            if (!_storage.Shutdown(_shutdownTimeout))
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Storage taking too long to finish operations; shutting down channel without waiting any longer.");
            }
            _mutex.Release();
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

        public void Dispose()
        {
            foreach (var channel in _channels)
            {
                channel.Dispose();
            }
            _mutex.Dispose();
            _ingestion.Dispose();
            _storage.Dispose();
        }
    }
}
