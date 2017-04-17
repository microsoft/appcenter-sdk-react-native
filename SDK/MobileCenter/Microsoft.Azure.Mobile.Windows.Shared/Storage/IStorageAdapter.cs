using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Storage
{
    public interface IStorageAdapter
    {
        Task<List<T>> GetAsync<T>(Expression<Func<T, bool>> pred, int limit) where T : new();
        Task CreateTableAsync<T>() where T : new();
        Task<int> CountAsync<T>(Expression<Func<T, bool>> pred) where T : new();
        Task<int> InsertAsync<T>(T val) where T : new();
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> pred) where T : new();
        Task<int> DeleteAsync<T>(T val) where T : new();
    }
}
