using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Storage
{
    public interface IStorage : IDisposable
    {
        Task PutLog(string channelName, Log log);
        Task DeleteLogs(string channelName, string batchId);
        Task DeleteLogs(string channelName);
        Task<int> CountLogsAsync(string channelName);
        Task ClearPendingLogState(string channelName);
        Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs);
        Task<bool> ShutdownAsync(TimeSpan timeout);
    }
}
