// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using ModelException = Microsoft.AppCenter.Ingestion.Models.Exception;

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
    private const string ErrorStorageDirectoryName = "Microsoft.AppCenter.Error";

    /// <summary>
    /// Device information utility.
    /// </summary>
    private readonly DeviceInformationHelper _deviceInformationHelper;

    public ErrorLogHelper()
    {
        _deviceInformationHelper = new DeviceInformationHelper();
    }

    public async Task<ManagedErrorLog> CreateErrorLogAsync(System.Exception exception)
    {
        ManagedErrorLog errorLog = new ManagedErrorLog();
        errorLog.Id = Guid.NewGuid();
        errorLog.Timestamp = DateTime.UtcNow;
        errorLog.Device = await _deviceInformationHelper.GetDeviceInformationAsync();
#if WINDOWS_UWP
        // TODO get parent process info?
        errorLog.ProcessId = (int)Windows.System.Diagnostics.ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId;
        errorLog.ProcessName = Windows.System.Diagnostics.ProcessDiagnosticInfo.GetForCurrentProcess().ExecutableFileName; //TODO double check
        errorLog.Architecture = Windows.ApplicationModel.Package.Current.Id.Architecture.ToString(); //TODO unify with names for winforms
        errorLog.AppLaunchTimestamp = Windows.System.Diagnostics.ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.DateTime;
#else
        var process = System.Diagnostics.Process.GetCurrentProcess();
        try
        {
            errorLog.AppLaunchTimestamp = process.StartTime;
        }
        catch (System.Exception e) when (e is InvalidOperationException || e is PlatformNotSupportedException || e is NotSupportedException || e is System.ComponentModel.Win32Exception)
        {
            //TODO log
        }
        try
        {
            errorLog.ProcessId = process.Id;
        }
        catch (System.Exception e) when (e is InvalidOperationException || e is PlatformNotSupportedException)
        {
            //TODO log
        }
        try
        {
            errorLog.ProcessName = process.ProcessName;
        }
        catch (System.Exception e) when (e is InvalidOperationException || e is PlatformNotSupportedException || e is NotSupportedException)
        {
            //TODO log
        }
        try
        {
            errorLog.Architecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"); //TODO improve this
        }
        catch (System.Exception e) when (e is ArgumentNullException || e is System.Security.SecurityException)
        {
            //TODO log
        }
        errorLog.ErrorThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        try
        {
            errorLog.ErrorThreadName = System.Threading.Thread.CurrentThread.Name;
        }
        catch (InvalidOperationException e)
        {
            //TODO log
        }
#endif
        errorLog.Fatal = true;
        errorLog.Exception = CreateModelException(exception);
        return errorLog;
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
    /// Returns the directory where errors are stored.
    /// </summary>
    public DirectoryInfo ErrorStorageDirectory
    {
        get
        {
            // TODO exception handling. Move to a helper class? Cache this value?
            return Directory.CreateDirectory(ErrorStorageDirectoryName);
        }
    }

    /// <summary>
    /// Gets all files with the error log file extension in the error directory.
    /// </summary>
    public IEnumerable<FileInfo> GetErrorLogFiles()
    {
        return ErrorStorageDirectory.EnumerateFiles($".{ErrorLogFileExtension}");
    }

    /// <summary>
    /// Gets the most recently modified error log file.
    /// </summary>
    /// <returns>The most recently modified error log file.</returns>
    public FileInfo GetLastErrorLogFile()
    {
        FileInfo lastErrorLogFile = null;
        foreach (var errorLogFile in GetErrorLogFiles())
        {
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
    public FileInfo GetStoredErrorLogFile(Guid errorId)
    {
        return GetStoredFile(errorId, ErrorLogFileExtension);
    }

    /// <summary>
    /// Gets a file from storage with the given ID and extension.
    /// </summary>
    /// <param name="errorId">The error ID.</param>
    /// <param name="extension">The file extension.</param>
    /// <returns>The file corresponding to the given parameters, or null if not found.</returns>
    private FileInfo GetStoredFile(Guid errorId, string extension)
    {
        return ErrorStorageDirectory.GetFiles($"{errorId}.{extension}").SingleOrDefault();
    }

    /// <summary>
    /// Deletes an error log from disk.
    /// </summary>
    /// <param name="errorId">The ID for the error log.</param>
    public void RemoveStoredErrorLogFile(Guid errorId)
    {
        var file = GetStoredErrorLogFile(errorId);
        if (file != null)
        {
            AppCenterLog.Info(Crashes.LogTag, $"Deleting error log file {file.Name}");
            file.Delete();
        }
    }
}