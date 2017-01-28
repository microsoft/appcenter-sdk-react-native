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
        void PutLog(string channelName, ILog log);
        void DeleteLogs(string channelName, string batchId);
        void DeleteLogs(string channelName);
        int CountLogs(string channelName);
        void ClearPendingLogState();
        string GetLogs(string channelName, int limit, out List<ILog> outLogs);
    }
}
