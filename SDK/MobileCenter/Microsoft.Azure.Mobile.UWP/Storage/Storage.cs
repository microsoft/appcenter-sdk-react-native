using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Data.Sqlite;

using Microsoft.Azure.Mobile.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.Mobile.Storage
{
    //TODO error handling (especially around statements that execute commands)
    public class Storage : IStorage
    {
        private const string Database = "Microsoft.Azure.Mobile.Storage";
        private const string Table = "logs";
        private const string ChannelColumn = "channel";
        private const string LogColumn = "log";
        private Dictionary<string, List<long>> _pendingDbIdentifierGroups = new Dictionary<string, List<long>>();
        private HashSet<long> _pendingDbIdentifiers = new HashSet<long>();
        private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private SqliteConnection _dbConnection;
        public Storage()
        {
            _mutex.Wait();
            _dbConnection = new SqliteConnection("DATA SOURCE=" + Database);
            _mutex.Release();
            Task.Run(() => InitializeDatabaseAsync());
        }
        public async Task PutLogAsync(string channelName, Log log)
        {
            await OpenDbAsync();
            try
            {
                string logJsonString = LogSerializer.Serialize(log);
                var command = new SqliteCommand(null, _dbConnection);
                command.CommandText = string.Format("INSERT INTO {0} ({1}, {2}) VALUES (@channelName, @log)", Table, ChannelColumn, LogColumn);
                var channelParameter = new SqliteParameter("channelName", channelName);
                var logParameter = new SqliteParameter("log", logJsonString);
                command.Parameters.Add(channelParameter);
                command.Parameters.Add(logParameter);
                command.Prepare();
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                CloseDb();
            }

            //TODO throw exception on failure
        }
        public async Task DeleteLogsAsync(string channelName, string batchId)
        {
            //TODO throw exceptions on failure
            await OpenDbAsync();
            try
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting logs from the Storage database for " + channelName + " with " + batchId);
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "The IDs for deleting log(s) is/are:");
                var identifiers = _pendingDbIdentifierGroups[channelName + batchId];
                _pendingDbIdentifierGroups.Remove(channelName + batchId);
                foreach (long id in identifiers)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "\t" + id);
                    _pendingDbIdentifiers.Remove(id);
                }
                foreach (long id in identifiers)
                {
                    await DeleteLogAsync(channelName, id);
                }
            }
            finally
            {
                CloseDb();
            }
        }
        public async Task DeleteLogsAsync(string channelName)
        {
            //TODO throw exception on failure
            await OpenDbAsync();
            try
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting all logs from the Storage database for " + channelName);
                await DeleteLogsAsyncHelper(channelName);
            }
            finally
            {
                CloseDb();
            }
        }
        private async Task DeleteLogsAsyncHelper(string channelName)
        {
            //TODO throw exception on failure
            List<string> fullIdentifiers = new List<string>();
            foreach (string fullIdentifier in _pendingDbIdentifierGroups.Keys)
            {
                //TODO fix issue where a channel can't start with the full name of another
                if (fullIdentifier.StartsWith(channelName))
                {
                    foreach (long id in _pendingDbIdentifierGroups[fullIdentifier])
                    {
                        _pendingDbIdentifiers.Remove(id);
                    }
                    fullIdentifiers.Add(fullIdentifier);
                }
            }
            foreach (string fullIdentifier in fullIdentifiers)
            {
                _pendingDbIdentifierGroups.Remove(fullIdentifier);
            }
            var command = new SqliteCommand(null, _dbConnection);
            command.CommandText = string.Format("DELETE FROM {0} WHERE {1}=@channel", Table, ChannelColumn);
            var channelParameter = new SqliteParameter("channel", channelName);
            command.Parameters.Add(channelParameter);
            command.Prepare();
            await command.ExecuteNonQueryAsync();
        }
        public async Task<int> CountLogs(string channelName)
        {
            await OpenDbAsync();
            try
            {
                var command = new SqliteCommand(null, _dbConnection);
                command.CommandText = string.Format("SELECT COUNT(ROWID) FROM {0} WHERE {1}=@channel", Table, ChannelColumn);
                var channelParameter = new SqliteParameter("channel", channelName);
                command.Parameters.Add(channelParameter);
                command.Prepare();
                SqliteDataReader reader = await command.ExecuteReaderAsync();
                return reader.GetInt32(0);
            }
            finally
            {
                CloseDb();
            }
        }
        public void ClearPendingLogState(string channelName)
        {
            _mutex.Wait();
            _pendingDbIdentifierGroups.Clear();
            _pendingDbIdentifiers.Clear();
            _mutex.Release();
        }
        public async Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs) //TODO see if this can be broken up into smaller pieces
        {
            //TODO throw exception on failure
            await OpenDbAsync();
            logs.Clear();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Trying to get up to " + limit + " logs from the database for " + channelName);
            string batchId = (new Guid()).ToString();
            try
            {
                /* Save ids as a 2-tuple (SId, RowId) */
                var idPairs = new List<Tuple<Guid?, long>>();
                var command = new SqliteCommand(null, _dbConnection);
                command.CommandText = string.Format("SELECT ROWID,* FROM {0} WHERE {1}=@channel LIMIT @limit", Table, ChannelColumn);
                var channelParameter = new SqliteParameter("channel", channelName);
                var limitParameter = new SqliteParameter("limit", limit);
                command.Parameters.Add(channelParameter);
                command.Parameters.Add(limitParameter);
                command.Prepare();
                SqliteDataReader reader = await command.ExecuteReaderAsync();
                bool failedToDeserializeALog = false;
                while (reader.Read()) //TODO should this be ReadAsync?
                {
                    string logJson = reader[LogColumn] as string;
                    //TODO error check?
                    Log log;
                    try
                    {
                        log = LogSerializer.DeserializeLog(logJson);
                        logs.Add(log);
                        idPairs.Add(Tuple.Create(log.Sid, reader.GetInt64(0)));
                    }
                    catch (Exception e) //TODO use a more specific exception?
                    {
                        MobileCenterLog.Error(MobileCenterLog.LogTag, "Cannot deserialize a log in the database", e);
                        failedToDeserializeALog = true;
                        await DeleteLogAsync(channelName, reader.GetInt64(0));
                    }
                }
                if (failedToDeserializeALog)
                {
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, "Deleted logs that could not be deserialized");
                }
                if (idPairs.Count == 0)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "No logs found in the database for channel " + channelName);
                    return null;
                }

                var ids = new List<long>();
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "The SID/ID pairs for returning logs are:");
                foreach (var idPair in idPairs)
                {
                    string sidString = idPair.Item1?.ToString() ?? "(null)";
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "\t" + sidString + " / " + idPair.Item2);
                    _pendingDbIdentifiers.Add(idPair.Item2);
                    ids.Add(idPair.Item2);
                }
                _pendingDbIdentifierGroups.Add(channelName + batchId, ids);
            }
            catch
            {
                logs.Clear();
                throw;
            }
            finally
            {
                CloseDb();
            }
            return batchId;
        }
        private async Task InitializeDatabaseAsync()
        {
            await OpenDbAsync();
            try
            {
                string commandString = $"CREATE TABLE IF NOT EXISTS {Storage.Table} ({Storage.ChannelColumn} TEXT, {Storage.LogColumn} TEXT)";
                SqliteCommand command = new SqliteCommand(commandString, _dbConnection);
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                CloseDb();
            }
        }
        private async Task DeleteLogAsync(string channelName, long rowId)
        {
            /* We should have an open connection already */

                var command = new SqliteCommand(null, _dbConnection);
                command.CommandText = string.Format("DELETE FROM {0} WHERE ROWID=@id", Table);
                var idParameter = new SqliteParameter("id", rowId);
                command.Parameters.Add(idParameter);
                command.Prepare();
                await command.ExecuteNonQueryAsync();
        }

        private async Task OpenDbAsync()
        {
            await _mutex.WaitAsync();
            await _dbConnection.OpenAsync();
        }

        private void CloseDb()
        {
            _dbConnection.Close();
            _mutex.Release();
        }
    }
}
