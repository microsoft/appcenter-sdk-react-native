using System;

namespace Microsoft.Azure.Mobile.Utils
{
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
        protected override string GetSdkName()
        {
            throw new NotImplementedException();
        }

        protected override string GetDeviceModel()
        {
            throw new NotImplementedException();
        }

        public override event EventHandler InformationInvalidated;
        protected override string GetDeviceOemName()
        {
            throw new NotImplementedException();
        }

        protected override string GetOsName()
        {
            throw new NotImplementedException();
        }

        protected override string GetOsBuild()
        {
            throw new NotImplementedException();
        }

        protected override string GetOsVersion()
        {
            throw new NotImplementedException();
        }

        protected override string GetAppVersion()
        {
            throw new NotImplementedException();
        }

        protected override string GetAppBuild()
        {
            throw new NotImplementedException();
        }

        protected override string GetScreenSize()
        {
            throw new NotImplementedException();
        }
    }
}
