using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

using Microsoft.Azure.Mobile.UWP.Ingestion.Models;

namespace Microsoft.Azure.Mobile.UWP.Storage
{
    //TODO error handling
    //TODO thread safety. Also, need to find out if sqlite is thread safe. either way, need mutex
    //TODO 
    public class Storage : IStorage
    {
        private const string Database = "Microsoft.Azure.Mobile.UWP.Storage";
        private const string Table = "logs";
        private const string ChannelColumn = "channel";
        private const string LogColumn = "log";
        private SqliteConnection _dbConnection;
        private Dictionary<string, List<long>> _pendingDbIdentifierGroups = new Dictionary<string, List<long>>();
        private HashSet<long> _pendingDbIdentifiers = new HashSet<long>();

        public Storage()
        {
            _dbConnection = new SqliteConnection("DATA SOURCE=" + Database);
            InitializeDatabaseAsync().Start();
        }

        //Group = column (crashes or analytics)
        //id = batch id
        public async Task PutLogAsync(string channelName, ILog log)
        {
            string logJsonString = MiscStubs.WriteFromObject(log);
            await _dbConnection.OpenAsync();
            try
            {
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
                _dbConnection.Close();
            }

            //TODO throw exception on failure
        }
        public async Task DeleteLogsAsync(string channelName, string batchId)
        {
            //TODO throw exceptions on failure

            await _dbConnection.OpenAsync();
            try
            {
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting logs from the Storage database for " + channelName + " with " + batchId);
                MobileCenterLog.Debug(MobileCenterLog.LogTag, "The IDs for deleting log(s) is/are:");

                var identifiers = _pendingDbIdentifierGroups[channelName + batchId];
                _pendingDbIdentifierGroups.Remove(channelName + batchId);
                foreach (long id in identifiers)
                {
                    //TODO use a prepared statement
                    string commandString = string.Format("DELETE FROM {0} WHERE ROWID={1}", Table, id);
                    SqliteCommand command = new SqliteCommand(commandString, _dbConnection);
                    MobileCenterLog.Debug(MobileCenterLog.LogTag, "\t" + id);
                    _pendingDbIdentifiers.Remove(id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            finally
            {
                _dbConnection.Close();
            }
        }
        public async Task DeleteLogsAsync(string channelName)
        {
            //TODO throw exception on failure

            await _dbConnection.OpenAsync();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting all logs from the Storage database for " + channelName);
            try
            {
                List<string> fullIdentifiers = new List<string>();
                foreach (string fullIdentifier in _pendingDbIdentifierGroups.Keys)
                {
                    //TODO fix issue where a channel can't start with the full name of another
                    if (fullIdentifier.StartsWith(channelName))
                    {
                        foreach (long id in _pendingDbIdentifierGroups[fullIdentifier])
                        {
                            _pendingDbIdentifiers.Remove(id); //TODO what if it is already gone? we crash
                        }
                        fullIdentifiers.Add(fullIdentifier);
                    }
                }
                foreach (string fullIdentifier in fullIdentifiers)
                {
                    _pendingDbIdentifierGroups.Remove(fullIdentifier);
                }
                //TODO use a prepared statement
                string commandString = string.Format("DELETE FROM {0} WHERE {1}={2}", Table, ChannelColumn, channelName);
                SqliteCommand command = new SqliteCommand(commandString, _dbConnection);
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                _dbConnection.Close();
            }
        }
        public async Task<int> CountLogs(string channelName)
        {
            await _dbConnection.OpenAsync();
            MobileCenterLog.Debug(MobileCenterLog.LogTag, "Deleting all logs from the Storage database for " + channelName);
            try
            {
                //TODO use a prepared statement
                string commandString = string.Format("SELECT COUNT(ROWID) FROM {0} WHERE {1}={2}", Table, ChannelColumn, channelName);
                SqliteCommand command = new SqliteCommand(commandString, _dbConnection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();
                return reader.GetInt32(0);
            }
            finally
            {
                _dbConnection.Close();
            }
        }
        public void ClearPendingLogState(string channelName)
        {
            _pendingDbIdentifierGroups.Clear();
            _pendingDbIdentifiers.Clear();
        }
        public async Task<string> GetLogsAsync(string channelName, int limit, List<ILog> logs)
        {
            //TODO throws exception on failure

            await _dbConnection.OpenAsync();
            try
            {
                //TODO use a prepared statement
                string commandString = string.Format("SELECT {0} FROM {1} WHERE {1}={2} LIMIT {4}", Table, ChannelColumn, channelName, limit);

            }
            finally
            {
                _dbConnection.Close();
            }
        }
        public void WaitForCurrentTasksToComplete(long timeout)
        {

        }

        private async Task InitializeDatabaseAsync()
        {
            await _dbConnection.OpenAsync();
            try
            {
                //TODO use a prepared statement
                string commandString = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1} TEXT, {2} TEXT)", Table, ChannelColumn, LogColumn);
                SqliteCommand command = new SqliteCommand(commandString, _dbConnection);
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                _dbConnection.Close();
            }
        }
    }
}
