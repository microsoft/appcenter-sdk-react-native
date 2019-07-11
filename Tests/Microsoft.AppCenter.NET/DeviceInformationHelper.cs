// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.AppCenter.Utils
{
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        private const string DefaultValue = "default";

        protected override string GetSdkName()
        {
            return DefaultValue;
        }

        protected override string GetDeviceModel()
        {
            return DefaultValue;
        }

        protected override string GetDeviceOemName()
        {
            return DefaultValue;
        }

        protected override string GetOsName()
        {
            return DefaultValue;
        }

        protected override string GetOsBuild()
        {
            return DefaultValue;
        }

        protected override string GetOsVersion()
        {
            return DefaultValue;
        }

        protected override string GetAppVersion()
        {
            return DefaultValue;
        }

        protected override string GetAppBuild()
        {
            return DefaultValue;
        }

        protected override string GetScreenSize()
        {
            return DefaultValue;
        }

        public static void RefreshDisplayCache()
        {
            
        }
    }
}
