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
    public partial class ErrorLogHelper
    {
        /// <summary>
        /// Error log file extension for the JSON schema.
        /// </summary>
        public const string ErrorLogFileExtension = ".json";

        /// <summary>
        /// Error log file extension for the serialized exception for client side inspection.
        /// </summary>
        public const string ExceptionFileExtension = ".exception";

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
        /// Gets the error storage directory, or creates it if it does not exist.
        /// </summary>
        /// <returns>The error storage directory.</returns>
        public static Directory GetErrorStorageDirectory() => Instance.InstanceGetErrorStorageDirectory();

        /// <summary>
        /// Gets the error log file with the given ID.
        /// </summary>
        /// <param name="errorId">The ID for the error log.</param>
        /// <returns>The error log file or null if it is not found.</returns>
        public static File GetStoredErrorLogFile(Guid errorId) => Instance.InstanceGetStoredErrorLogFile(errorId);

        /// <summary>
        /// Gets the exception file with the given ID.
        /// </summary>
        /// <param name="errorId">The ID for the exception.</param>
        /// <returns>The exception file or null if it is not found.</returns>
        public static File GetStoredExceptionFile(Guid errorId) => Instance.InstanceGetStoredExceptionFile(errorId);

        /// <summary>
        /// Reads an error log on disk.
        /// </summary>
        /// <param name="file">The error log file.</param>
        /// <returns>The managed error log instance.</returns>
        public static ManagedErrorLog ReadErrorLogFile(File file) => Instance.InstanceReadErrorLogFile(file);

        /// <summary>
        /// Reads an exception on disk.
        /// </summary>
        /// <param name="file">The exception file.</param>
        /// <returns>The exception instance.</returns>
        public static System.Exception ReadExceptionFile(File file)
        {
            // The instance method is implemented in the other parts of the partial class in platform-specific projects.
            return Instance.InstanceReadExceptionFile(file);
        }

        /// <summary>
        /// Saves an error log and an exception on disk.
        /// </summary>
        /// <param name="exception">The exception that caused the crash.</param>
        /// <param name="errorLog">The error log.</param>
        public static void SaveErrorLogFiles(System.Exception exception, ManagedErrorLog errorLog) => Instance.InstanceSaveErrorLogFiles(exception, errorLog);

        /// <summary>
        /// Deletes an error log from disk.
        /// </summary>
        /// <param name="errorId">The ID for the error log.</param>
        public static void RemoveStoredErrorLogFile(Guid errorId) => Instance.InstanceRemoveStoredErrorLogFile(errorId);

        /// <summary>
        /// Deletes an exception from disk.
        /// </summary>
        /// <param name="errorId">The ID for the exception.</param>
        public static void RemoveStoredExceptionFile(Guid errorId) => Instance.InstanceRemoveStoredExceptionFile(errorId);

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
                AppLaunchTimestamp = _processInformation.ProcessStartTime?.ToUniversalTime(),
                Architecture = _processInformation.ProcessArchitecture,
                Fatal = true,
                Exception = CreateModelException(exception)
            };
        }

        public virtual IEnumerable<File> InstanceGetErrorLogFiles()
        {
            lock (LockObject)
            {
                try
                {
                    // Convert to list so enumeration does not occur outside the lock.
                    return InstanceGetErrorStorageDirectory().EnumerateFiles($"*{ErrorLogFileExtension}").ToList();
                }
                catch (System.Exception ex)
                {
                    AppCenterLog.Error(Crashes.LogTag, "Failed to retrieve error log files.", ex);
                }
                return new List<File>();
            }
        }

        private File InstanceGetStoredErrorLogFile(Guid errorId)
        {
            return GetStoredFile(errorId, ErrorLogFileExtension);
        }

        public virtual File InstanceGetStoredExceptionFile(Guid errorId)
        {
            return GetStoredFile(errorId, ExceptionFileExtension);
        }

        /// <summary>
        /// Reads an error log file from the given file.
        /// </summary>
        /// <param name="file">The file that contains error log.</param>
        /// <returns>An error log instance or null if the file doesn't contain an error log.</returns>
        public virtual ManagedErrorLog InstanceReadErrorLogFile(File file)
        {
            try
            {
                var errorLogString = file.ReadAllText();
                return (ManagedErrorLog)LogSerializer.DeserializeLog(errorLogString);
            }
            catch (System.Exception e)
            {
                AppCenterLog.Error(Crashes.LogTag, $"Encountered an unexpected error while reading an error log file: {file.Name}", e);
            }
            return null;
        }

        /// <summary>
        /// Saves an error log and an exception on disk.
        /// Get the error storage directory, or creates it if it does not exist.
        /// </summary>
        /// <returns>The error storage directory.</returns>
        public virtual Directory InstanceGetErrorStorageDirectory()
        {
            _crashesDirectory.Create();
            return _crashesDirectory;
        }

        /// <summary>
        /// Saves an error log on disk.
        /// </summary>
        /// <param name="exception">The exception that caused the crash.</param>
        /// <param name="errorLog">The error log.</param>
        public virtual void InstanceSaveErrorLogFiles(System.Exception exception, ManagedErrorLog errorLog)
        {
            try
            {
                // Serialize main log file.
                var errorLogString = LogSerializer.Serialize(errorLog);
                var errorLogFileName = errorLog.Id + ErrorLogFileExtension;
                AppCenterLog.Debug(Crashes.LogTag, "Saving uncaught exception.");
                var directory = InstanceGetErrorStorageDirectory();
                directory.CreateFile(errorLogFileName, errorLogString);
                AppCenterLog.Debug(Crashes.LogTag, $"Saved error log in directory {ErrorStorageDirectoryName} with name {errorLogFileName}.");

                // Serialize binary exception.
                var exceptionFileName = errorLog.Id + ExceptionFileExtension;
                SaveExceptionFile(directory, exceptionFileName, exception);
            }
            catch (System.Exception ex)
            {
                AppCenterLog.Error(Crashes.LogTag, "Failed to save error log.", ex);
            }
        }

        public virtual void InstanceRemoveStoredErrorLogFile(Guid errorId)
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

        public virtual void InstanceRemoveStoredExceptionFile(Guid errorId)
        {
            lock (LockObject)
            {
                var file = GetStoredExceptionFile(errorId);
                if (file != null)
                {
                    AppCenterLog.Info(Crashes.LogTag, $"Deleting exception file {file.Name}.");
                    try
                    {
                        file.Delete();
                    }
                    catch (System.Exception ex)
                    {
                        AppCenterLog.Warn(Crashes.LogTag, $"Failed to delete exception file {file.Name}.", ex);
                    }
                }
            }
        }

        public virtual void InstanceRemoveAllStoredErrorLogFiles()
        {
            lock (LockObject)
            {
                _crashesDirectory.Refresh();

                // Crashes directory will not exist if disabling has already deleted the folder.
                if (_crashesDirectory.Exists())
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
                    AppCenterLog.Debug(Crashes.LogTag, "Deleted crashes local files.");
                }
            }
        }

        internal static ModelException CreateModelException(System.Exception exception)
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
                    return InstanceGetErrorStorageDirectory().EnumerateFiles(fileName).SingleOrDefault();
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
