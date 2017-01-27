using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Storage
{
    public interface IStorage // : IDisposable?
    {
        //Group = column (crashes or analytics)
        //id = batch id
        void PutLog(string groupName, ILog log);
        void DeleteLogs(string groupName, string batchId);
        void DeleteLogs(string groupName);
        int CountLogs(string groupName);
        void ClearPendingLogState();
    }
}
