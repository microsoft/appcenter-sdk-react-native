// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Crashes.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using ModelBinary = Microsoft.AppCenter.Crashes.Ingestion.Models.Binary;
using ModelException = Microsoft.AppCenter.Crashes.Ingestion.Models.Exception;
using ModelStackFrame = Microsoft.AppCenter.Crashes.Ingestion.Models.StackFrame;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public partial class ErrorLogHelper
    {
        private const string AddressFormat = "0x{0:x16}";

        internal static ErrorExceptionAndBinaries CreateModelExceptionAndBinaries(System.Exception exception)
        {
            var binaries = new Dictionary<long, ModelBinary>();
            var modelException = ProcessException(exception, null, binaries);
            return new ErrorExceptionAndBinaries { Binaries = binaries.Count > 0 ? binaries.Values.ToList() : null, Exception = modelException };
        }

        private static ModelException ProcessException(System.Exception exception, ModelException outerException, Dictionary<long, ModelBinary> seenBinaries)
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
                        ProcessException(innerException, modelException, seenBinaries);
                    }
                }
            }
            if (exception.InnerException != null)
            {
                modelException.InnerExceptions = modelException.InnerExceptions ?? new List<ModelException>();
                ProcessException(exception.InnerException, modelException, seenBinaries);
            }
            var stackTrace = new StackTrace(exception, true);
            var frames = stackTrace.GetFrames();

            // If there are native frames available, process them to extract image information and frame addresses.
            // The check looks odd, but there is a possibility of frames being null or empty both.
            if (frames != null && frames.Length > 0 && frames[0].HasNativeImage())
            {
                foreach (var frame in frames)
                {
                    // Get stack frame address.
                    var crashFrame = new ModelStackFrame
                    {
                        Address = string.Format(CultureInfo.InvariantCulture, AddressFormat, frame.GetNativeIP().ToInt64()),
                    };
                    if (modelException.Frames == null)
                    {
                        modelException.Frames = new List<ModelStackFrame>();
                    }
                    modelException.Frames.Add(crashFrame);

                    // Process binary.
                    var nativeImageBase = frame.GetNativeImageBase().ToInt64();
                    if (seenBinaries.ContainsKey(nativeImageBase) || nativeImageBase == 0)
                    {
                        continue;
                    }
                    var binary = ImageToBinary(frame.GetNativeImageBase());
                    if (binary != null)
                    {
                        seenBinaries[nativeImageBase] = binary;
                    }
                }
            }
            outerException?.InnerExceptions.Add(modelException);
            return modelException;
        }

        private static unsafe ModelBinary ImageToBinary(IntPtr imageBase)
        {
            // TODO - we are told that this "int.MaxValue" is safe because PEReader will only read what it must read and thus won't go out of bounds. If this is a problem, some of the parsing code from HockeyApp can be ported to get an exact size.
            // That parsing code is here: https://github.com/bitstadium/HockeySDK-Windows/blob/af56dd7f7b10f9d5f63ce33bd68dbaab8c504faf/Src/Kit.UWP/Extensibility/PEImageReader.cs#L88
            var reader = new System.Reflection.PortableExecutable.PEReader((byte*)imageBase.ToPointer(), int.MaxValue, true);
            var debugdir = reader.ReadDebugDirectory();
            var codeViewEntry = debugdir.First(entry => entry.Type == System.Reflection.PortableExecutable.DebugDirectoryEntryType.CodeView);
            var codeView = reader.ReadCodeViewDebugDirectoryData(codeViewEntry);
            var pdbPath = Path.GetFileName(codeView.Path);
            var endAddress = imageBase + reader.PEHeaders.PEHeader.SizeOfImage;
            return new ModelBinary
            {
                StartAddress = string.Format(CultureInfo.InvariantCulture, AddressFormat, imageBase.ToInt64()),
                EndAddress = string.Format(CultureInfo.InvariantCulture, AddressFormat, endAddress.ToInt64()),
                Path = pdbPath,
                Name = string.IsNullOrEmpty(pdbPath) == false ? Path.GetFileNameWithoutExtension(pdbPath) : null,
                Id = string.Format(CultureInfo.InvariantCulture, "{0:N}-{1}", codeView.Guid, codeView.Age)
            };
        }
    }
}