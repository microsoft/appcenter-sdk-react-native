using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Ingestion
{
    //this is a dummy class
    public interface ISender
    {
        Task<SendLogsAsyncResult> SendLogsAsync(List<ILog> log);
        void Close();
    }

    public class SendLogsAsyncResult
    {
        public bool Success;
        public Exception Exception;
    }
}
