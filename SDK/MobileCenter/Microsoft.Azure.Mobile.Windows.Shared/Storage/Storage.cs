using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Data.Common;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Storage
{
    internal class Storage : IStorage
    {
        private const string Database = "Microsoft.Azure.Mobile.Storage";
        private const string Table = "logs";
        private const string ChannelColumn = "channel";
        private const string LogColumn = "log";
        private const string RowIdColumn = "rowid";
        private const string DbIdentifierDelimiter = "@";
        private readonly Dictionary<string, List<long>> _pendingDbIdentifierGroups = new Dictionary<string, List<long>>();
        private readonly HashSet<long> _pendingDbIdentifiers = new HashSet<long>();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(0, 1);
        private readonly IStorageAdapter _storageAdapter;

        public Storage() : this(new StorageAdapter(Database))
        {
        }

        public Storage(IStorageAdapter storageAdapter)
        {
            _storageAdapter = storageAdapter;
            Task.Run(() => InitializeDatabaseAsync());
        }

        /// <exception cref="StorageException"/>
        public async Task PutLogAsync(string channelName, Log log)
        {
            await OpenDbAsync();
            try
            {
                var logJsonString = LogSerializer.Serialize(log);
                var command = _storageAdapter.CreateCommand();
                var channelParameter = command.CreateParameter();
                channelParameter.ParameterName = "channelName";
                channelParameter.Value = channelName;
                var logParameter = command.CreateParameter();
                logParameter.ParameterName = "log";
                logParameter.Value = logJsonString;
                command.Parameters.Add(channelParameter);
                command.Parameters.Add(logParameter);
                command.CommandText = $"INSERT INTO {Table} ({ChannelColumn}, {LogColumn}) " +
                                      $"VALUES (@{channelParameter.ParameterName}, @{logParameter.ParameterName})";
                command.Prepare();
                await _storageAdapter.ExecuteNonQueryAsync(command);
            }
            catch (DbException e)
            {
                throw new StorageException(e);
            }
            finally
            {
                CloseDb();
            }
        }

        /// <exception cref="StorageException"/>
        public async Task DeleteLogsAsync(string channelName, string batchId)
        {
            await OpenDbAsync();
            try
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"Deleting logs from the Storage database for {channelName} with {batchId}");
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "The IDs for deleting log(s) is/are:");
                var identifiers = _pendingDbIdentifierGroups[GetFullIdentifier(channelName, batchId)];
                _pendingDbIdentifierGroups.Remove(GetFullIdentifier(channelName, batchId));
                foreach (var id in identifiers)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "\t" + id);
                    _pendingDbIdentifiers.Remove(id);
                }
                foreach (var id in identifiers)
                {
                    await DeleteLogAsync(channelName, id);
                }
            }
            finally
            {
                CloseDb();
            }
        }

        /// <exception cref="StorageException"/>
        public async Task DeleteLogsAsync(string channelName)
        {
            await OpenDbAsync();
            try
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, $"Deleting all logs from storage for '{channelName}'");
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
                var command = _storageAdapter.CreateCommand();
                var channelParameter = command.CreateParameter();
                channelParameter.ParameterName = "channelName";
                channelParameter.Value = channelName;
                command.Parameters.Add(channelParameter);
                command.CommandText = $"DELETE FROM {Table} WHERE {ChannelColumn}=@{channelParameter.ParameterName}";
                command.Prepare();
                await command.ExecuteNonQueryAsync();
            }
            catch (DbException e)
            {
                throw new StorageException("Error deleting logs", e);
            }
            finally
            {
                CloseDb();
            }
        }

        /// <exception cref="StorageException"/>
        private async Task DeleteLogAsync(string channelName, long rowId)
        {
            /* We should have an open connection already */
            var command = _storageAdapter.CreateCommand();
            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "id";
            idParameter.Value = rowId;
            command.Parameters.Add(idParameter);
            command.CommandText = $"DELETE FROM {Table} WHERE {RowIdColumn}=@{idParameter.ParameterName}";
            command.Prepare();
            try
            {
                await _storageAdapter.ExecuteNonQueryAsync(command);
            }
            catch (DbException e)
            {
                throw new StorageException($"Error deleting log from channel {channelName} with id {rowId}", e);
            }
        }

        /// <exception cref="StorageException"/>
        public async Task<int> CountLogsAsync(string channelName)
        {
            await OpenDbAsync();
            const string errorMessage = "Error counting logs";
            try
            {
                var countResultName = "NumberOfLogs";
                var command = _storageAdapter.CreateCommand();
                var channelParameter = new SqliteParameter("channel", channelName);
                command.Parameters.Add(channelParameter);
                command.CommandText =
                    $"SELECT COUNT(*) AS {countResultName} FROM {Table} WHERE {ChannelColumn}=@{channelParameter.ParameterName}";
                command.Prepare();
                var results = await _storageAdapter.ExecuteQueryAsync(command);
                return Convert.ToInt32(results[0][countResultName]);
            }
            catch (DbException e)
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
                CloseDb();
            }
        }

        public async Task ClearPendingLogStateAsync(string channelName)
        {
            await _mutex.WaitAsync();
            _pendingDbIdentifierGroups.Clear();
            _pendingDbIdentifiers.Clear();
            _mutex.Release();
        }

        /// <exception cref="StorageException"/>
        public async Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            await OpenDbAsync();
            logs?.Clear();
            var retrievedLogs = new List<Log>();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, $"Trying to get up to {limit} logs from the database for {channelName}");
            try
            {
                /* Create the query */
                var command = _storageAdapter.CreateCommand();
                var channelParameter = command.CreateParameter();
                channelParameter.ParameterName = "channelName";
                channelParameter.Value = channelName;
                var limitParameter = command.CreateParameter();
                limitParameter.ParameterName = "limit";
                limitParameter.Value = limit;
                command.Parameters.Add(channelParameter);
                command.Parameters.Add(limitParameter);
                command.CommandText =
                    $"SELECT {RowIdColumn},* FROM {Table} " +
                    $"WHERE {ChannelColumn}=@{channelParameter.ParameterName} " +
                    $"LIMIT @{limitParameter.ParameterName}";
                command.Prepare();

                /* Execute the query */
                var idPairs = new List<Tuple<Guid?, long>>();
                await RetrieveLogsAsync(command, channelName, retrievedLogs, idPairs);
                if (idPairs.Count == 0)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, $"No available logs in the database for channel '{channelName}'");
                    return null;
                }

                /* Process the results */
                var batchId = Guid.NewGuid().ToString();
                ProcessLogIds(channelName, batchId, idPairs);
                logs?.AddRange(retrievedLogs);
                return batchId;
            }
            catch (DbException e)
            {
                throw new StorageException("Error retrieving logs from storage", e);
            }
            finally
            {
                CloseDb();
            }
        }

        private void ProcessLogIds(string channelName, string batchId, IEnumerable<Tuple<Guid?, long>> idPairs)
        {
            var ids = new List<long>();
            var message = "The SID/ID pairs for returning logs are:\n";
            foreach (var idPair in idPairs)
            {
                var sidString = idPair.Item1?.ToString() ?? "(null)";
                message += "\t" + sidString + " / " + idPair.Item2;
                _pendingDbIdentifiers.Add(idPair.Item2);
                ids.Add(idPair.Item2);
            }
            _pendingDbIdentifierGroups.Add(GetFullIdentifier(channelName, batchId), ids);
            MobileCenterLog.Debug(MobileCenterLog.LogTag, message);
        }

        /// <exception cref="StorageException"/>
        private async Task RetrieveLogsAsync(DbCommand command, string channelName, ICollection<Log> retrievedLogs,
            ICollection<Tuple<Guid?, long>> idPairs)
        {
            var failedToDeserializeALog = false;
            var retrievedRows = await _storageAdapter.ExecuteQueryAsync(command);
            foreach (var row in retrievedRows)
            {
                var logJson = row[LogColumn] as string;
                var logId = Convert.ToInt64(row[RowIdColumn]);
                if (_pendingDbIdentifiers.Contains(logId))
                {
                    continue;
                }
                try
                {
                    var log = LogSerializer.DeserializeLog(logJson);
                    retrievedLogs.Add(log);
                    idPairs.Add(Tuple.Create(log.Sid, logId));
                }
                catch (JsonSerializationException e)
                {
                    MobileCenterLog.Error(MobileCenterLog.LogTag, "Cannot deserialize a log in the database", e);
                    failedToDeserializeALog = true;
                    await DeleteLogAsync(channelName, logId);
                }
            }
            if (failedToDeserializeALog)
            {
                MobileCenterLog.Warn(MobileCenterLog.LogTag, "Deleted logs that could not be deserialized");
            }
        }

        private async Task InitializeDatabaseAsync()
        {
            /* The mutex should already be owned */
            await _storageAdapter.OpenAsync();
            try
            {
                var command = _storageAdapter.CreateCommand();
                command.CommandText = $"CREATE TABLE IF NOT EXISTS {Table} ({ChannelColumn} TEXT, {LogColumn} TEXT)";
                await _storageAdapter.ExecuteNonQueryAsync(command);
            }
            catch (DbException e)
            {
                var storageException = new StorageException("Failed to initialize storage", e);
                MobileCenterLog.Error(MobileCenterLog.LogTag, "An error occurred in storage", storageException);
            }
            finally
            {
                CloseDb();
            }
        }

        private async Task OpenDbAsync()
        {
            await _mutex.WaitAsync();
            await _storageAdapter.OpenAsync();
        }

        private void CloseDb()
        {
            _storageAdapter.Close();
            _mutex.Release();
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
    }
}
