// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Crashes.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            var imageSize = GetImageSize(imageBase);
            var reader = new System.Reflection.PortableExecutable.PEReader((byte*)imageBase.ToPointer(), imageSize, true);
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

        private static int GetImageSize(IntPtr imageBase)
        {
            // These constants come from the PE format described in documentation: https://docs.microsoft.com/en-us/windows/win32/debug/pe-format
            var sizeOfImageOffset = 56;
            var peSignatureOffsetLocation = 0x3C;
            var dwordSizeInBytes = 4;
            var sizeofCOFFFileHeader = 20;
            var baseOfDataSize = 4; //TODO verify this variable name.
            var peHeaderBytes = new byte[dwordSizeInBytes];
            Marshal.Copy(imageBase + peSignatureOffsetLocation, peHeaderBytes, 0, peHeaderBytes.Length);
            var peHeaderOffset = BitConverter.ToInt32(peHeaderBytes, 0);
            var peOptionalHeaderOffset = peHeaderOffset + baseOfDataSize + sizeofCOFFFileHeader;
            var peOptionalHeaderBytes = new byte[dwordSizeInBytes];
            Marshal.Copy(imageBase + peOptionalHeaderOffset + sizeOfImageOffset, peOptionalHeaderBytes, 0, peOptionalHeaderBytes.Length);
            return BitConverter.ToInt32(peOptionalHeaderBytes, 0);
        }
    }
}