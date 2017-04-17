using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.Azure.Mobile.Ingestion.Models;
using SQLite;

namespace Microsoft.Azure.Mobile.Storage
{
    /// <summary>
    /// Manages the database of Mobile Center logs on disk
    /// </summary>
    internal sealed class Storage : IStorage
    {
        private readonly IStorageAdapter _storageAdapter;

        internal class LogEntry
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            // The name of the channel that emitted the log
            public string Channel { get; set; }

            // The serialized json text of the log
            public string Log { get; set; }
        }

        private const string Database = "Microsoft.Azure.Mobile.Storage";
        private const string DbIdentifierDelimiter = "@";

        private readonly Dictionary<string, List<long>> _pendingDbIdentifierGroups = new Dictionary<string, List<long>>();
        private readonly HashSet<long> _pendingDbIdentifiers = new HashSet<long>();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        // Variables used to track the number of tasks being performed
        private bool _stopAddingOperations;
        private int _numTasksRemaning;
        private readonly SemaphoreSlim _taskSem = new SemaphoreSlim(1);
        private readonly object _taskLock = new object();

        /// <summary>
        /// Creates an instance of Storage
        /// </summary>
        public Storage() : this(new StorageAdapter(Database))
        {
        }

        /// <summary>
        /// Creates an instance of Storage given a connection object
        /// </summary>
        internal Storage(IStorageAdapter adapter)
        {
            _storageAdapter = adapter;
            StartTask();
            Task.Run(InitializeDatabaseAsync);
        }

        /// <summary>
        /// Asynchronously adds a log to storage
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the log</param>
        /// <param name="log">The log to add</param>
        /// <exception cref="StorageException"/>
        public async Task PutLogAsync(string channelName, Log log)
        {
            await StartTaskAsync().ConfigureAwait(false);
            try
            {
                var logJsonString = LogSerializer.Serialize(log);
                var logEntry = new LogEntry { Channel = channelName, Log = logJsonString };
                await _storageAdapter.InsertAsync(logEntry).ConfigureAwait(false);
            }
            finally
            {
                StopTask();
            }
        }

