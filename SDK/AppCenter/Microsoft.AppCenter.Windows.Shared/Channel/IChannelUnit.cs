// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Ingestion.Models;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Channel
{
    /// <summary>
    /// Specification for a leaf channel that can enqueue logs, as opposed to groups.
    /// </summary>
    public interface IChannelUnit : IChannel
    {
        /// <summary>
        /// Enqueue a log for processing
        /// </summary>
        /// <param name="log"></param>
        Task EnqueueAsync(Log log);
    }
}
