using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Storage;

namespace Microsoft.AppCenter.Test.Storage
{
    public class MockStorage : IStorage
    {
        public Task ClearPendingLogState(string channelName)
        {
            return TaskExtension.GetCompletedTask();
        }

        public Task<int> CountLogsAsync(string channelName)
        {
            return TaskExtension.GetCompletedTask(1);
        }

        public Task DeleteLogs(string channelName)
        {
            return TaskExtension.GetCompletedTask();
        }

        public Task DeleteLogs(string channelName, string batchId)
        {
            return TaskExtension.GetCompletedTask();
        }

        public Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs)
        {
            return TaskExtension.GetCompletedTask(Guid.NewGuid().ToString());
        }

        public Task PutLog(string channelName, Log log)
        {
            return TaskExtension.GetCompletedTask();
        }

        public Task<bool> ShutdownAsync(TimeSpan timeout)
        {
            return TaskExtension.GetCompletedTask(true);
        }

        public void Dispose()
        {
        }
    }
}
