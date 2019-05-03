// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AppCenter.Ingestion.Models;

namespace Microsoft.AppCenter.Channel
{
    /// <inheritdoc />
    /// <summary>
    /// Base type for all channel events.
    /// </summary>
    public abstract class ChannelEventArgs : EventArgs
    {
        /// <summary>
        /// Log associated to the event.
        /// </summary>
        public Log Log { get; }

        /// <inheritdoc />
        /// <summary>
        /// Init event with a log.
        /// </summary>
        /// <param name="log">log associated to this event.</param>
        protected ChannelEventArgs(Log log)
        {
            Log = log;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Event called when a log is enqueued to a channel but before properties are final.
    /// </summary>
    public class EnqueuingLogEventArgs : ChannelEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        /// Init event with a log.
        /// </summary>
        /// <param name="log">log associated to this event.</param>
        public EnqueuingLogEventArgs(Log log) : base(log) { }
    }

    /// <inheritdoc />
    /// <summary>
    /// Event called to possibly filter out a log before it is persisted and scheduled for sending.
    /// </summary>
    public class FilteringLogEventArgs : ChannelEventArgs
    {
        /// <summary>
        /// Set this property to true to request the log to be filtered out.
        /// </summary>
        public bool FilterRequested { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Init event with a log.
        /// </summary>
        /// <param name="log">log associated to this event.</param>
        public FilteringLogEventArgs(Log log) : base(log) { }
    }

    /// <inheritdoc />
    /// <summary>
    /// Event called when a log is about to be sent.
    /// </summary>
    public class SendingLogEventArgs : ChannelEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        /// Init event with a log.
        /// </summary>
        /// <param name="log">log associated to this event.</param>
        public SendingLogEventArgs(Log log) : base(log) { }
    }

    /// <inheritdoc />
    /// <summary>
    /// Event called when a log has been successfully sent.
    /// </summary>
    public class SentLogEventArgs : ChannelEventArgs
    {
        /// <inheritdoc />
        /// <summary>
        /// Init event with a log.
        /// </summary>
        /// <param name="log">log associated to this event.</param>
        public SentLogEventArgs(Log log) : base(log) { }
    }

    /// <inheritdoc />
    /// <summary>
    /// Event called when a log has failed to send, even after retries and is now discarded.
    /// </summary>
    public class FailedToSendLogEventArgs : ChannelEventArgs
    {
        /// <summary>
        /// Cause of the last sending failure.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc />
        /// <summary>
        /// Init event with a log.
        /// </summary>
        /// <param name="log">log associated to this event.</param>
        /// <param name="exception">cause of the last sending failure.</param>
        public FailedToSendLogEventArgs(Log log, Exception exception) : base(log)
        {
            Exception = exception;
        }
    }
}
