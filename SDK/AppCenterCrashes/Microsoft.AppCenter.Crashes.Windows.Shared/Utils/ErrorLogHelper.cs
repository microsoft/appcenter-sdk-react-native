// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using ModelException = Microsoft.AppCenter.Crashes.Ingestion.Models.Exception;

namespace Microsoft.AppCenter.Crashes.Utils
{
    //TODO thread safety (maybe if filehelper is thread safe that would suffice).
    /// <summary>
    /// ErrorLogHelper to help constructing, serializing, and de-serializing locally stored error logs.
    /// </summary>
    public class ErrorLogHelper
    {
        /// <summary>
        /// Error log file extension for the JSON schema.
        /// </summary>
        public const string ErrorLogFileExtension = ".json";

        /// <summary>
        /// Error log directory within application files.
        /// </summary>
        public const string ErrorStorageDirectoryName = "Microsoft.AppCenter.Error";

        /// <summary>
        /// Device information utility. Public for testing purposes only.
        /// </summary>
        public static IDeviceInformationHelper DeviceInformationHelper;

        /// <summary>
        /// Process information utility. Public for testing purposes only.
        /// </summary>
        public static IProcessInformation ProcessInformation;

        /// <summary>
        /// File system utility. Public for testing purposes only.
        /// </summary>
        public static FileHelper FileHelper;

        static ErrorLogHelper()
        {
            DeviceInformationHelper = new DeviceInformationHelper();
            ProcessInformation = new ProcessInformation();
            FileHelper = new FileHelper(ErrorStorageDirectoryName);
        }

        /// <summary>
        /// Creates an error log for the given exception object.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A new error log instance.</returns>
        public static async Task<ManagedErrorLog> CreateErrorLogAsync(System.Exception exception) // TODO make this synchronous
        {
            return new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                //TODO expose synchronous way to get device information. also cache the value for the class.
                Device = await DeviceInformationHelper.GetDeviceInformationAsync(),
                ProcessId = ProcessInformation.ProcessId ?? 0,
                ProcessName = ProcessInformation.ProcessName,
                ParentProcessId = ProcessInformation.ParentProcessId,
                ParentProcessName = ProcessInformation.ParentProcessName,
                AppLaunchTimestamp = ProcessInformation.ProcessStartTime,
                Architecture = ProcessInformation.ProcessArchitecture,
                Fatal = true,
                Exception = CreateModelException(exception)
            };
        }

        /// <summary>
        /// Gets all files with the error log file extension in the error directory.
        /// </summary>
        public static IEnumerable<FileInfo> GetErrorLogFiles()
        {
            try
            {
                return FileHelper.EnumerateFiles($"*{ErrorLogFileExtension}");
            }
            catch (System.Exception ex)
            {
                AppCenterLog.Error(Crashes.LogTag, "Failed to retrieve error log files.", ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the most recently modified error log file.
        /// </summary>
        /// <returns>The most recently modified error log file.</returns>
        public static FileInfo GetLastErrorLogFile()
        {
            FileInfo lastErrorLogFile = null;
            var errorLogFiles = GetErrorLogFiles();
            if (errorLogFiles == null)
            {
                return null;
            }
            foreach (var errorLogFile in errorLogFiles)
            {
                // TODO exception handling. 
                if (lastErrorLogFile == null || lastErrorLogFile.LastWriteTime > errorLogFile.LastWriteTime)
                {
                    lastErrorLogFile = errorLogFile;
                }
            }
            return lastErrorLogFile;
        }

        /// <summary>
        /// Gets the error log file with the given ID.
        /// </summary>
        /// <param name="errorId">The ID for the error log.</param>
        /// <returns>The error log file or null if it is not found.</returns>
        public static FileInfo GetStoredErrorLogFile(Guid errorId)
        {
            return GetStoredFile(errorId, ErrorLogFileExtension);
        }

        /// <summary>
        /// Saves an error log on disk.
        /// </summary>
        /// <param name="errorLog">The error log.</param>
        public static void SaveErrorLogFile(ManagedErrorLog errorLog)
        {
            var errorLogString = LogSerializer.Serialize(errorLog);
            var fileName = errorLog.Id + ErrorLogFileExtension;
            try
            {
                FileHelper.CreateFile(fileName, errorLogString);
            }
            catch (System.Exception ex)
            {
                AppCenterLog.Error(Crashes.LogTag, "Failed to save error log.", ex);
                return;
            }
            AppCenterLog.Debug(Crashes.LogTag, $"Saved error log in directory {ErrorStorageDirectoryName} with name {fileName}.");
        }

        /// <summary>
        /// Deletes an error log from disk.
        /// </summary>
        /// <param name="errorId">The ID for the error log.</param>
        public static void RemoveStoredErrorLogFile(Guid errorId)
        {
            var file = GetStoredErrorLogFile(errorId);
            if (file != null)
            {
                AppCenterLog.Info(Crashes.LogTag, $"Deleting error log file {file.Name}.");
                try
                {
                    file.Delete();
                }
                catch (System.Exception ex)
                {
                    AppCenterLog.Warn(Crashes.LogTag, $"Failed to delete error log file {file.Name}.", ex);
                }
            }
        }

        private static ModelException CreateModelException(System.Exception exception)
        {
            var modelException = new ModelException
            {
                Type = exception.GetType().ToString(),
                Message = exception.Message,
                StackTrace = exception.StackTrace
            };
            if (exception is AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Count != 0)
                {
                    modelException.InnerExceptions = new List<ModelException>();
                    foreach (var innerException in aggregateException.InnerExceptions)
                    {
                        modelException.InnerExceptions.Add(CreateModelException(innerException));
                    }
                }
            }
            if (exception.InnerException != null)
            {
                modelException.InnerExceptions = modelException.InnerExceptions ?? new List<ModelException>();
                modelException.InnerExceptions.Add(CreateModelException(exception.InnerException));
            }
            return modelException;
        }

        /// <summary>
        /// Gets a file from storage with the given ID and extension.
        /// </summary>
        /// <param name="errorId">The error ID.</param>
        /// <param name="extension">The file extension.</param>
        /// <returns>The file corresponding to the given parameters, or null if not found.</returns>
        private static FileInfo GetStoredFile(Guid errorId, string extension)
        {
            var fileName = $"{errorId}{extension}";
            try
            {
                return FileHelper.EnumerateFiles(fileName).SingleOrDefault();
            }
            catch (System.Exception ex)
            {
                AppCenterLog.Error(Crashes.LogTag, $"Failed to retrieve error log file {fileName}.", ex);
            }
            return null;
        }
    }
}