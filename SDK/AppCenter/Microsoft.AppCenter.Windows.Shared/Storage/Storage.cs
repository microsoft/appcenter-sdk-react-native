// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Newtonsoft.Json;
using SQLite;

namespace Microsoft.AppCenter.Storage
{
    /// <summary>
    /// Manages the database of App Center logs on disk
    /// </summary>
    internal sealed class Storage : IStorage
    {
        internal class LogEntry
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            // The name of the channel that emitted the log
            public string Channel { get; set; }

            // The serialized json text of the log
            public string Log { get; set; }
        }

        private readonly IStorageAdapter _storageAdapter;
        private const string Database = "Microsoft.AppCenter.Storage";
        private const string DbIdentifierDelimiter = "@";

        private readonly Dictionary<string, List<long>> _pendingDbIdentifierGroups = new Dictionary<string, List<long>>();
        private readonly HashSet<long> _pendingDbIdentifiers = new HashSet<long>();
        // Blocking collection is thread safe
        private readonly BlockingCollection<Task> _queue = new BlockingCollection<Task>();
        private readonly SemaphoreSlim _flushSemaphore = new SemaphoreSlim(0);
        private readonly Task _queueFlushTask;

        /// <summary>
        /// Creates an instance of Storage
        /// </summary>
        public Storage() : this(DefaultAdapter())
        {
        }

        /// <summary>
        /// Creates an instance of Storage given a connection object
        /// </summary>
        internal Storage(IStorageAdapter adapter)
        {
            _storageAdapter = adapter;
            _queue.Add(new Task(() => InitializeDatabaseAsync().GetAwaiter().GetResult()));
            _queueFlushTask = Task.Run(FlushQueueAsync);
        }

        private static IStorageAdapter DefaultAdapter()
        {
            try
            {
                return new StorageAdapter(Database);
            }
            catch (FileLoadException e)
            {
                if (e.Message.Contains("SQLite-net"))
                {
                    AppCenterLog.Error(AppCenterLog.LogTag,
                        "If you are using sqlite-net-pcl version 1.4.118, please use a different version. " +
                        "There is a known bug in this version that will prevent App Center from working properly.");
                    throw new StorageException("Cannot initialize SQLite library.", e);
                }
                throw;
            }
        }

        /// <summary>
        /// Asynchronously adds a log to storage
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the log</param>
        /// <param name="log">The log to add</param>
        /// <exception cref="StorageException"/>
        public Task PutLog(string channelName, Log log)
        {
            return AddTaskToQueue(() =>
            {
                var logJsonString = LogSerializer.Serialize(log);
                var logEntry = new LogEntry { Channel = channelName, Log = logJsonString };
                _storageAdapter.InsertAsync(logEntry).GetAwaiter().GetResult();
            });
        }

