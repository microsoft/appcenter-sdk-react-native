using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Mobile.Ingestion;
using Microsoft.Azure.Mobile.Ingestion.Http;
using Microsoft.Azure.Mobile.Storage;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Channel
{
    public sealed class ChannelGroup : IChannelGroup, IAppSecretHolder
    {
        private readonly HashSet<IChannelUnit> _channels = new HashSet<IChannelUnit>();
        private readonly TimeSpan _shutdownTimeout = TimeSpan.FromSeconds(5);
        private readonly IIngestion _ingestion;
        private readonly IStorage _storage;
        private readonly object _channelGroupLock = new object();
        private bool _isDisposed;
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
            ThrowIfDisposed();
            lock (_channelGroupLock)
            {
                _ingestion.SetLogUrl(logUrl);
            }
        }

        /// <exception cref="MobileCenterException">Attempted to add duplicate channel to group</exception>
        public IChannelUnit AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
        {
            ThrowIfDisposed();
            lock (_channelGroupLock)
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"AddChannel({name})");
                var newChannel = new Channel(name, maxLogsPerBatch, batchTimeInterval, maxParallelBatches, AppSecret,
                    _ingestion, _storage);
                AddChannel(newChannel);
                return newChannel;
            }
        }

        /// <exception cref="MobileCenterException">Attempted to add duplicate channel to group</exception>
        public void AddChannel(IChannelUnit channel)
        {
            ThrowIfDisposed();
            lock (_channelGroupLock)
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
        }

        public void SetEnabled(bool enabled)
        {
            ThrowIfDisposed();
            lock (_channelGroupLock)
            {
                foreach (var channel in _channels)
                {
                    channel.SetEnabled(enabled);
                }
            }
        }

        public Task Shutdown()
        {
            ThrowIfDisposed();
            var tasks = new List<Task>();
            lock (_channelGroupLock)
            {
                foreach (var channel in _channels)
                {
                    tasks.Add(channel.Shutdown());
                }
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Waiting for storage to finish operations");
                if (!_storage.Shutdown(_shutdownTimeout))
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Storage taking too long to finish operations; shutting down channel without waiting any longer.");
                }
            }
            return Task.WhenAll(tasks);
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
            _ingestion.Dispose();
            _storage.Dispose();
            _isDisposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("ChannelGroup");
            }
        }
    }
}
