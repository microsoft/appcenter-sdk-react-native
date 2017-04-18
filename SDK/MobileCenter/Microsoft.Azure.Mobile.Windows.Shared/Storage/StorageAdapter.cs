using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Microsoft.Azure.Mobile.Storage
{
    internal class StorageAdapter : IStorageAdapter
    {
        private readonly SQLiteAsyncConnection _dbConnection;

        public StorageAdapter(string databasePath)
        {
            _dbConnection = new SQLiteAsyncConnection(databasePath);
        }

        public async Task CreateTableAsync<T>() where T : new()
        {
            try
            {
                await _dbConnection.CreateTableAsync<T>().ConfigureAwait(false);
            }
            catch (SQLiteException e)
            {
                throw new StorageException(e);
            }
        }
        public async Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> pred, int limit) where T : new()
        {
            try
            {
                var table = _dbConnection.Table<T>();
                return await table.Where(pred).Take(limit).ToListAsync().ConfigureAwait(false);
            }
            catch (SQLiteException e)
            {
                throw new StorageException(e);
            }

        }

        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> pred) where T : new()
        {
            var table = _dbConnection.Table<T>();
            return await table.Where(pred).CountAsync().ConfigureAwait(false);
        }

        public async Task<int> InsertAsync<T>(T val) where T : new()
        {
            try
            {
                return await _dbConnection.InsertAsync(val).ConfigureAwait(false);
            }
            catch (SQLiteException e)
            {
                throw new StorageException(e);
            }
        }

        public async Task<int> DeleteAsync<T>(Expression<Func<T, bool>> pred) where T : new()
        {
            try
            {
                var numDeleted = 0;
                var table = _dbConnection.Table<T>();
                var entries = await table.Where(pred).ToListAsync().ConfigureAwait(false);
                foreach (var entry in entries)
                {
                    numDeleted += await _dbConnection.DeleteAsync(entry).ConfigureAwait(false);
                }
                return numDeleted;
            }
            catch (SQLiteException e)
            {
                throw new StorageException(e);
            }
        }
    }
}