        /// <summary>
        /// Asynchronously deletes all logs in a particular batch
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the batch</param>
        /// <param name="batchId">The batch identifier</param>
        /// <exception cref="StorageException"/>
        public Task DeleteLogs(string channelName, string batchId)
        {
            return AddTaskToQueue(() =>
            {
                try
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag,
                        $"Deleting logs from storage for channel '{channelName}' with batch id '{batchId}'");
                    var identifiers = _pendingDbIdentifierGroups[GetFullIdentifier(channelName, batchId)];
                    _pendingDbIdentifierGroups.Remove(GetFullIdentifier(channelName, batchId));
                    var deletedIdsMessage = "The IDs for deleting log(s) is/ are:";
                    foreach (var id in identifiers)
                    {
                        deletedIdsMessage += "\n\t" + id;
                        _pendingDbIdentifiers.Remove(id);
                    }
                    AppCenterLog.Debug(AppCenterLog.LogTag, deletedIdsMessage);
                    foreach (var id in identifiers)
                    {
                        _storageAdapter
                            .DeleteAsync<LogEntry>(entry => entry.Channel == channelName && entry.Id == id)
                            .GetAwaiter().GetResult();
                    }
                }
                catch (KeyNotFoundException e)
                {
                    throw new StorageException(e);
                }
            });
        }

        /// <summary>
        /// Asynchronously deletes all logs for a particular channel
        /// </summary>
        /// <param name="channelName">Name of the channel to delete logs for</param>
        /// <exception cref="StorageException"/>
        public Task DeleteLogs(string channelName)
        {
            return AddTaskToQueue(() =>
            {
                try
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag,
                        $"Deleting all logs from storage for channel '{channelName}'");
                    ClearPendingLogStateWithoutEnqueue(channelName);
                    _storageAdapter.DeleteAsync<LogEntry>(entry => entry.Channel == channelName)
                        .GetAwaiter().GetResult();
                }
                catch (KeyNotFoundException e)
                {
                    throw new StorageException(e);
                }
            });
        }

        /// <summary>
        /// Asynchronously counts the number of logs stored for a particular channel
        /// </summary>
        /// <param name="channelName">The name of the channel to count logs for</param>
        /// <returns>The number of logs found in storage</returns>
        /// <exception cref="StorageException"/>
        public Task<int> CountLogsAsync(string channelName)
        {
            return AddTaskToQueue(() =>
            {
                return _storageAdapter.CountAsync<LogEntry>(entry => entry.Channel == channelName)
                    .GetAwaiter().GetResult();
            });
        }

        /// <summary>
        /// Asynchronously clears the stored state of logs that have been retrieved
        /// </summary>
        /// <param name="channelName"></param>
        public Task ClearPendingLogState(string channelName)
        {
            return AddTaskToQueue(() =>
            {
                ClearPendingLogStateWithoutEnqueue(channelName);
                AppCenterLog.Debug(AppCenterLog.LogTag, $"Clear pending log states for channel {channelName}");
            });
        }

        private void ClearPendingLogStateWithoutEnqueue(string channelName)
        {
            var fullIdentifiers = new List<string>();

            foreach (var fullIdentifier in _pendingDbIdentifierGroups.Keys)
            {
                if (!ChannelMatchesIdentifier(channelName, fullIdentifier))
                {
                    continue;
                }
                foreach (var id in _pendingDbIdentifierGroups[fullIdentifier])
                {
                    _pendingDbIdentifiers.Remove(id);
                }
                fullIdentifiers.Add(fullIdentifier);
            }
            foreach (var fullIdentifier in fullIdentifiers)
            {
                _pendingDbIdentifierGroups.Remove(fullIdentifier);
            }
        }

        /// <summary>
        /// Asynchronously retrieves logs from storage and flags them to avoid duplicate retrievals on subsequent calls
        /// </summary>
        /// <param name="channelName">Name of the channel to retrieve logs from</param>
        /// <param name="limit">The maximum number of logs to retrieve</param>
        /// <param name="logs">A list to which the retrieved logs will be added</param>
        /// <returns>A batch ID for the set of returned logs; null if no logs are found</returns>
        /// <exception cref="StorageException"/>
        public Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            return AddTaskToQueue(() =>
            {
                logs?.Clear();
                var retrievedLogs = new List<Log>();
                AppCenterLog.Debug(AppCenterLog.LogTag,
                    $"Trying to get up to {limit} logs from storage for {channelName}");
                var idPairs = new List<Tuple<Guid?, long>>();
                var failedToDeserializeALog = false;
                var retrievedEntries =
                    _storageAdapter.GetAsync<LogEntry>(entry => entry.Channel == channelName, limit)
                        .GetAwaiter().GetResult();
                foreach (var entry in retrievedEntries)
                {
                    if (_pendingDbIdentifiers.Contains(entry.Id))
                    {
                        continue;
                    }
                    try
                    {
                        var log = LogSerializer.DeserializeLog(entry.Log);
                        retrievedLogs.Add(log);
                        idPairs.Add(Tuple.Create(log.Sid, Convert.ToInt64(entry.Id)));
                    }
                    catch (JsonException e)
                    {
                        AppCenterLog.Error(AppCenterLog.LogTag, "Cannot deserialize a log in storage", e);
                        failedToDeserializeALog = true;
                        _storageAdapter.DeleteAsync<LogEntry>(row => row.Id == entry.Id)
                            .GetAwaiter().GetResult();
                    }
                }
                if (failedToDeserializeALog)
                {
                    AppCenterLog.Warn(AppCenterLog.LogTag, "Deleted logs that could not be deserialized");
                }
                if (idPairs.Count == 0)
                {
                    AppCenterLog.Debug(AppCenterLog.LogTag,
                        $"No available logs in storage for channel '{channelName}'");
                    return null;
                }

                // Process the results
                var batchId = Guid.NewGuid().ToString();
                ProcessLogIds(channelName, batchId, idPairs);
                logs?.AddRange(retrievedLogs);
                return batchId;
            });
        }

        private void ProcessLogIds(string channelName, string batchId, IEnumerable<Tuple<Guid?, long>> idPairs)
        {
            var ids = new List<long>();
            var message = "The SID/ID pairs for returning logs are:";
            foreach (var idPair in idPairs)
            {
                var sidString = idPair.Item1?.ToString() ?? "(null)";
                message += "\n\t" + sidString + " / " + idPair.Item2;
                _pendingDbIdentifiers.Add(idPair.Item2);
                ids.Add(idPair.Item2);
            }
            _pendingDbIdentifierGroups.Add(GetFullIdentifier(channelName, batchId), ids);
            AppCenterLog.Debug(AppCenterLog.LogTag, message);
        }
       
        private async Task InitializeDatabaseAsync()
        {
            try
            {
                await _storageAdapter.CreateTableAsync<LogEntry>().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                AppCenterLog.Error(AppCenterLog.LogTag, "An error occurred while initializing storage", e);
            }
        }

        /// <summary>
        /// Waits for any running storage operations to complete
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for remaining tasks</param>
        /// <returns>True if remaining tasks completed in time; false otherwise</returns>
        public async Task WaitOperationsAsync(TimeSpan timeout)
        {
            var tokenSource = new CancellationTokenSource();
            try
            {
                var emptyQueueTask = AddTaskToQueue(() => { });
                var timeoutTask = Task.Delay(timeout, tokenSource.Token);
                await Task.WhenAny(emptyQueueTask, timeoutTask).ConfigureAwait(false);
            }
            finally
            {
                tokenSource.Cancel();
            }
        }

        /// <summary>
        /// Waits for any running storage operations to complete and prevents subsequent storage operations from running
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for remaining tasks</param>
        /// <returns>True if remaining tasks completed in time; false otherwise</returns>
        public async Task<bool> ShutdownAsync(TimeSpan timeout)
        {
            _queue.CompleteAdding();
            _flushSemaphore.Release();
            var tokenSource = new CancellationTokenSource();
            try
            {
                var timeoutTask = Task.Delay(timeout, tokenSource.Token);
                return await Task.WhenAny(_queueFlushTask, timeoutTask).ConfigureAwait(false) != timeoutTask;
            }
            finally
            {
                tokenSource.Cancel();
            }
        }

        private static string GetFullIdentifier(string channelName, string identifier)
        {
            return channelName + DbIdentifierDelimiter + identifier;
        }

        private static bool ChannelMatchesIdentifier(string channelName, string identifier)
        {
            var lastDelimiterIndex = identifier.LastIndexOf(DbIdentifierDelimiter, StringComparison.Ordinal);
            return identifier.Substring(0, lastDelimiterIndex) == channelName;
        }

        private Task AddTaskToQueue(Action action)
        {
            var task = new Task(action);
            AddTaskToQueue(task);
            return task;
        }

        private Task<T> AddTaskToQueue<T>(Func<T> action)
        {
            var task = new Task<T>(action);
            AddTaskToQueue(task);
            return task;
        }

        private void AddTaskToQueue(Task task)
        {
            try
            {
                _queue.Add(task);
            }
            catch (InvalidOperationException)
            {
                throw new StorageException("The operation has been canceled");
            }
            _flushSemaphore.Release();
        }

        // Flushes the queue
        private async Task FlushQueueAsync()
        {
            while (true)
            {
                while (_queue.Count == 0)
                {
                    if (_queue.IsAddingCompleted)
                    {
                        return;
                    }
                    await _flushSemaphore.WaitAsync();
                }
                var t = _queue.Take();
                t.Start();
                try
                {
                    await t.ConfigureAwait(false);
                }
                catch
                {
                    // Can't throw exceptions here because it will cause the FlushQueue to stop
                    // processing, but if the task faults, the exception will be thrown again 
                    // because the original creator of this task will await it too.
                }
            }
        }

        /// <summary>
        /// Disposes the storage object
        /// </summary>
        public void Dispose()
        {
            _queue.CompleteAdding();
        }
    }
}
