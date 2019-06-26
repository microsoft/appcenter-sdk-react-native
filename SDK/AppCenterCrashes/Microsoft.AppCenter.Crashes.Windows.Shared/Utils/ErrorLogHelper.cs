// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AppCenter.Crashes.Ingestion.Models;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using Microsoft.AppCenter.Utils;
using Microsoft.AppCenter.Utils.Files;

using ModelException = Microsoft.AppCenter.Crashes.Ingestion.Models.Exception;

namespace Microsoft.AppCenter.Crashes.Utils
{
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
        public const string ErrorStorageDirectoryName = "Errors";

        /// <summary>
        /// Device information utility. Exposed for testing purposes only.
        /// </summary>
        internal IDeviceInformationHelper _deviceInformationHelper;

        /// <summary>
        /// Process information utility. Exposed for testing purposes only.
        /// </summary>
        internal IProcessInformation _processInformation;

        /// <summary>
        /// Directory containing crashes files. Exposed for testing purposes only.
        /// </summary>
        internal Directory _crashesDirectory;

        /// <summary>
        /// Static lock object.
        /// </summary>
        private readonly static object LockObject = new object();

        private static ErrorLogHelper _instanceField;

        /// <summary>
        /// Singleton instance. Should only be accessed from unit tests.
        /// </summary>
        internal static ErrorLogHelper Instance
        {
            get
            {
                lock (LockObject)
                {
                    return _instanceField ?? (_instanceField = new ErrorLogHelper());
                }
            }
            set
            {
                lock (LockObject)
                {
                    _instanceField = value;
                }
            }
        }

        /// <summary>
        /// Public constructor for testing purposes.
        /// </summary>
        public ErrorLogHelper()
        {
            _deviceInformationHelper = new DeviceInformationHelper();
            _processInformation = new ProcessInformation();
            var crashesDirectoryLocation = System.IO.Path.Combine(Constants.AppCenterFilesDirectoryPath, ErrorStorageDirectoryName);
            _crashesDirectory = new Directory(crashesDirectoryLocation);
        }

        /// <summary>
        /// Creates an error log for the given exception object.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A new error log instance.</returns>
        public static ManagedErrorLog CreateErrorLog(System.Exception exception) => Instance.InstanceCreateErrorLog(exception);

        /// <summary>
        /// Gets all files with the error log file extension in the error directory.
        /// </summary>
        public static IEnumerable<File> GetErrorLogFiles() => Instance.InstanceGetErrorLogFiles();

        /// <summary>
        /// Gets the most recently modified error log file.
        /// </summary>
        /// <returns>The most recently modified error log file.</returns>
        public static File GetLastErrorLogFile() => Instance.InstanceGetLastErrorLogFile();

        /// <summary>
        /// Gets the error log file with the given ID.
        /// </summary>
        /// <param name="errorId">The ID for the error log.</param>
        /// <returns>The error log file or null if it is not found.</returns>
        public static File GetStoredErrorLogFile(Guid errorId) => Instance.InstanceGetStoredErrorLogFile(errorId);

        /// <summary>
        /// Saves an error log on disk.
        /// </summary>
        /// <param name="errorLog">The error log.</param>
        public static void SaveErrorLogFile(ManagedErrorLog errorLog) => Instance.InstanceSaveErrorLogFile(errorLog);

        /// <summary>
        /// Deletes an error log from disk.
        /// </summary>
        /// <param name="errorId">The ID for the error log.</param>
        public static void RemoveStoredErrorLogFile(Guid errorId) => Instance.InstanceRemoveStoredErrorLogFile(errorId);

        /// <summary>
        /// Removes all stored error log files.
        /// </summary>
        public static void RemoveAllStoredErrorLogFiles() => Instance.InstanceRemoveAllStoredErrorLogFiles();

        private ManagedErrorLog InstanceCreateErrorLog(System.Exception exception)
        {
            return new ManagedErrorLog
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Device = _deviceInformationHelper.GetDeviceInformation(),
                ProcessId = _processInformation.ProcessId ?? 0,
                ProcessName = _processInformation.ProcessName,
                ParentProcessId = _processInformation.ParentProcessId,
                ParentProcessName = _processInformation.ParentProcessName,
                AppLaunchTimestamp = _processInformation.ProcessStartTime,
                Architecture = _processInformation.ProcessArchitecture,
                Fatal = true,
                Exception = CreateModelException(exception)
            };
        }

        private IEnumerable<File> InstanceGetErrorLogFiles()
        {
            lock (LockObject)
            {
                try
                {
                    // Convert to list so enumeration does not occur outside the lock.
                    return _crashesDirectory.EnumerateFiles($"*{ErrorLogFileExtension}").ToList();
                }
                catch (System.Exception ex)
                {
                    AppCenterLog.Error(Crashes.LogTag, "Failed to retrieve error log files.", ex);
                }
                return new List<File>();
            }
        }

        private File InstanceGetLastErrorLogFile()
        {
            File lastErrorLogFile = null;
            lock (LockObject)
            {
                var errorLogFiles = InstanceGetErrorLogFiles();
                if (errorLogFiles == null)
                {
                    return null;
                }
                try
                {
                    foreach (var errorLogFile in errorLogFiles)
                    {
                        if (lastErrorLogFile == null || lastErrorLogFile.LastWriteTime < errorLogFile.LastWriteTime)
                        {
                            lastErrorLogFile = errorLogFile;
                        }
                    }
                    return lastErrorLogFile;
                }
                catch (System.Exception e)
                {
                    AppCenterLog.Error(Crashes.LogTag, "Encountered an unexpected error while retrieving the latest error log.", e);
                }
            }
            return null;
        }

        private File InstanceGetStoredErrorLogFile(Guid errorId)
        {
            return GetStoredFile(errorId, ErrorLogFileExtension);
        }

        public virtual void InstanceSaveErrorLogFile(ManagedErrorLog errorLog)
        {
            var errorLogString = LogSerializer.Serialize(errorLog);
            var fileName = errorLog.Id + ErrorLogFileExtension;
            try
            {
                lock (LockObject)
                {
                    _crashesDirectory.Create();
                }
                _crashesDirectory.CreateFile(fileName, errorLogString);
            }
            catch (System.Exception ex)
            {
                AppCenterLog.Error(Crashes.LogTag, "Failed to save error log.", ex);
                return;
            }
            AppCenterLog.Debug(Crashes.LogTag, $"Saved error log in directory {ErrorStorageDirectoryName} with name {fileName}.");
        }

        private void InstanceRemoveStoredErrorLogFile(Guid errorId)
        {
            lock (LockObject)
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
        }

        public virtual void InstanceRemoveAllStoredErrorLogFiles()
        {
            lock (LockObject)
            {
                AppCenterLog.Debug(Crashes.LogTag, $"Deleting error log directory.");
                try
                {
                    _crashesDirectory.Delete(true);
                }
                catch (System.Exception ex)
                {
                    AppCenterLog.Warn(Crashes.LogTag, $"Failed to delete error log directory.", ex);
                }
            }
            AppCenterLog.Debug(Crashes.LogTag, "Deleted crashes local files.");
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
        private File GetStoredFile(Guid errorId, string extension)
        {
            var fileName = $"{errorId}{extension}";
            try
            {
                lock (LockObject)
                {
                    return _crashesDirectory.EnumerateFiles(fileName).Single();
                }
            }
            catch (System.Exception ex)
            {
                AppCenterLog.Error(Crashes.LogTag, $"Failed to retrieve error log file {fileName}.", ex);
            }
            return null;
        }
    }
}