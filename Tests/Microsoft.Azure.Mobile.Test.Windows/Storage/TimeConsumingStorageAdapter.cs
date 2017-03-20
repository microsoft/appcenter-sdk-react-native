using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Storage;

namespace Microsoft.Azure.Mobile.Test.Windows.Storage
{
    class TimeConsumingStorageAdapter : IStorageAdapter
    {
        public void Close()
        {
        }

        public DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public async Task ExecuteNonQueryAsync(DbCommand command)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(DbCommand command)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            return new List<Dictionary<string, object>>();
        }

        public async Task OpenAsync()
        {
        }
    }
}
