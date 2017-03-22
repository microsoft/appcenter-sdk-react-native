using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Microsoft.Azure.Mobile.Storage
{
    /// <summary>
    /// Implementation of IStorageAdapter that uses <see cref="Microsoft.Data.Sqlite"/>
    /// </summary>
    /// <remarks><see cref="Microsoft.Data.Sqlite"/> doesn't actually support asynchronicity, so the methods are actually executed synchronously. To mitigate this, Write-Ahead Logging is used.</remarks>
    public sealed class StorageAdapter : IStorageAdapter
    {
        private readonly DbConnection _dbConnection;
        private bool _walEnabled;

        /// <summary>
        /// Creates an instance of the Storage Adapter
        /// </summary>
        /// <param name="databaseName"></param>
        public StorageAdapter(string databaseName)
        {
            _dbConnection = new SqliteConnection($"DATA SOURCE={databaseName}");
        }

        /// <summary>
        /// Creates a DbComand associated with this adapter
        /// </summary>
        /// <returns>The requested command</returns>
        public DbCommand CreateCommand()
        {
            return _dbConnection.CreateCommand();
        }

        /// <summary>
        /// Asynchronously executes and reads the results of a query command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>The results in the form of a dictionary</returns>
        /// <exception cref="DbException"/>
        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(DbCommand command)
        {
            var rows = new List<Dictionary<string, object>>();

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    var row = new Dictionary<string, object>();
                    for (var i = 0; i < reader.FieldCount; ++i)
                    {
                        var columnName = reader.GetName(i);
                        var columnValue = reader.GetValue(i);
                        row.Add(columnName, columnValue);
                    }
                    rows.Add(row);
                }
            }
            return rows;
        }

        /// <summary>
        /// Asyncrhonously executes a non-query command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <exception cref="DbException"/>
        public async Task ExecuteNonQueryAsync(DbCommand command)
        {
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously opens a connection to the database
        /// </summary>
        public async Task OpenAsync()
        {
            await _dbConnection.OpenAsync().ConfigureAwait(false);
            EnableWal(); // Enable WAL in case it hasn't been enabled already
        }

        /// <summary>
        /// Asynchronously closes a connection to the database
        /// </summary>
        /// <exception cref="DbException"/>
        public void Close()
        {
            _dbConnection.Close();
        }

        /// <summary>
        /// Disposes the adapter
        /// </summary>
        public void Dispose()
        {
            _dbConnection.Dispose();
        }

        // Write-Ahead Logging (WAL) in SQLite: http://www.sqlite.org/draft/wal.html
        private void EnableWal()
        {
            if (_walEnabled)
            {
                return;
            }

            // Note that this doesn't work with the current SQLite package. https://github.com/aspnet/Microsoft.Data.Sqlite/issues/337
            var command = _dbConnection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=WAL";
            command.ExecuteScalar();
            _walEnabled = true;
        }
    }
}
