// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace Microsoft.AppCenter.Crashes.Utils
{
    public class ProcessInformation : IProcessInformation
    {
        public DateTime? ProcessStartTime
        {
            get
            {
                try
                {
                    return Process.GetCurrentProcess().StartTime;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public int? ProcessId
        {
            get
            {
                try
                {
                    return Process.GetCurrentProcess().Id;
                }
                catch (Exception e)
                {
                    //TODO log
                    return null;
                }
            }
        }

        public string ProcessName
        {
            get
            {
                try
                {
                    return Process.GetCurrentProcess().ProcessName;
                }
                catch (Exception e)
                {
                    //TODO log
                    return null;
                }
            }
        }

        // Parent process information is not (easily) available, and not necessary.
        public int? ParentProcessId => null;

        // Parent process information is not (easily) available, and not necessary.
        public string ParentProcessName => null;

        public string ProcessArchitecture
        {
            get
            {
                try
                {
                    return Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"); // TODO improve
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}