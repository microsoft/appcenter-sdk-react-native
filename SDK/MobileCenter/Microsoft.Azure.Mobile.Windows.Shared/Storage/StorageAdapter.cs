using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Microsoft.Azure.Mobile.Storage
{
    internal class StorageAdapter : IStorageAdapter
    {
        private SQLiteAsyncConnection _dbConnection;

        public StorageAdapter(string databasePath)
        {
            _dbConnection = new SQLiteAsyncConnection(databasePath);
        }

        public async Task<List<T>> GetAsync<T>(Predicate<T> pred, int limit)
        {
            
        }

        public async Task<int> InsertAsync<T>(T val)
        {
            
        }

    }
}
