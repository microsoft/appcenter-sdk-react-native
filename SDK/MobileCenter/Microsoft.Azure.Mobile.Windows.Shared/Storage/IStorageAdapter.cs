using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Storage
{
    public interface IStorageAdapter
    {
        DbCommand CreateCommand();
        Task<List<Dictionary<string, object>>> ExecuteQueryAsync(DbCommand command);
        Task ExecuteNonQueryAsync(DbCommand command);
        Task OpenAsync();
        void Close();
    }
}
