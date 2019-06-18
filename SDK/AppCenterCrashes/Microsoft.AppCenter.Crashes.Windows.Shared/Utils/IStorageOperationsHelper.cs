using Microsoft.AppCenter.Crashes.Ingestion.Models;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public interface IStorageOperationsHelper
    {
        void SaveErrorLogFile(ManagedErrorLog errorLog);
    }
}
