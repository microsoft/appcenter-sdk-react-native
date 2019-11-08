// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter
{
    public partial class WrapperSdk
    {
        public string WrapperSdkVersion { get; private set; }

        public string WrapperSdkName { get; private set; }

        public string WrapperRuntimeVersion { get; private set; }

        public string LiveUpdateReleaseLabel { get; private set; }

        public string LiveUpdateDeploymentKey { get; private set; }

        public string LiveUpdatePackageHash { get; private set; }

        public WrapperSdk(string wrapperSdkName,
                          string wrapperSdkVersion,
                          string wrapperRuntimeVersion = null,
                          string liveUpdateReleaseLabel = null,
                          string liveUpdateDeploymentKey = null,
                          string liveUpdatePackageHash = null)
        {
            WrapperSdkName = wrapperSdkName;
            WrapperSdkVersion = wrapperSdkVersion;
            WrapperRuntimeVersion = wrapperRuntimeVersion;
            LiveUpdateReleaseLabel = liveUpdateReleaseLabel;
            LiveUpdateDeploymentKey = liveUpdateDeploymentKey;
            LiveUpdatePackageHash = liveUpdatePackageHash;
        }
    }
}