        /// <summary>
        /// Asynchronously deletes all logs in a particular batch
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the batch</param>
        /// <param name="batchId">The batch identifier</param>
        /// <exception cref="StorageException"/>
        public async Task DeleteLogsAsync(string channelName, string batchId)
        {
            await StartTaskAsync().ConfigureAwait(false);
            try
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag,
                    $"Deleting logs from storage for channel '{channelName}' with batch id '{batchId}'");
                var identifiers = _pendingDbIdentifierGroups[GetFullIdentifier(channelName, batchId)];
                _pendingDbIdentifierGroups.Remove(GetFullIdentifier(channelName, batchId));
                var deletedIdsMessage = "The IDs for deleting log(s) is/ are:";
                foreach (var id in identifiers)
                {
                    deletedIdsMessage += "\n\t" + id;
                    _pendingDbIdentifiers.Remove(id);
                }
                MobileCenterLog.Debug(MobileCenterLog.LogTag, deletedIdsMessage);
                foreach (var id in identifiers)
                {
                    await DeleteLogAsync(channelName, id).ConfigureAwait(false);
                }
            }
            catch (KeyNotFoundException e)
            {
                throw new StorageException(e);
            }
            finally
            {
                StopTask();
            }
        }

        /// <summary>
        /// Asynchronously deletes all logs for a particular channel
        /// </summary>
        /// <param name="channelName">Name of the channel to delete logs for</param>
        /// <exception cref="StorageException"/>
        public async Task DeleteLogsAsync(string channelName)
        {
            await StartTaskAsync().ConfigureAwait(false);
            try
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"Deleting all logs from storage for channel '{channelName}'");
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

                await _storageAdapter.DeleteAsync<LogEntry>(entry => entry.Channel == channelName).ConfigureAwait(false);
            }
            catch (KeyNotFoundException e)
            {
                throw new StorageException(e);
            }
            finally
            {
                StopTask();
            }
        }

        /// <summary>
        /// Asynchronously delete a single log from storage
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the log</param>
        /// <param name="rowId">The row id of the log</param>
        /// <exception cref="StorageException"/>
        private async Task DeleteLogAsync(string channelName, long rowId)
        {
            // Should be in a task already
            try
            {
                await _storageAdapter.DeleteAsync<LogEntry>(entry => entry.Id == rowId).ConfigureAwait(false);
            }
            catch (StorageException)
            {
                throw new StorageException($"Error deleting log from storage for channel '{channelName}' with id '{rowId}'");
            }
        }

        /// <summary>
        /// Asynchronously counts the number of logs stored for a particular channel
        /// </summary>
        /// <param name="channelName">The name of the channel to count logs for</param>
        /// <returns>The number of logs found in storage</returns>
        /// <exception cref="StorageException"/>
        public async Task<int> CountLogsAsync(string channelName)
        {
            await StartTaskAsync().ConfigureAwait(false);
            const string errorMessage = "Error counting logs";
            try
            {
                return await _storageAdapter.CountAsync<LogEntry>(entry => entry.Channel == channelName).ConfigureAwait(false);            
            }
            catch (SQLiteException e)
            {
                throw new StorageException(errorMessage, e);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new StorageException(errorMessage, e);
            }
            catch (KeyNotFoundException e)
            {
                throw new StorageException(errorMessage, e);
            }
            catch (InvalidCastException e)
            {
                throw new StorageException(errorMessage, e);
            }
            finally
            {
                StopTask();
            }
        }

        /// <summary>
        /// Asynchronously clears the stored state of logs that have been retrieved
        /// </summary>
        /// <param name="channelName"></param>
        public async Task ClearPendingLogStateAsync(string channelName)
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            _pendingDbIdentifierGroups.Clear();
            _pendingDbIdentifiers.Clear();
            _mutex.Release();
        }

        /// <summary>
        /// Asynchronously retrieves logs from storage and flags them to avoid duplicate retrievals on subsequent calls
        /// </summary>
        /// <param name="channelName">Name of the channel to retrieve logs from</param>
        /// <param name="limit">The maximum number of logs to retrieve</param>
        /// <param name="logs">A list to which the retrieved logs will be added</param>
        /// <returns>A batch ID for the set of returned logs; null if no logs are found</returns>
        /// <exception cref="StorageException"/>
        public async Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            await StartTaskAsync().ConfigureAwait(false);
            logs?.Clear();
            var retrievedLogs = new List<Log>();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"Trying to get up to {limit} logs from storage for {channelName}");
            try
            {
                var idPairs = new List<Tuple<Guid?, long>>();
                var failedToDeserializeALog = false;
                var retrievedEntries =
                    await _storageAdapter.GetAsync<LogEntry>(entry => entry.Channel == channelName, limit)
                        .ConfigureAwait(false);
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
                        MobileCenterLog.Error(MobileCenterLog.LogTag, "Cannot deserialize a log in storage", e);
                        failedToDeserializeALog = true;
                        await DeleteLogAsync(channelName, entry.Id).ConfigureAwait(false);
                    }
                }
                if (failedToDeserializeALog)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Deleted logs that could not be deserialized");
                }
                if (idPairs.Count == 0)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, $"No available logs in storage for channel '{channelName}'");
                    return null;
                }

                // Process the results
                var batchId = Guid.NewGuid().ToString();
                ProcessLogIds(channelName, batchId, idPairs);
                logs?.AddRange(retrievedLogs);
                return batchId;
            }
            catch (StorageException)
            {
                throw new StorageException("Error retrieving logs from storage");
            }
            finally
            {
                StopTask();
            }
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
            MobileCenterLog.Debug(MobileCenterLog.LogTag, message);
        }
       
        private async Task InitializeDatabaseAsync()
        {
            // The mutex should already be owned and the task should be started
            try
            {
                await _storageAdapter.CreateTableAsync<LogEntry>().ConfigureAwait(false);
            }
            catch (SQLiteException e)
            {
                var storageException = new StorageException("Failed to initialize storage", e);
                MobileCenterLog.Error(MobileCenterLog.LogTag, "An error occurred in storage", storageException);
            }
            finally
            {
                StopTask();
            }
        }

        /// <summary>
        /// Marks the start of a storage operation that can be waited on by <see cref="Shutdown"/>
        /// </summary>
        /// <exception cref="StorageException"><see cref="Shutdown"/> has previously been called</exception>
        private void StartTask()
        {
            lock (_taskLock)
            {
                if (_stopAddingOperations)
                {
                    throw new StorageException("Trying to execute task after shutdown requested");
                }
                _numTasksRemaning++;
                if (_taskSem.CurrentCount == 1)
                {
                    _taskSem.Wait();
                }
            }
            _mutex.Wait();
        }

        private async Task StartTaskAsync()
        {
            await Task.Factory.StartNew(StartTask);
        }

        /// <summary>
        /// Marks the end of a storage operation that can be waited on by <see cref="Shutdown"/>
        /// </summary>
        private void StopTask()
        {
            _mutex.Release();
            lock (_taskLock)
            {
                _numTasksRemaning--;
                if (_numTasksRemaning == 0 && _stopAddingOperations && _taskSem.CurrentCount == 0)
                {
                    _taskSem.Release();
                }
            }
        }

        /// <summary>
        /// Waits for any running storage operations to complete and prevents subsequent storage operations from running
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for remaining tasks</param>
        /// <returns>True if remaining tasks completed in time; false otherwise</returns>
        /// <remarks>This method blocks the calling thread</remarks>
        public bool Shutdown(TimeSpan timeout)
        {
            lock (_taskLock)
            {
                _stopAddingOperations = true;
            }
            return _taskSem.Wait(timeout);
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

        /// <summary>
        /// Disposes the storage object
        /// </summary>
        public void Dispose()
        {
            _mutex.Dispose();
            _taskSem.Dispose();
        }
    }
}
