// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.AppCenter.Test.Functional.Analytics
{
    using Analytics = Microsoft.AppCenter.Analytics.Analytics;

    public class TrackEventTest
    {
        private readonly string _appSecret = Guid.NewGuid().ToString();

        // TODO: The SDK couldn't be started multiple times, disable this test temporarily.
        //[Fact]
        public async Task TrackEventWithoutPropertiesAsync()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Test TrackEvent.
            Analytics.TrackEvent("Hello World");

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var actualEventName = (string)httpNetworkAdapter.JsonContent["logs"][0]["name"];
            Assert.Equal("Hello World", actualEventName);
            var typedProperties = httpNetworkAdapter.JsonContent["logs"][0]["typedProperties"];
            Assert.Null(typedProperties);
        }

        [Fact]
        public async Task TrackEventWithPropertiesAsync()
        {
            // Set up HttpNetworkAdapter.
            var httpNetworkAdapter = new HttpNetworkAdapter(expectedLogType: "event");
            DependencyConfiguration.HttpNetworkAdapter = httpNetworkAdapter;

            // Start App Center.
            AppCenter.Start(_appSecret, typeof(Analytics));

            // Build event properties.
            var properties = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" },
                { "Key3", "Value3" }
            };

            // Test TrackEvent.
            Analytics.TrackEvent("Hello World", properties);

            // Wait for processing event.
            await httpNetworkAdapter.HttpResponseTask;

            // Verify.
            Assert.Equal("POST", httpNetworkAdapter.Method);
            var actualEventName = (string)httpNetworkAdapter.JsonContent["logs"][0]["name"];
            Assert.Equal("Hello World", actualEventName);
            var typedProperties = httpNetworkAdapter.JsonContent["logs"][0]["typedProperties"];
            Assert.Equal(3, typedProperties.Count());
            for (var i = 1; i <= 3; i++)
            {
                Assert.NotNull(typedProperties.SelectToken($"[?(@.name == 'Key{i}' && @.value == 'Value{i}')]"));
            }
        }
    }
}
