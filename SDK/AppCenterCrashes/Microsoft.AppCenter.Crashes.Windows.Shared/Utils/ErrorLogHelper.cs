// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Ingestion.Models;
using Microsoft.AppCenter.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// For huge stack traces such as giant StackOverflowError, we keep only beginning and end of frames according to this limit.
    /// </summary>
    public const int StackFrameLimit = 256;

    /// <summary>
    /// Error log directory within application files.
    /// </summary>
    private const string ErrorDirectory = "Microsoft.AppCenter.Error";

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

    public File getErrorStorageDirectory()
    {
        File
        if (sErrorLogDirectory == null)
        {
            sErrorLogDirectory = new File(Constants.FILES_PATH, ERROR_DIRECTORY);
            FileManager.mkdir(sErrorLogDirectory.getAbsolutePath());
        }
        return sErrorLogDirectory;
    }

@NonNull
    public static File[] getStoredErrorLogFiles()
{
    File[] files = getErrorStorageDirectory().listFiles(new FilenameFilter() {
            @Override
            public boolean accept(File dir, String filename)
    {
        return filename.endsWith(ERROR_LOG_FILE_EXTENSION);
    }
});
        return files != null ? files : new File[0];
    }

@Nullable
    public static File getLastErrorLogFile()
{
    return FileManager.lastModifiedFile(getErrorStorageDirectory(), new FilenameFilter() {
            @Override
            public boolean accept(File dir, String filename)
    {
        return filename.endsWith(ERROR_LOG_FILE_EXTENSION);
    }
});
    }

@Nullable
    static File getStoredErrorLogFile(@NonNull UUID id)
{
    return getStoredFile(id, ERROR_LOG_FILE_EXTENSION);
}

public static void removeStoredErrorLogFile(@NonNull UUID id)
{
    File file = getStoredErrorLogFile(id);
    if (file != null)
    {
        AppCenterLog.info(Crashes.LOG_TAG, "Deleting error log file " + file.getName());
        FileManager.delete(file);
    }
}

@NonNull
    public static ErrorReport getErrorReportFromErrorLog(@NonNull ManagedErrorLog log, Throwable throwable)
{
    ErrorReport report = new ErrorReport();
    report.setId(log.getId().toString());
    report.setThreadName(log.getErrorThreadName());
    report.setThrowable(throwable);
    report.setAppStartTime(log.getAppLaunchTimestamp());
    report.setAppErrorTime(log.getTimestamp());
    report.setDevice(log.getDevice());
    return report;
}

@Nullable
    private static File getStoredFile(@NonNull final UUID id, @NonNull final String extension)
{
    File[] files = getErrorStorageDirectory().listFiles(new FilenameFilter() {
            @Override
            public boolean accept(File dir, String filename)
    {
        return filename.startsWith(id.toString()) && filename.endsWith(extension);
    }
});

        return files != null && files.length > 0 ? files[0] : null;
    }
}