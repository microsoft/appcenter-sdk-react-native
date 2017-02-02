using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.UWP.Ingestion
{
    public class IngestionException : Exception { }
    public interface IIngestion
    {
        Task SendLogsAsync(string appSecret, Guid installId, IEnumerable<ILog> logs);
        void Close();

        void SetServerUrl(string serverUrl); //TODO should this be a property?
    }

}
