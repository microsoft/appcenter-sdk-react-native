// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Test.UWP
{
    public class TestAppCenterService : AppCenterService
    {
        protected override string ChannelName => "test_service";
        public override string ServiceName => "TestService";

        private static TestAppCenterService _instanceField;
        public static TestAppCenterService Instance => _instanceField ?? (_instanceField = new TestAppCenterService());
    }
}
