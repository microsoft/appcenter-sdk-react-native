using Microsoft.Azure.Mobile.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

namespace Microsoft.Azure.Mobile.Test.Windows.Storage
{
    public class TestDbException : DbException
    {
        public TestDbException(string message)
            : base(message)
        {

        }
    }

    public class TestStorageAdapter : IStorageAdapter
    {
        public bool IsSuccessQuery { get; set; } = true;
        public bool IsDisposed { get; private set; } = false;

        private StorageAdapter adapter;

        public TestStorageAdapter()
        {
            adapter = new StorageAdapter("testDB");
        }

        public void Close()
        {
        }

        public DbCommand CreateCommand()
        {
            if (!IsSuccessQuery)
                throw new TestDbException("CreateCommand");
            return adapter.CreateCommand();
        }

        public void Dispose()
        {
            if (!IsSuccessQuery)
                throw new TestDbException("Dispose"); ;
            IsDisposed = true;
        }

        public Task ExecuteNonQueryAsync(DbCommand command)
        {
            return IsSuccessQuery ? TaskExtension.GetCompletedTask() : TaskExtension.GetFaultedTask();
        }

        public Task<List<Dictionary<string, object>>> ExecuteQueryAsync(DbCommand command)
        {
            return IsSuccessQuery
                ? TaskExtension.GetCompletedTask(new List<Dictionary<string, object>>())
                : TaskExtension.GetFaultedTask(new List<Dictionary<string, object>>());
        }

        public Task OpenAsync()
        {
            return TaskExtension.GetCompletedTask();
        }
    }
}
