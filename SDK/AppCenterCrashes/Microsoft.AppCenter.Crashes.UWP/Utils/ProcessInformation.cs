// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Windows.ApplicationModel;
using Windows.System.Diagnostics;

namespace Microsoft.AppCenter.Crashes.Utils
{
    class ProcessInformation : IProcessInformation
    {
        public DateTime? ProcessStartTime => ProcessDiagnosticInfo.GetForCurrentProcess().ProcessStartTime.DateTime;

        public int? ProcessId => (int?)ProcessDiagnosticInfo.GetForCurrentProcess().ProcessId;

        public string ProcessName => ProcessDiagnosticInfo.GetForCurrentProcess().ExecutableFileName;

        public int? ParentProcessId => (int?)ProcessDiagnosticInfo.GetForCurrentProcess().Parent?.ProcessId;

        public string ParentProcessName => ProcessDiagnosticInfo.GetForCurrentProcess().Parent?.ExecutableFileName;

        public string ProcessArchitecture => Package.Current.Id.Architecture.ToString();
    }
}