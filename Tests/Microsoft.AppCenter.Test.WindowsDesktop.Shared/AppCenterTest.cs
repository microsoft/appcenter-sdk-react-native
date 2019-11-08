// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Xunit;

namespace Microsoft.AppCenter.Test.WindowsDesktop
{
    public class AppCenterTest
    {
        /// <summary>
        /// Verify configure with WindowsDesktop platform id
        /// </summary>
        [Fact]
        public void VerifyPlatformId()
        {
            var appClientId = AppCenter.GetSecretAndTargetForPlatform("windowsdesktop=6a367cda-2c0a-4fb0-bedf-f110bf4e338b", "windowsdesktop");
            Assert.Equal("6a367cda-2c0a-4fb0-bedf-f110bf4e338b", appClientId);
        }
    }
}
