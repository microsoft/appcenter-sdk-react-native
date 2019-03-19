// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Ingestion
{
    public interface IServiceCall : IDisposable
    {
        /// <summary>
        /// Check if the call is completed due to cancellation.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Check if the call is completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Check if the call is completed due to an unhandled exception.
        /// </summary>
        bool IsFaulted { get; }

        /// <summary>
        /// HTTP payload.
        /// </summary>
        string Result { get; }

        /// <summary>
        /// The exception thrown from the pipeline.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Handle call results.
        /// </summary>
        /// <param name="continuationAction">The action to perform when the call is completed.</param>
        void ContinueWith(Action<IServiceCall> continuationAction);

        /// <summary>
        /// Cancel the call if possible.
        /// </summary>
        void Cancel();
    }
}
