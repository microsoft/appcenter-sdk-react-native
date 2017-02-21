using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Utils
{
    public static class IdHelper
    {
        private const string InstallIdKey = "MobileCenterInstallId";

        public static Guid InstallId
        {
            get
            {
                var settings = new ApplicationSettings();
                return settings.GetValue(InstallIdKey, Guid.NewGuid());
            }
        }
    }
}
