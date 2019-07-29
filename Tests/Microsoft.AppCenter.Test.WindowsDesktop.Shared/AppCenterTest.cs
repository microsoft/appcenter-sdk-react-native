// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;

namespace Microsoft.AppCenter.Test.WindowsDesktop
{
    public class AppCenterTest
    {
        public AppCenterTest()
        {
            AppCenter.Instance = null;
        }

        /// <summary>
        /// Verify configure with WindowsDesktop platform id
        /// </summary>
        [Fact]
        public void VerifyPlatformId()
        {
            AppCenter.Configure("windowsdesktop=appsecret");
            Assert.True(AppCenter.Configured);
        }
    }
}
