using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Microsoft.Azure.Mobile.Storage
{
    public class StorageAdapter : IStorageAdapter
    {
        private readonly DbConnection _dbConnection;
        public StorageAdapter(string databaseName)
        {
            _dbConnection = new SqliteConnection($"DATA SOURCE={databaseName}");
        }

        public DbCommand CreateCommand()
        {
            return _dbConnection.CreateCommand();
        }

        /// <exception cref="DbException"/>
        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(DbCommand command)
        {
            var rows = new List<Dictionary<string, object>>();

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        string columnName = reader.GetName(i);
                        object columnValue = reader.GetValue(i);
                        row.Add(columnName, columnValue);
                    }
                    rows.Add(row);
                }
            }
            return rows;
        }

        /// <exception cref="DbException"/>
        public async Task ExecuteNonQueryAsync(DbCommand command)
        {
            await command.ExecuteNonQueryAsync();
        }

        public async Task OpenAsync()
        {
            await _dbConnection.OpenAsync();
        }

        /// <exception cref="DbException"/>
        public void Close()
        {
            _dbConnection.Close();
        }
    }
}
