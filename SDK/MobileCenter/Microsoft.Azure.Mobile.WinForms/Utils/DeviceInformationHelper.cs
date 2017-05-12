using System;
using System.Management;
using System.Reflection;
using System.Windows.Forms;

namespace Microsoft.Azure.Mobile.Utils
{
    /// <summary>
    /// Implements the abstract device information helper class
    /// </summary>
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        public static event EventHandler InformationInvalidated;
        
        protected override string GetSdkName()
        {
            return "mobilecenter.winforms";
        }

        protected override string GetDeviceModel()
        {
            var managementClass = new ManagementClass("Win32_ComputerSystem");
            foreach (var managementObject in managementClass.GetInstances())
            {
                return (string)managementObject["Model"];
            }
            return string.Empty;
        }

        protected override string GetAppNamespace()
        {
             return Assembly.GetEntryAssembly().EntryPoint.DeclaringType.Namespace;
        }

        protected override string GetDeviceOemName()
        {
            var managementClass = new ManagementClass("Win32_ComputerSystem");
            foreach (var managementObject in managementClass.GetInstances())
            {
                return (string)managementObject["Manufacturer"];
            }
            return string.Empty;
        }

        protected override string GetOsName()
        {
            return Environment.OSVersion.Platform.ToString();
        }

        protected override string GetOsBuild()
        {
            var version = Environment.OSVersion.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        protected override string GetOsVersion()
        {
            var version = Environment.OSVersion.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        protected override string GetAppVersion()
        {
            return Application.ProductVersion;
        }

        protected override string GetAppBuild()
        {
            return Application.ProductVersion;
        }

        protected override string GetScreenSize()
        {
            var size = SystemInformation.PrimaryMonitorSize;
            return $"{size.Width}x{size.Height}";
        }
    }
}
