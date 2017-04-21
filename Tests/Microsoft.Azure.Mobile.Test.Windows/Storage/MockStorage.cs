using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;

namespace Microsoft.Azure.Mobile.Test.Storage
{
    public class MockStorage : IStorage
    {
        public Task ClearPendingLogStateAsync(string channelName)
        {
            return TaskExtension.GetCompletedTask();
        }

        public Task<int> CountLogsAsync(string channelName)
        {
            return TaskExtension.GetCompletedTask(1);
        }

        public Task DeleteLogsAsync(string channelName)
        {
            return TaskExtension.GetCompletedTask();
        }

        public Task DeleteLogsAsync(string channelName, string batchId)
        {
            return TaskExtension.GetCompletedTask();
        }

        public Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            return TaskExtension.GetCompletedTask(Guid.NewGuid().ToString());
        }

        public Task PutLogAsync(string channelName, Log log)
        {
            return TaskExtension.GetCompletedTask();
        }

        public bool Shutdown(TimeSpan timeout)
        {
            return true;
        }

        public void Dispose()
        {
        }
    }
}
