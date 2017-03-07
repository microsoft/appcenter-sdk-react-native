using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile.Storage;

namespace Microsoft.Azure.Mobile.Test.Storage
{
    public class MockStorage : IStorage
    {
        private List<Log> logs = new List<Log>();
        
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
            logs.Add(new TestLog());
            return TaskExtension.GetCompletedTask("hi");
        }

        public Task PutLogAsync(string channelName, Log log)
        {
            return TaskExtension.GetCompletedTask();
        }
    }
}
