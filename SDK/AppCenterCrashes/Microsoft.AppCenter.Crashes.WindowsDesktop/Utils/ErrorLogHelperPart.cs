// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes.Windows.Utils;
using ModelException = Microsoft.AppCenter.Crashes.Ingestion.Models.Exception;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public partial class ErrorLogHelper
    {
        internal static ErrorExceptionAndBinaries CreateModelExceptionAndBinaries(System.Exception exception)
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
                        modelException.InnerExceptions.Add(CreateModelExceptionAndBinaries(innerException).Exception);
                    }
                }
            }
            if (exception.InnerException != null)
            {
                modelException.InnerExceptions = modelException.InnerExceptions ?? new List<ModelException>();
                modelException.InnerExceptions.Add(CreateModelExceptionAndBinaries(exception.InnerException).Exception);
            }
            
            // Binary information is not required on this platform.
            return new ErrorExceptionAndBinaries { Exception = modelException, Binaries = null };
        }
    }
}
