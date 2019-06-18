using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public class StorageOperationsHelper : IStorageOperationsHelper
    {
        /// <summary>
        /// Error log file extension for the JSON schema.
        /// </summary>
        private const string ErrorLogFileExtension = ".json";

        /// <summary>
        /// Error log directory within application files.
        /// </summary>
        private const string ErrorStorageDirectoryName = "Microsoft.AppCenter.Error";

        /// <summary>
        /// File system utility. Public for testing purposes only.
        /// </summary>
        public static FileHelper FileHelper;

        /// <summary>
        /// Static lock object.
        /// </summary>
        private readonly static object LockObject = new object();

        public StorageOperationsHelper()
        {
            FileHelper = new FileHelper(ErrorStorageDirectoryName);
        }

        /// <summary>
        /// Saves an error log on disk.
        /// </summary>
        /// <param name="errorLog">The error log.</param>
        public void SaveErrorLogFile(ManagedErrorLog errorLog)
        {
            var errorLogString = LogSerializer.Serialize(errorLog);
            var fileName = errorLog.Id + ErrorLogFileExtension;
            try
            {
                lock (LockObject)
                {
                    FileHelper.CreateFile(fileName, errorLogString);
                }
            }
            catch (System.Exception ex)
            {
                AppCenterLog.Error(Crashes.LogTag, "Failed to save error log.", ex);
                return;
            }
            AppCenterLog.Debug(Crashes.LogTag, $"Saved error log in directory {ErrorStorageDirectoryName} with name {fileName}.");
        }
    }
}
