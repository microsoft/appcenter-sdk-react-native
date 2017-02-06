using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Data.Sqlite;

using Microsoft.Azure.Mobile.UWP.Ingestion.Models;

namespace Microsoft.Azure.Mobile.UWP.Storage
{
    //TODO error handling (especially around statements that execute commands)
    public class Storage : IStorage
    {
        private const string Database = "Microsoft.Azure.Mobile.UWP.Storage";
        private const string Table = "logs";
        private const string ChannelColumn = "channel";
        private const string LogColumn = "log";
        private Dictionary<string, List<long>> _pendingDbIdentifierGroups = new Dictionary<string, List<long>>();
        private HashSet<long> _pendingDbIdentifiers = new HashSet<long>();
        private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        public Storage()
        {
            InitializeDatabaseAsync().Start();
        }
        public async Task PutLogAsync(string channelName, ILog log)
        {
            string logJsonString = MiscStubs.WriteFromObject(log);
            SqliteConnection dbConnection = GetDbConnection();
            await dbConnection.OpenAsync();
            try
            {
                var command = new SqliteCommand(null, dbConnection);
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
                dbConnection.Close();
            }

            //TODO throw exception on failure
        }
        public async Task DeleteLogsAsync(string channelName, string batchId)
        {
            //TODO throw exceptions on failure
            SqliteConnection dbConnection = GetDbConnection();
            await dbConnection.OpenAsync();
            try
            {
                await _mutex.WaitAsync();
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting logs from the Storage database for " + channelName + " with " + batchId);
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "The IDs for deleting log(s) is/are:");
                var identifiers = _pendingDbIdentifierGroups[channelName + batchId];
                _pendingDbIdentifierGroups.Remove(channelName + batchId);
                foreach (long id in identifiers)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "\t" + id);
                    _pendingDbIdentifiers.Remove(id);
                }
                _mutex.Release();
                foreach (long id in identifiers)
                {
                    await DeleteLogAsync(channelName, id);
                }
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public async Task DeleteLogsAsync(string channelName)
        {
            //TODO throw exception on failure
            SqliteConnection dbConnection = GetDbConnection();
            await dbConnection.OpenAsync();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting all logs from the Storage database for " + channelName);
            try
            {
                List<string> fullIdentifiers = new List<string>();
                await _mutex.WaitAsync();
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
                _mutex.Release();

                var command = new SqliteCommand(null, dbConnection);
                command.CommandText = string.Format("DELETE FROM {0} WHERE {1}=@channel", Table, ChannelColumn);
                var channelParameter = new SqliteParameter("channel", channelName);
                command.Parameters.Add(channelParameter);
                command.Prepare();
                await command.ExecuteNonQueryAsync();

            }
            finally
            {
                dbConnection.Close();
            }
        }
        public async Task<int> CountLogs(string channelName)
        {
            SqliteConnection dbConnection = GetDbConnection();
            await dbConnection.OpenAsync();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting all logs from the Storage database for " + channelName);
            try
            {
                var command = new SqliteCommand(null, dbConnection);
                command.CommandText = string.Format("SELECT COUNT(ROWID) FROM {0} WHERE {1}=@channel", Table, ChannelColumn);
                var channelParameter = new SqliteParameter("channel", channelName);
                command.Parameters.Add(channelParameter);
                command.Prepare();
                SqliteDataReader reader = await command.ExecuteReaderAsync();
                return reader.GetInt32(0);
            }
            finally
            {
                dbConnection.Close();
                _mutex.Release();
            }
        }
        public void ClearPendingLogState(string channelName)
        {
            _mutex.Wait();
            _pendingDbIdentifierGroups.Clear();
            _pendingDbIdentifiers.Clear();
            _mutex.Release();
        }
        public async Task<string> GetLogsAsync(string channelName, int limit, List<ILog> logs) //TODO see if this can be broken up into smaller pieces
        {
            //TODO throw exception on failure
            SqliteConnection dbConnection = GetDbConnection();
            await dbConnection.OpenAsync();
            logs.Clear();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Trying to get up to " + limit + " logs from the database for " + channelName);
            string batchId = (new Guid()).ToString();
            try
            {
                /* Save ids as a 2-tuple (SId, RowId) */
                var idPairs = new List<Tuple<long, long>>();
                var command = new SqliteCommand(null, dbConnection);
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
                    ILog log;
                    try
                    {
                        log = MiscStubs.ReadToObject<ILog>(logJson);
                        logs.Add(log);
                        idPairs.Add(Tuple.Create(log.SId, reader.GetInt64(0)));
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
                await _mutex.WaitAsync();
                foreach (Tuple<long, long> idPair in idPairs)
                {
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "\t" + idPair.Item1 + " / " + idPair.Item2);
                    _pendingDbIdentifiers.Add(idPair.Item2);
                    ids.Add(idPair.Item2);
                }
                _pendingDbIdentifierGroups.Add(channelName + batchId, ids);
                _mutex.Release();
            }
            catch
            {
                logs.Clear();
                throw;
            }
            finally
            {
                dbConnection.Close();
            }
            return batchId;
        }
        private async Task InitializeDatabaseAsync()
        {
            SqliteConnection dbConnection = GetDbConnection();
            await dbConnection.OpenAsync();
            try
            {
                string commandString = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1} TEXT, {2} TEXT)", Table, ChannelColumn, LogColumn);
                SqliteCommand command = new SqliteCommand(commandString, dbConnection);
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                dbConnection.Close();
            }
        }
        private async Task DeleteLogAsync(string channelName, long rowId)
        {
            SqliteConnection dbConnection = GetDbConnection();
            await dbConnection.OpenAsync();
            try
            {
                var command = new SqliteCommand(null, dbConnection);
                command.CommandText = string.Format("DELETE FROM {0} WHERE ROWID=@id", Table);
                var idParameter = new SqliteParameter("id", rowId);
                command.Parameters.Add(idParameter);
                command.Prepare();
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                dbConnection.Close();
            }
        }
        private SqliteConnection GetDbConnection()
        {
            return new SqliteConnection("DATA SOURCE=" + Database);
        }
    }
}
