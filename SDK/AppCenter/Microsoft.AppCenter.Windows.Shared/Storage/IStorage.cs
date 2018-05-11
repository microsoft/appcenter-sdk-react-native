using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Storage
{
    /// <inheritdoc />
    /// <summary>
    /// Manages the database of App Center logs on disk
    /// </summary>
    public interface IStorage : IDisposable
    {
        /// <summary>
        /// Asynchronously adds a log to storage
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the log</param>
        /// <param name="log">The log to add</param>
        /// <exception cref="StorageException"/>
        Task PutLog(string channelName, Log log);

        /// <summary>
        /// Asynchronously deletes all logs in a particular batch
        /// </summary>
        /// <param name="channelName">The name of the channel associated with the batch</param>
        /// <param name="batchId">The batch identifier</param>
        /// <exception cref="StorageException"/>
        Task DeleteLogs(string channelName, string batchId);

        /// <summary>
        /// Asynchronously deletes all logs for a particular channel
        /// </summary>
        /// <param name="channelName">Name of the channel to delete logs for</param>
        /// <exception cref="StorageException"/>
        Task DeleteLogs(string channelName);

        /// <summary>
        /// Asynchronously counts the number of logs stored for a particular channel
        /// </summary>
        /// <param name="channelName">The name of the channel to count logs for</param>
        /// <returns>The number of logs found in storage</returns>
        /// <exception cref="StorageException"/>
        Task<int> CountLogsAsync(string channelName);

        /// <summary>
        /// Asynchronously clears the stored state of logs that have been retrieved
        /// </summary>
        /// <param name="channelName"></param>
        Task ClearPendingLogState(string channelName);

        /// <summary>
        /// Asynchronously retrieves logs from storage and flags them to avoid duplicate retrievals on subsequent calls
        /// </summary>
        /// <param name="channelName">Name of the channel to retrieve logs from</param>
        /// <param name="limit">The maximum number of logs to retrieve</param>
        /// <param name="logs">A list to which the retrieved logs will be added</param>
        /// <returns>A batch ID for the set of returned logs; null if no logs are found</returns>
        /// <exception cref="StorageException"/>
        Task<string> GetLogsAsync(string channelName, int limit, List<Log> logs);

        /// <summary>
        /// Waits for any running storage operations to complete.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for remaining tasks</param>
        /// <returns>True if remaining tasks completed in time; false otherwise</returns>
        Task WaitOperationsAsync(TimeSpan timeout);

        /// <summary>
        /// Waits for any running storage operations to complete and prevents subsequent storage operations from running
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for remaining tasks</param>
        /// <returns>True if remaining tasks completed in time; false otherwise</returns>
        Task<bool> ShutdownAsync(TimeSpan timeout);
    }
}
