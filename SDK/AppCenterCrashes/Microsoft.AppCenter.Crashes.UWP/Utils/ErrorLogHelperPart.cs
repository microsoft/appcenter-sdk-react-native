// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Crashes.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using ModelBinary = Microsoft.AppCenter.Crashes.Ingestion.Models.Binary;
using ModelException = Microsoft.AppCenter.Crashes.Ingestion.Models.Exception;
using ModelStackFrame = Microsoft.AppCenter.Crashes.Ingestion.Models.StackFrame;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public partial class ErrorLogHelper
    {
        private const string AddressFormat = "0x{0:x16}";

        // A dword, which is short for "double word," is a data type definition that is specific to Microsoft Windows. As defined in the file windows.h, a dword is an unsigned, 32-bit unit of data.
        private const int DWordSize = 4;

        // These constants come from the PE format described in documentation: https://docs.microsoft.com/en-us/windows/win32/debug/pe-format.

        // Optional Header Windows-Specific field: SizeOfImage is located at the offset 56.
        private const int SizeOfImageOffset = 56;

        // At location 0x3c, the stub has the file offset to the PE signature. This information enables Windows to properly execute the image file.
        private const int SignatureOffsetLocation = 0x3C;

        // At the beginning of an object file, or immediately after the signature of an image file, is a standard COFF file header of 20 bytes.
        private const int COFFFileHeaderSize = 20;

        // Size in bytes of the address that is relative to the image base of the beginning-of-code section when it is loaded into memory.
        private const int BaseOfDataSize = 4;

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
            using (var reader = new PEReader((byte*)imageBase.ToPointer(), imageSize, true))
            {
                var debugdir = reader.ReadDebugDirectory();
                var codeViewEntry = debugdir.First(entry => entry.Type == DebugDirectoryEntryType.CodeView);

                // When attaching a debugger in release, it will break into MissingRuntimeArtifactException, just click continue as it is actually caught and recovered by the lib.
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

        private static int GetImageSize(IntPtr imageBase)
        {
            var peHeaderBytes = new byte[DWordSize];
            Marshal.Copy(imageBase + SignatureOffsetLocation, peHeaderBytes, 0, peHeaderBytes.Length);
            var peHeaderOffset = BitConverter.ToInt32(peHeaderBytes, 0);
            var peOptionalHeaderOffset = peHeaderOffset + BaseOfDataSize + COFFFileHeaderSize;
            var peOptionalHeaderBytes = new byte[DWordSize];
            Marshal.Copy(imageBase + peOptionalHeaderOffset + SizeOfImageOffset, peOptionalHeaderBytes, 0, peOptionalHeaderBytes.Length);
            return BitConverter.ToInt32(peOptionalHeaderBytes, 0);
        }
    }
}