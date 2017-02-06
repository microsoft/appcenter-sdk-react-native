using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;

namespace Microsoft.Azure.Mobile.UWP.Storage
{
    public interface IStorage // : IDisposable?
    {
        //Group = column (crashes or analytics)
        //id = batch id
        Task PutLogAsync(string channelName, ILog log);
        Task DeleteLogsAsync(string channelName, string batchId);
        Task DeleteLogsAsync(string channelName);
        Task<int> CountLogs(string channelName);
        void ClearPendingLogState(string channelName);
        Task<string> GetLogsAsync(string channelName, int limit, List<ILog> logs);
    }
}
