// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Analytics
{
    using Analytics = Microsoft.AppCenter.Analytics.Analytics;

    public class TrackEventTest
    {
        private readonly string _appSecret = Guid.NewGuid().ToString();

        [Fact]
        public async Task TrackEventWithoutPropertiesAsync()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter();
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Test TrackEvent.
            Analytics.TrackEvent("Hello World");

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify.
            // TODO: Complete this verification for the test with JSON content.
            Assert.Equal("POST", httpNetworkAdapter.Method);
        }
    }
}
