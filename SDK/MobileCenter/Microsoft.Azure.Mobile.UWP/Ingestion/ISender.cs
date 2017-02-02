using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Ingestion
{
    public class SenderException : Exception { }
    public interface ISender
    {
        Task SendLogsAsync(string appSecret, Guid installId, IEnumerable<ILog> logs);
        void Close();
    }

}
