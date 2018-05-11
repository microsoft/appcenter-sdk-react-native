using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion;
using Microsoft.AppCenter.Ingestion.Http;
using Microsoft.AppCenter.Storage;

namespace Microsoft.AppCenter.Channel
{
    public sealed class ChannelGroup : IChannelGroup, IAppSecretHolder
    {
        private static readonly TimeSpan WaitStorageTimeout = TimeSpan.FromSeconds(5);

        private readonly HashSet<IChannelUnit> _channels = new HashSet<IChannelUnit>();

        private readonly IIngestion _ingestion;

        private readonly IStorage _storage;

        private readonly object _channelGroupLock = new object();

        private bool _isDisposed;

        private bool _isShutdown;

        public string AppSecret { get; internal set; }

        public event EventHandler<EnqueuingLogEventArgs> EnqueuingLog;

        public event EventHandler<FilteringLogEventArgs> FilteringLog;

        public event EventHandler<SendingLogEventArgs> SendingLog;

        public event EventHandler<SentLogEventArgs> SentLog;

        public event EventHandler<FailedToSendLogEventArgs> FailedToSendLog;

        public ChannelGroup(string appSecret)
            : this(appSecret, null, null)
        {
        }

        public ChannelGroup(string appSecret, IHttpNetworkAdapter httpNetwork, INetworkStateAdapter networkState)
            : this(DefaultIngestion(httpNetwork, networkState), DefaultStorage(), appSecret)
        {
        }

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

        /// <exception cref="AppCenterException">Attempted to add duplicate channel to group</exception>
        public IChannelUnit AddChannel(string name, int maxLogsPerBatch, TimeSpan batchTimeInterval, int maxParallelBatches)
        {
            ThrowIfDisposed();
            lock (_channelGroupLock)
            {
                AppCenterLog.Debug(AppCenterLog.LogTag, $"AddChannel({name})");
                var newChannel = new Channel(name, maxLogsPerBatch, batchTimeInterval, maxParallelBatches, AppSecret,
                    _ingestion, _storage);
                AddChannel(newChannel);
                return newChannel;
            }
        }

        /// <exception cref="AppCenterException">Attempted to add duplicate channel to group</exception>
        public void AddChannel(IChannelUnit channel)
        {
            ThrowIfDisposed();
            lock (_channelGroupLock)
            {
                if (channel == null)
                {
                    throw new AppCenterException("Attempted to add null channel to group");
                }
                var added = _channels.Add(channel);
                if (!added)
                {
                    // The benefit of throwing an exception in this case is debatable. Might make sense to allow this.
                    throw new AppCenterException("Attempted to add duplicate channel to group");
                }
                channel.EnqueuingLog += AnyChannelEnqueuingLog;
                channel.FilteringLog += AnyChannelFilteringLog;
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

        public Task WaitStorageOperationsAsync()
        {
            ThrowIfDisposed();
            AppCenterLog.Debug(AppCenterLog.LogTag, "Waiting for storage to finish operations.");
            return _storage.WaitAsync(WaitStorageTimeout);
        }

        public async Task ShutdownAsync()
        {
            ThrowIfDisposed();
            var tasks = new List<Task>();
            lock (_channelGroupLock)
            {
                if (_isShutdown)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, "Attempted to shutdown channel multiple times.");
                    return;
                }
                _isShutdown = true;
                _ingestion.Close();
                foreach (var channel in _channels)
                {
                    tasks.Add(channel.ShutdownAsync());
                }
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            AppCenterLog.Debug(AppCenterLog.LogTag, "Waiting for storage to finish operations.");
            if (!await _storage.ShutdownAsync(WaitStorageTimeout).ConfigureAwait(false))
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Storage taking too long to finish operations; shutting down channel without waiting any longer.");
            }
        }

        private static IIngestion DefaultIngestion(IHttpNetworkAdapter httpNetwork = null, INetworkStateAdapter networkState = null)
        {
            if (httpNetwork == null)
            {
                httpNetwork = new HttpNetworkAdapter();
            }
            if (networkState == null)
            {
                networkState = new NetworkStateAdapter();
            }
            return new NetworkStateIngestion(new RetryableIngestion(new IngestionHttp(httpNetwork)), networkState);
        }

        private static IStorage DefaultStorage()
        {
            return new Storage.Storage();
        }

        private void AnyChannelEnqueuingLog(object sender, EnqueuingLogEventArgs e)
        {
            EnqueuingLog?.Invoke(sender, e);
        }

        private void AnyChannelFilteringLog(object sender, FilteringLogEventArgs e)
        {
            FilteringLog?.Invoke(sender, e);
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
                throw new ObjectDisposedException(nameof(ChannelGroup));
            }
        }
    }
}
